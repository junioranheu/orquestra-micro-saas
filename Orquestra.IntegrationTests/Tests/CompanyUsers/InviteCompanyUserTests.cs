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
using Orquestra.Infrastructure.Services.Email;
using Orquestra.Infrastructure.Services.Env;
using Orquestra.IntegrationTests.Fixtures;
using Orquestra.IntegrationTests.Fixtures.Mocks;

namespace Orquestra.IntegrationTests.Tests.CompanyUsers;

public sealed class InviteCompanyUserIntegrationTests
{
    [Fact]
    public async Task Execute_ShouldThrow_WhenEmailIsEmpty()
    {
        // Arrange
        var context = Fixture.CreateContext();
        var authUser = UserMock.Create();
        await Fixture.Save(context, authUser);

        var sut = CreateSut(context, authUser);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() =>
            sut.Execute(authUser.UserId, Guid.NewGuid(), "", false));
    }

    [Fact]
    public async Task Execute_ShouldInviteNewUserWithoutAccount()
    {
        // Arrange
        var context = Fixture.CreateContext();
        var authUser = UserMock.Create();
        authUser.Role = UserRoleEnum.Administrator;
        await Fixture.Save(context, authUser);

        var company = CompanyMock.Create();
        await Fixture.Save(context, company);

        string email = "novo.usuario@teste.com";

        var emailServiceMock = Fixture.CreateEmailService();

        var sut = CreateSut(context, authUser, emailServiceMock.Object);

        // Act
        await sut.Execute(authUser.UserId, company.CompanyId, email, false);

        // Assert
        emailServiceMock.Verify(x =>
            x.SendEmail(It.Is<string>(s => s == email), It.IsAny<string>(), It.IsAny<string>(), true, null),
            Times.Once);
    }

    [Fact]
    public async Task Execute_ShouldInviteExistingUserAsMember()
    {
        // Arrange
        var context = Fixture.CreateContext();
        var authUser = UserMock.Create();
        authUser.Role = UserRoleEnum.Administrator;
        await Fixture.Save(context, authUser);

        var company = CompanyMock.Create();
        await Fixture.Save(context, company);

        var existingUser = UserMock.Create();
        await Fixture.Save(context, existingUser);

        var emailServiceMock = Fixture.CreateEmailService();

        var sut = CreateSut(context, authUser, emailServiceMock.Object);

        // Act
        await sut.Execute(authUser.UserId, company.CompanyId, existingUser.Email, false);

        // Assert
        var created = await context.CompanyUsers.AsNoTracking()
            .FirstOrDefaultAsync(x => x.UserId == existingUser.UserId && x.CompanyId == company.CompanyId);

        var createdX = await context.CompanyUsers.AsNoTracking()
            .ToListAsync();

        Assert.NotNull(created);
        Assert.Equal(CompanyUserRoleEnum.Member, created!.CompanyUserRole);
        Assert.Equal(authUser.UserId, created.InviterUserId);
        Assert.False(created.IsCurrentMainCompanyUser);

        emailServiceMock.Verify(x =>
            x.SendEmail(existingUser.Email, It.IsAny<string>(), It.IsAny<string>(), true, null),
            Times.Once);
    }

    [Fact]
    public async Task Execute_ShouldInviteExistingUserAsFirstAdministrator()
    {
        // Arrange
        var context = Fixture.CreateContext();
        var authUser = UserMock.Create();
        authUser.Role = UserRoleEnum.Administrator;
        await Fixture.Save(context, authUser);

        var company = CompanyMock.Create();
        await Fixture.Save(context, company);

        var existingUser = UserMock.Create();
        await Fixture.Save(context, existingUser);

        var emailServiceMock = Fixture.CreateEmailService();

        var sut = CreateSut(context, authUser, emailServiceMock.Object);

        // Act
        await sut.Execute(authUser.UserId, company.CompanyId, existingUser.Email, true);

        // Assert
        var created = await context.CompanyUsers.AsNoTracking()
            .FirstOrDefaultAsync(x => x.UserId == authUser.UserId && x.CompanyId == company.CompanyId);

        Assert.NotNull(created);
        Assert.Equal(CompanyUserRoleEnum.Administrator, created!.CompanyUserRole);
        Assert.True(created.IsCurrentMainCompanyUser);
        Assert.Null(created.InviterUserId);

        emailServiceMock.Verify(x => x.SendEmail(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), true, null), Times.Never);
    }

    #region Helpers
    private static InviteCompanyUser CreateSut(Context context, User authUser, IEmailService? emailService = null)
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
        IEmailService finalEmailService = emailService ?? new Mock<IEmailService>().Object;

        // Cria o SUT real com todos os serviços concretos;
        InviteCompanyUser inviteCompanyUser = new(context, envService, createVerification, checkLinked, getUser, getCompany, finalEmailService);

        return inviteCompanyUser;
    }
    #endregion
}