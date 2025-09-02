using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Moq;
using Orquestra.Application.UseCases.Users.Create;
using Orquestra.Application.UseCases.Users.Get;
using Orquestra.Application.UseCases.Users.Shared;
using Orquestra.Application.UseCases.Verifications.Create;
using Orquestra.Domain.Entities;
using Orquestra.Domain.Enums;
using Orquestra.Infrastructure.Data;
using Orquestra.Infrastructure.Services.Email;
using Orquestra.Infrastructure.Services.Env;
using Orquestra.IntegrationTests.Fixtures;

namespace Orquestra.IntegrationTests.Tests.Users;

public sealed class CreateUserTests
{
    [Fact]
    public async Task Execute_ShouldThrow_WhenInputIsInvalid()
    {
        // Arrange;
        Context context = Fixture.CreateContext();
        CreateUser sut = CreateSut(context, new Mock<IEmailService>());

        UserInput input = new() { FullName = "", Email = "", Password = "" };

        // Act & Assert;
        await Assert.ThrowsAsync<ArgumentException>(() => sut.Execute(input));
    }

    [Fact]
    public async Task Execute_ShouldCreateUserAndSendEmail()
    {
        // Arrange;
        Context context = Fixture.CreateContext();

        UserInput input = new()
        {
            FullName = "Junior Test",
            Email = "junior@teste.com",
            Password = "FazOBellingham22@"
        };

        Dictionary<string, string>? capturedValues = null;
        Mock<IEmailService> emailServiceMock = Fixture.CreateEmailService(vals => capturedValues = new Dictionary<string, string>(vals));

        CreateUser sut = CreateSut(context, emailServiceMock);

        // Act;
        UserOutput output = await sut.Execute(input);

        // Assert: Usuário criado corretamente;
        User? userInDb = await context.Users.AsNoTracking().FirstOrDefaultAsync(u => u.UserId == output.UserId);

        Assert.NotNull(userInDb);
        Assert.Equal(input.FullName, userInDb!.FullName);
        Assert.Equal(input.Email, userInDb.Email);
        Assert.False(userInDb.Status); // Status inicial false;

        // Assert: E-mail enviado com token correto;
        Assert.NotNull(capturedValues);
        Assert.Equal("Junior", capturedValues!["[UserName]"]);
        Assert.Contains("/User/Verify/", capturedValues["[VerifyUrl]"]);

        emailServiceMock.Verify(x => x.SendEmail(input.Email, It.IsAny<string>(), It.IsAny<string>(), true, null), Times.Once);
    }

    [Fact]
    public async Task Execute_ShouldGenerateValidToken()
    {
        // Arrange
        Context context = Fixture.CreateContext();

        UserInput input = new()
        {
            FullName = "Token Test",
            Email = "token@teste.com",
            Password = "Selalau22@"
        };

        // Cria CreateVerification real, mas com token controlado;
        ICreateVerification verificationService = new CreateVerification(context);

        // EmailService mock apenas para não enviar e-mail de verdade;
        Mock<IEmailService> emailServiceMock = Fixture.CreateEmailService();

        // Ambiente e GetUser reais;
        EnvService envService = new(Fixture.CreateDevelopmentEnvironment(), Fixture.CreateConfiguration());
        GetUser getUser = new(context);

        // SUT real;
        CreateUser sut = new(context, envService, verificationService, emailServiceMock.Object, getUser);

        // Act;
        await sut.Execute(input);

        // Assert;
        Verification? createdVerification = await context.Verifications.AsNoTracking().FirstOrDefaultAsync();

        Assert.NotNull(createdVerification);
        Assert.Equal(43, createdVerification.Token.Length); // Geralmente URL-safe 32 bytes codificados ficam com 43 chars;
        Assert.Matches("^[A-Za-z0-9_-]+$", createdVerification.Token); // Somente caracteres URL-safe;
        Assert.Equal(VerificationTypeEnum.User, createdVerification.VerificationType);
    }

    [Theory]
    [InlineData("", "test@teste.com", "123456")] // nome vazio
    [InlineData("Junior Test", "", "123456")]   // email vazio
    [InlineData("Junior Test", "test@teste.com", "")] // senha vazia
    [InlineData("Junior Test", "invalid-email", "123456")] // email inválido
    [InlineData("Junior Test", "test@teste.com", "123")]   // senha curta
    [InlineData("", "", "")] // tudo vazio
    public async Task Execute_ShouldThrowArgumentException_ForInvalidInputs(string fullName, string email, string password)
    {
        // Arrange;
        Context context = Fixture.CreateContext();

        UserInput input = new()
        {
            FullName = fullName,
            Email = email,
            Password = password
        };

        Mock<IEmailService> emailServiceMock = Fixture.CreateEmailService();
        CreateUser sut = CreateSut(context, emailServiceMock);

        // Act & Assert;
        await Assert.ThrowsAsync<ArgumentException>(() => sut.Execute(input));
    }

    [Theory]
    [InlineData("", "junior@teste.com", "Senha123!")]        // Nome vazio;
    [InlineData("A", "junior@teste.com", "Senha123!")]       // Nome muito curto;
    [InlineData("  ", "junior@teste.com", "Senha123!")]      // Nome só espaços;
    [InlineData("Junior Test", "", "Senha123!")]             // Email vazio;
    [InlineData("Junior Test", "  ", "Senha123!")]           // Email só espaços;
    [InlineData("Junior Test", "junior", "Senha123!")]       // Email inválido;
    [InlineData("Junior Test", "junior@teste.com", "")]      // Senha vazia;
    [InlineData("Junior Test", "junior@teste.com", "  ")]    // Senha só espaços;
    [InlineData("Junior Test", "junior@teste.com", "123")]   // Senha pequena;
    public async Task Execute_ShouldThrow_WhenInputIsInvalidOrWeird(string fullName, string email, string password)
    {
        // Arrange;
        Context context = Fixture.CreateContext();

        UserInput input = new()
        {
            FullName = fullName,
            Email = email,
            Password = password
        };

        Mock<IEmailService> emailServiceMock = Fixture.CreateEmailService();
        CreateUser sut = CreateSut(context, emailServiceMock);

        // Act & Assert;
        await Assert.ThrowsAsync<ArgumentException>(() => sut.Execute(input));
    }

    [Fact]
    public async Task Execute_ShouldRejectDuplicateEmail()
    {
        // Arrange;
        Context context = Fixture.CreateContext();

        UserInput input = new()
        {
            FullName = "Junior Test",
            Email = "duplicate@teste.com",
            Password = "SenhaSegura123!"
        };

        // Salva usuário original;
        context.Users.Add(new User
        {
            FullName = input.FullName,
            Email = input.Email.ToLowerInvariant(),
            Password = input.Password,
            Role = UserRoleEnum.Common,
            Status = true
        });

        await context.SaveChangesAsync();

        Mock<IEmailService> emailServiceMock = Fixture.CreateEmailService();
        CreateUser sut = CreateSut(context, emailServiceMock);

        // Act & Assert;
        await Assert.ThrowsAsync<InvalidOperationException>(() => sut.Execute(input));
    }

    #region helpers
    private static CreateUser CreateSut(Context context, Mock<IEmailService> emailService)
    {
        IWebHostEnvironment env = Fixture.CreateDevelopmentEnvironment();
        IConfiguration config = Fixture.CreateConfiguration();
        EnvService envService = new(env, config);
        CreateVerification verificationService = new(context);
        GetUser getUser = new(context);

        CreateUser createUser = new(context, envService, verificationService, emailService.Object, getUser);

        return createUser;
    }
    #endregion
}