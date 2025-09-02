using Mapster;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Moq;
using Orquestra.Application.UseCases.Companies.Create;
using Orquestra.Application.UseCases.Companies.Get;
using Orquestra.Application.UseCases.Companies.Shared;
using Orquestra.Application.UseCases.CompanyUsers.CheckIfUserIsLinked;
using Orquestra.Application.UseCases.CompanyUsers.GetAllByCompanyId;
using Orquestra.Application.UseCases.CompanyUsers.Invite;
using Orquestra.Application.UseCases.CompanyUsers.UpdateCurrentMain;
using Orquestra.Application.UseCases.Users.Get;
using Orquestra.Application.UseCases.Verifications.Create;
using Orquestra.Domain.Entities;
using Orquestra.Domain.Enums;
using Orquestra.Infrastructure.Data;
using Orquestra.Infrastructure.Services.Email;
using Orquestra.Infrastructure.Services.Env;
using Orquestra.IntegrationTests.Fixtures;
using Orquestra.IntegrationTests.Fixtures.Mocks;

namespace Orquestra.IntegrationTests.Tests.Companies;

public sealed class CreateCompanyTests
{
    [Fact]
    public async Task Execute_ShouldCreateCompany_WhenValidInput()
    {
        // Arrange;
        Context context = Fixture.CreateContext();

        User adminUser = UserMock.Create();
        adminUser.Role = UserRoleEnum.Administrator;
        await Fixture.Save(context, adminUser);

        CreateCompany sut = CreateSut(context, user: adminUser);

        Company company = CompanyMock.Create();
        CompanyInput input = company.Adapt<CompanyInput>();

        // Act;
        CompanyOutput result = await sut.Execute(userIdAuth: adminUser.UserId, input);

        // Assert;
        Assert.NotNull(result);
        Assert.Equal(input.Name, result.Name);
        Assert.Equal(input.Email, result.Email);
        Assert.Equal(input.Phone, result.Phone);
        Assert.Equal(input.CompanyType, result.CompanyType);
        Assert.Equal(input.City, result.City);
        Assert.Equal(input.State, result.State);
        Assert.Equal(input.ZipCode, result.ZipCode);
        Assert.Equal(input.Country, result.Country);
        Assert.Equal(input.LogoUrl, result.LogoUrl);
        Assert.Equal(input.Color, result.Color);
        Assert.False(result.Status);
    }

    #region helper
    private static CreateCompany CreateSut(Context context, User user)
    {
        IConfiguration config = Fixture.CreateConfiguration();
        IWebHostEnvironment env = Fixture.CreateDevelopmentEnvironment();
        IHttpContextAccessor httpContextAccessor = Fixture.CreateIHttpContextAccessor(user);
        EnvService envService = new(env, config);
        CreateVerification createVerification = new(context);
        GetUser getUser = new(context);
        Mock<IEmailService> emailServiceMock = Fixture.CreateEmailService();
        GetCompanyUserByCompanyId getCompanyUserByCompanyId = new(context);
        CheckIfUserIsLinkedCompanyUser checkIfUserIsLinkedCompanyUser = new(getCompanyUserByCompanyId, httpContextAccessor);
        GetCompany getCompany = new(context, checkIfUserIsLinkedCompanyUser);   
        InviteCompanyUser inviteCompanyUser = new(context, envService, createVerification, checkIfUserIsLinkedCompanyUser, getUser, getCompany, emailServiceMock.Object);
        UpdateCurrentMainCompanyUser updateCurrentMainCompanyUser = new(context, checkIfUserIsLinkedCompanyUser);

        CreateCompany createCompany = new(
            context,
            envService,
            createVerification,
            inviteCompanyUser,
            updateCurrentMainCompanyUser,
            getUser,
            emailServiceMock.Object,
            checkIfUserIsLinkedCompanyUser
        );

        return createCompany;
    }
    #endregion
}