using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Moq;
using Orquestra.Application.UseCases.Companies.Get;
using Orquestra.Application.UseCases.CompanyUsers.CheckIfUserIsLinked;
using Orquestra.Application.UseCases.CompanyUsers.GetAllByCompanyId;
using Orquestra.Application.UseCases.CompanyUsers.Invite;
using Orquestra.Application.UseCases.Users.Get;
using Orquestra.Application.UseCases.Verifications.Create;
using Orquestra.Domain.Entities;
using Orquestra.Domain.Enums;
using Orquestra.Infrastructure.Data;
using Orquestra.Infrastructure.Messaging.Publishers;
using Orquestra.Infrastructure.Services.Env;
using Orquestra.IntegrationTests.Fixtures;
using Orquestra.IntegrationTests.Fixtures.Mocks;

namespace Orquestra.IntegrationTests.Tests.CompanyUsers;

public sealed class InviteCompanyUserIntegrationTests
{
    [Fact]
    public async Task Execute_ShouldThrow_WhenEmailIsEmpty()
    {
        // Arrange;
        Context context = Fixture.CreateContext();
        var authUser = UserMock.Create();
        await Fixture.Save(context, authUser);

        InviteCompanyUser sut = CreateSut(context, authUser);

        // Act & Assert;
        await Assert.ThrowsAsync<ArgumentException>(() => sut.Execute(authUser.UserId, Guid.NewGuid(), "", false));
    }

    [Fact]
    public async Task Execute_ShouldInviteNewUserWithoutAccount()
    {
        // Arrange;
        Context context = Fixture.CreateContext();
        User authUser = UserMock.Create();
        authUser.Role = UserRoleEnum.Administrator;
        await Fixture.Save(context, authUser);

        Company company = CompanyMock.Create();
        await Fixture.Save(context, company);

        string email = "novo.usuario@teste.com";

        InviteCompanyUser sut = CreateSut(context, authUser);

        // Act;
        await sut.Execute(authUser.UserId, company.CompanyId, email, false);
    }

    [Fact]
    public async Task Execute_ShouldInviteExistingUserAsMember()
    {
        // Arrange;
        Context context = Fixture.CreateContext();
        var authUser = UserMock.Create();
        authUser.Role = UserRoleEnum.Administrator;
        await Fixture.Save(context, authUser);

        Company company = CompanyMock.Create();
        await Fixture.Save(context, company);

        User existingUser = UserMock.Create();
        await Fixture.Save(context, existingUser);

        InviteCompanyUser sut = CreateSut(context, authUser);

        // Act;
        await sut.Execute(authUser.UserId, company.CompanyId, existingUser.Email, false);

        // Assert;
        CompanyUser? created = await context.CompanyUsers.AsNoTracking().FirstOrDefaultAsync(x => x.UserId == existingUser.UserId && x.CompanyId == company.CompanyId);

        Assert.NotNull(created);
        Assert.Equal(CompanyUserRoleEnum.Member, created!.CompanyUserRole);
        Assert.Equal(authUser.UserId, created.InviterUserId);
        Assert.False(created.IsCurrentMainCompanyUser);
    }

    [Fact]
    public async Task Execute_ShouldInviteExistingUserAsFirstAdministrator()
    {
        // Arrange;
        Context context = Fixture.CreateContext();
        User authUser = UserMock.Create();
        authUser.Role = UserRoleEnum.Administrator;
        await Fixture.Save(context, authUser);

        Company company = CompanyMock.Create();
        await Fixture.Save(context, company);

        User existingUser = UserMock.Create();
        await Fixture.Save(context, existingUser);

        InviteCompanyUser sut = CreateSut(context, authUser);

        // Act;
        await sut.Execute(authUser.UserId, company.CompanyId, existingUser.Email, true);

        // Assert;
        CompanyUser? created = await context.CompanyUsers.AsNoTracking().FirstOrDefaultAsync(x => x.UserId == authUser.UserId && x.CompanyId == company.CompanyId);

        Assert.NotNull(created);
        Assert.Equal(CompanyUserRoleEnum.Administrator, created!.CompanyUserRole);
        Assert.True(created.IsCurrentMainCompanyUser);
        Assert.Null(created.InviterUserId);
    }

    [Fact]
    public async Task ShouldCreateVerification_WithAllRequiredFields()
    {
        // Arrange;
        Context context = Fixture.CreateContext();

        Verification verification = new()
        {
            VerificationId = Guid.NewGuid(),
            Token = "token123",
            EntityId = Guid.NewGuid(),
            EntityType = nameof(CompanyUser),
            VerificationType = VerificationTypeEnum.CompanyUser,
            Reference = "teste@orquestra.com",
            Used = false
        };

        // Act;
        await context.Verifications.AddAsync(verification);
        await context.SaveChangesAsync();

        // Assert;
        Verification? saved = await context.Verifications.AsNoTracking().FirstOrDefaultAsync(v => v.VerificationId == verification.VerificationId);

        Assert.NotNull(saved);
        Assert.Equal(verification.Token, saved!.Token);
        Assert.Equal(verification.EntityId, saved.EntityId);
        Assert.Equal(verification.EntityType, saved.EntityType);
        Assert.Equal(verification.VerificationType, saved.VerificationType);
        Assert.Equal(verification.Reference, saved.Reference);
        Assert.False(saved.Used);
    }

    [Fact]
    public async Task ShouldMarkVerificationAsUsed()
    {
        // Arrange;
        Context context = Fixture.CreateContext();

        Verification verification = new()
        {
            VerificationId = Guid.NewGuid(),
            Token = "token456",
            EntityId = Guid.NewGuid(),
            EntityType = nameof(User),
            VerificationType = VerificationTypeEnum.User,
            Reference = null,
            Used = false
        };

        await context.Verifications.AddAsync(verification);
        await context.SaveChangesAsync();

        // Act;
        verification.Used = true;
        context.Verifications.Update(verification);
        await context.SaveChangesAsync();

        // Assert;
        Verification? saved = await context.Verifications.AsNoTracking().FirstOrDefaultAsync(v => v.VerificationId == verification.VerificationId);

        Assert.NotNull(saved);
        Assert.True(saved.Used);
    }

    [Fact]
    public async Task ShouldAllowNullReference()
    {
        // Arrange;
        Context context = Fixture.CreateContext();

        Verification verification = new()
        {
            VerificationId = Guid.NewGuid(),
            Token = "token789",
            EntityId = Guid.NewGuid(),
            EntityType = nameof(User),
            VerificationType = VerificationTypeEnum.User,
            Reference = null,
            Used = false
        };

        // Act;
        await context.Verifications.AddAsync(verification);
        await context.SaveChangesAsync();

        // Assert;
        Verification? saved = await context.Verifications.AsNoTracking().FirstOrDefaultAsync(v => v.VerificationId == verification.VerificationId);

        Assert.NotNull(saved);
        Assert.Null(saved!.Reference);
    }

    [Fact]
    public async Task ShouldPersistMultipleVerifications()
    {
        // Arrange;
        Context context = Fixture.CreateContext();

        Verification verification1 = new()
        {
            VerificationId = Guid.NewGuid(),
            Token = "token1",
            EntityId = Guid.NewGuid(),
            EntityType = nameof(User),
            VerificationType = VerificationTypeEnum.User
        };

        Verification verification2 = new()
        {
            VerificationId = Guid.NewGuid(),
            Token = "token2",
            EntityId = Guid.NewGuid(),
            EntityType = nameof(CompanyUser),
            VerificationType = VerificationTypeEnum.CompanyUser
        };

        // Act;
        await context.Verifications.AddRangeAsync(verification1, verification2);
        await context.SaveChangesAsync();

        // Assert;
        int count = await context.Verifications.CountAsync();
        Assert.Equal(2, count);
    }

    [Fact]
    public async Task Execute_ShouldThrow_WhenAuthUserNotLinkedAsAdmin()
    {
        // Arrange;
        Context context = Fixture.CreateContext();

        User authUser = UserMock.Create(); // Not linked to company;
        await Fixture.Save(context, authUser);

        Company company = CompanyMock.Create();
        await Fixture.Save(context, company);

        User target = UserMock.Create();
        await Fixture.Save(context, target);

        CompanyUser companyUser = new()
        {
            CompanyId = company.CompanyId,
            UserId = target.UserId,
            CompanyUserRole = CompanyUserRoleEnum.Member
        };

        await Fixture.Save(context, companyUser);

        InviteCompanyUser sut = CreateSut(context, authUser);

        // Act & Assert: authUser is not linked as admin -> should throw;
        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => sut.Execute(authUser.UserId, company.CompanyId, target.Email, false));
    }

    [Fact]
    public async Task Execute_ShouldNormalizeEmail_ToLowercase_WhenInvitingNoAccount()
    {
        // Arrange;
        Context context = Fixture.CreateContext();

        User authUser = UserMock.Create();
        authUser.Role = UserRoleEnum.Administrator;
        await Fixture.Save(context, authUser);

        Company company = CompanyMock.Create();
        await Fixture.Save(context, company);

        string mixedCaseEmail = "Foo.Bar@Example.COM";

        InviteCompanyUser sut = CreateSut(context, authUser);

        // Act;
        await sut.Execute(authUser.UserId, company.CompanyId, mixedCaseEmail, false);
    }

    [Fact]
    public async Task Execute_ShouldThrow_WhenCompanyNotFound()
    {
        // Arrange;
        Context context = Fixture.CreateContext();

        User authUser = UserMock.Create();
        await Fixture.Save(context, authUser);

        // Don't create company in DB => GetCompany should throw;
        InviteCompanyUser sut = CreateSut(context, authUser);

        // Act & Assert;
        await Assert.ThrowsAsync<KeyNotFoundException>(() => sut.Execute(authUser.UserId, Guid.NewGuid(), "xx@x.com", false));
    }

    [Fact]
    public async Task Execute_FirstAdministrator_ShouldUseAuthUserId_EvenIfTargetUserExists()
    {
        // Arrange;
        Context context = Fixture.CreateContext();

        User authUser = UserMock.Create();
        authUser.Role = UserRoleEnum.Administrator;
        await Fixture.Save(context, authUser);

        Company company = CompanyMock.Create();
        await Fixture.Save(context, company);

        User existing = UserMock.Create();
        await Fixture.Save(context, existing);

        InviteCompanyUser sut = CreateSut(context, authUser);

        // Act;
        await sut.Execute(authUser.UserId, company.CompanyId, existing.Email, true);

        // Assert: created CompanyUser must reference authUser.UserId (first admin uses userIdAuth);
        CompanyUser? created = await context.CompanyUsers.AsNoTracking().FirstOrDefaultAsync(x => x.CompanyId == company.CompanyId && x.UserId == authUser.UserId);

        Assert.NotNull(created);
        Assert.Equal(CompanyUserRoleEnum.Administrator, created!.CompanyUserRole);
        Assert.True(created.IsCurrentMainCompanyUser);
        Assert.Null(created.InviterUserId);
    }

    [Fact]
    public async Task Execute_ShouldPassLowercaseCompanyUserRole_ToTemplateValues()
    {
        // Arrange;
        Context context = Fixture.CreateContext();

        User authUser = UserMock.Create();
        authUser.Role = UserRoleEnum.Administrator;
        await Fixture.Save(context, authUser);

        Company company = CompanyMock.Create();
        await Fixture.Save(context, company);

        User existing = UserMock.Create();
        await Fixture.Save(context, existing);

        InviteCompanyUser sut = CreateSut(context, authUser);

        // Act;
        await sut.Execute(authUser.UserId, company.CompanyId, existing.Email, false);
    }

    [Fact]
    public async Task Execute_ShouldThrow_WhenUserAlreadyLinkedToCompany()
    {
        // Arrange;
        Context context = Fixture.CreateContext();
        User authUser = UserMock.Create();
        await Fixture.Save(context, authUser);

        User existingUser = UserMock.Create();
        await Fixture.Save(context, existingUser);

        Company company = CompanyMock.Create();
        await Fixture.Save(context, company);

        // Adiciona usuário já como CompanyUser;
        CompanyUser alreadyLinked = new()
        {
            CompanyId = company.CompanyId,
            UserId = existingUser.UserId,
            CompanyUserRole = CompanyUserRoleEnum.Member
        };

        await context.AddAsync(alreadyLinked);
        await context.SaveChangesAsync();

        InviteCompanyUser sut = CreateSut(context, authUser);

        // Act & Assert;
        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => sut.Execute(authUser.UserId, company.CompanyId, existingUser.Email, false));
    }

    [Fact]
    public async Task Execute_ShouldThrow_WhenEmailIsInvalid()
    {
        // Arrange;
        Context context = Fixture.CreateContext();
        User authUser = UserMock.Create();
        await Fixture.Save(context, authUser);

        Company company = CompanyMock.Create();
        await Fixture.Save(context, company);

        InviteCompanyUser sut = CreateSut(context, authUser);

        string invalidEmail = "email-invalido";

        // Act & Assert;
        await Assert.ThrowsAsync<ArgumentException>(() => sut.Execute(authUser.UserId, company.CompanyId, invalidEmail, false));
    }

    [Fact]
    public async Task Execute_ShouldSendFrontendLink_WhenUserHasNoAccount()
    {
        // Arrange;
        Context context = Fixture.CreateContext();
        User authUser = UserMock.Create();
        authUser.Role = UserRoleEnum.Administrator;
        await Fixture.Save(context, authUser);

        Company company = CompanyMock.Create();
        await Fixture.Save(context, company);

        string email = "novo.usuario@teste.com";

        InviteCompanyUser sut = CreateSut(context, authUser);

        // Act;
        await sut.Execute(authUser.UserId, company.CompanyId, email, false);
    }

    [Fact]
    public async Task Execute_ShouldSendBackendLink_WhenUserAlreadyHasAccount()
    {
        // Arrange;
        Context context = Fixture.CreateContext();
        User authUser = UserMock.Create();
        authUser.Role = UserRoleEnum.Administrator;
        await Fixture.Save(context, authUser);

        Company company = CompanyMock.Create();
        await Fixture.Save(context, company);

        User existingUser = UserMock.Create();
        await Fixture.Save(context, existingUser);

        InviteCompanyUser sut = CreateSut(context, authUser);

        // Act;
        await sut.Execute(authUser.UserId, company.CompanyId, existingUser.Email, false);
    }

    [Fact]
    public async Task Execute_ShouldGenerateValidTokenInVerifyUrl_WhenUserAlreadyHasAccount()
    {
        // Arrange;
        Context context = Fixture.CreateContext();
        User authUser = UserMock.Create();
        authUser.Role = UserRoleEnum.Administrator;
        await Fixture.Save(context, authUser);

        Company company = CompanyMock.Create();
        await Fixture.Save(context, company);

        User existingUser = UserMock.Create();
        await Fixture.Save(context, existingUser);

        InviteCompanyUser sut = CreateSut(context, authUser);

        // Act;
        await sut.Execute(authUser.UserId, company.CompanyId, existingUser.Email, false);
    }

    #region helpers
    private static InviteCompanyUser CreateSut(Context context, User authUser)
    {
        // Cria o IHttpContextAccessor real com o usuário autenticado;
        IHttpContextAccessor httpContextAccessor = Fixture.CreateIHttpContextAccessor(authUser);

        IWebHostEnvironment envMock = Fixture.CreateDevelopmentEnvironment();

        // Cria a implementação real do CheckIfUserIsLinkedCompanyUser;
        GetCompanyUserByCompanyId getCompanyUserByCompanyId = new(context);
        CheckIfUserIsLinkedCompanyUser checkLinked = new(getCompanyUserByCompanyId, httpContextAccessor);
        IConfiguration configuration = Fixture.CreateConfiguration();

        // Serviços concretos ou mocks mínimos;
        IEnvService envService = new EnvService(envMock, configuration);
        ICreateVerification createVerification = new CreateVerification(context);
        IGetUser getUser = new GetUser(context);
        IGetCompany getCompany = new GetCompany(context, checkLinked);
        Mock<IGenericPublisher> genericPublisherMock = Fixture.CreateGenericPublisher();

        // Cria o SUT real com todos os serviços concretos;
        InviteCompanyUser inviteCompanyUser = new(context, envService, createVerification, checkLinked, getUser, getCompany, genericPublisherMock.Object);

        return inviteCompanyUser;
    }
    #endregion
}