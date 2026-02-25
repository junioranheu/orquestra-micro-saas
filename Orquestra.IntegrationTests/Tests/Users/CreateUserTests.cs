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
using Orquestra.Infrastructure.Messaging.Publishers;
using Orquestra.Infrastructure.Services.Env;
using Orquestra.IntegrationTests.Fixtures;
using static Orquestra.Utils.Fixtures.Get;

namespace Orquestra.IntegrationTests.Tests.Users;

public sealed class CreateUserTests
{
    [Fact]
    public async Task Execute_ShouldThrow_WhenInputIsInvalid()
    {
        // Arrange;
        Context context = Fixture.CreateContext();
        CreateUser sut = CreateSut(context);

        UserInput input = new() { FullName = "", Email = "a", Password = "b", InviteToken = "" };

        // Act & Assert;
        await Assert.ThrowsAsync<ArgumentException>(() => sut.Execute(input));
    }

    [Fact]
    public async Task Execute_ShouldCreateUser_AndSendEmailVerification()
    {
        // Arrange;
        Context context = Fixture.CreateContext();

        UserInput input = new()
        {
            FullName = "Junior Test",
            Email = "junior@teste.com",
            Password = "SenhaTop123@",
            InviteToken = ""
        };

        CreateUser sut = CreateSut(context);

        // Act;
        UserOutput output = await sut.Execute(input);

        // Assert: Usuário salvo corretamente;
        User? userInDb = await context.Users.AsNoTracking().FirstOrDefaultAsync(x => x.UserId == output.UserId);
        Assert.NotNull(userInDb);
        Assert.Equal(input.FullName, userInDb!.FullName);
        Assert.Equal(input.Email.ToLowerInvariant(), userInDb.Email);
        Assert.False(userInDb.Status); // Sempre falso ao criar sem invite;
    }

    [Fact]
    public async Task Execute_ShouldRejectDuplicateEmail()
    {
        // Arrange;
        Context context = Fixture.CreateContext();

        User existing = new()
        {
            FullName = "Junior Existente",
            Email = "duplicado@teste.com",
            Password = "Senha123!",
            Role = UserRoleEnum.Common,
            Status = true,
            RecoverPasswordQuestion = RecoverPasswordQuestionEnum.MotherName,
            RecoverPasswordAnswer = "Sandra"
        };

        await context.Users.AddAsync(existing);
        await context.SaveChangesAsync();

        UserInput input = new()
        {
            FullName = "Junior Outro",
            Email = existing.Email,
            Password = "SenhaNova123!",
            InviteToken = ""
        };

        CreateUser sut = CreateSut(context);

        // Act & Assert;
        await Assert.ThrowsAsync<InvalidOperationException>(() => sut.Execute(input));
    }

    [Fact]
    public async Task Execute_ShouldGenerateValidVerificationToken()
    {
        // Arrange;
        Context context = Fixture.CreateContext();

        UserInput input = new()
        {
            FullName = "Token Test",
            Email = "token@teste.com",
            Password = "Senha123@",
            InviteToken = ""
        };

        ICreateVerification verificationService = new CreateVerification(context);
        Mock<IGenericPublisher> genericPublisherMock = Fixture.CreateGenericPublisher();
        EnvService envService = new(Fixture.CreateDevelopmentEnvironment(), Fixture.CreateConfiguration());
        GetUser getUser = new(context);

        CreateUser sut = new(context, envService, verificationService, getUser, genericPublisherMock.Object);

        // Act;
        await sut.Execute(input);

        // Assert;
        Verification? createdVerification = await context.Verifications.AsNoTracking().FirstOrDefaultAsync();
        Assert.NotNull(createdVerification);
        Assert.Equal(VerificationTypeEnum.User, createdVerification!.VerificationType);
        Assert.Matches("^[A-Za-z0-9_-]+$", createdVerification.Token);
        Assert.True(createdVerification.Token.Length >= 43);
    }

    [Fact]
    public async Task Execute_ShouldLinkUser_WhenInviteTokenIsProvided()
    {
        // Arrange;
        Context context = Fixture.CreateContext();

        Guid userId = Guid.NewGuid();

        // Cria verificação de convite;
        Verification verification = new()
        {
            Token = GenerateSafeToken32Bytes(urlSafe: true),
            EntityType = VerificationTypeEnum.User.ToString(),
            EntityId = Guid.Empty,
            VerificationType = VerificationTypeEnum.CompanyUser,
            Reference = userId.ToString(),
            CreatedBy = userId
        };

        await context.Verifications.AddAsync(verification);
        await context.SaveChangesAsync();

        UserInput input = new()
        {
            FullName = "Junior Invite",
            Email = "invite@teste.com",
            Password = "Senha123@",
            InviteToken = verification.Token
        };

        CreateUser sut = CreateSut(context);

        // Act;
        UserOutput output = await sut.Execute(input);

        // Assert: Usuário criado;
        User? user = await context.Users.AsNoTracking().FirstOrDefaultAsync(u => u.UserId == output.UserId);
        Assert.NotNull(user);

        // Assert: Vinculação feita na tabela CompanyUsers;
        bool linked = await context.CompanyUsers.AsNoTracking().AnyAsync(x => x.UserId == user!.UserId);
        Assert.True(linked);
    }

    [Theory]
    [InlineData("", "user@teste.com", "Senha123@")]          // nome vazio
    [InlineData("Junior", "", "Senha123@")]                  // email vazio
    [InlineData("Junior", "invalid-email", "Senha123@")]     // email inválido
    [InlineData("Junior", "user@teste.com", "")]             // senha vazia
    public async Task Execute_ShouldThrowArgumentException_ForInvalidInputs(string fullName, string email, string password)
    {
        // Arrange;
        Context context = Fixture.CreateContext();

        UserInput input = new()
        {
            FullName = fullName,
            Email = email,
            Password = password,
            InviteToken = string.Empty
        };

        CreateUser sut = CreateSut(context);

        // Act & Assert;
        await Assert.ThrowsAsync<ArgumentException>(() => sut.Execute(input));
    }

    #region helpers
    private static CreateUser CreateSut(Context context)
    {
        IWebHostEnvironment env = Fixture.CreateDevelopmentEnvironment();
        IConfiguration config = Fixture.CreateConfiguration();
        Mock<IGenericPublisher> genericPublisherMock = Fixture.CreateGenericPublisher();
        EnvService envService = new(env, config);
        CreateVerification verificationService = new(context);
        GetUser getUser = new(context);

        return new CreateUser(context, envService, verificationService, getUser, genericPublisherMock.Object);
    }
    #endregion
}