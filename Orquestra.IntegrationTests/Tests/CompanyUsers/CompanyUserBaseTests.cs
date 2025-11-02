using Microsoft.AspNetCore.Http;
using Orquestra.Application.UseCases.CompanyUsers.Base;
using Orquestra.Application.UseCases.CompanyUsers.CheckIfUserIsLinked;
using Orquestra.Application.UseCases.CompanyUsers.GetAllByCompanyId;
using Orquestra.Application.UseCases.CompanyUsers.Shared;
using Orquestra.Domain.Entities;
using Orquestra.Domain.Enums;
using Orquestra.Infrastructure.Data;
using Orquestra.IntegrationTests.Fixtures;
using Orquestra.IntegrationTests.Fixtures.Mocks;
using static Orquestra.Utils.Fixtures.Get;

namespace Orquestra.IntegrationTests.Tests.CompanyUsers;

public class CompanyUserBaseTests
{
    [Fact]
    public async Task Validate_ShouldPass_WhenCreatingFirstAdministrator()
    {
        // Arrange;
        Context context = Fixture.CreateContext();

        Company company = new()
        {
            Name = "Empresa Teste",
            Email = "empresa@teste.com",
            Phone = "11999999999",
            CompanyType = CompanyTypeEnum.ClinicaOdontologia,
            Country = "Brasil",
            ZipCode = "12345678",
            Status = false // ainda não verificada
        };

        await context.Companies.AddAsync(company);
        await context.SaveChangesAsync();

        User user = new()
        {
            FullName = "Usuário Criador",
            Email = "criador@teste.com",
            Password = "Senha123!",
            Role = UserRoleEnum.Common,
            RecoverPasswordQuestion = RecoverPasswordQuestionEnum.BirthCity,
            RecoverPasswordAnswer = GetRandomString(10),
            Status = true
        };

        await context.Users.AddAsync(user);
        await context.SaveChangesAsync();

        CompanyUserInput input = new()
        {
            CompanyId = company.CompanyId,
            UserId = user.UserId,
            CompanyUserRole = CompanyUserRoleEnum.Administrator
        };

        CompanyUserBase sut = CreateSut(context, user);

        // Act & Assert;
        await sut.Validate(input, user.UserId, isCreate: true);
    }

    [Fact]
    public async Task Validate_ShouldThrow_WhenCompanyNotFound()
    {
        // Arrange;
        Context context = Fixture.CreateContext();

        User user = UserMock.Create();
        await Fixture.Save(context, user);

        CompanyUserInput input = new()
        {
            CompanyId = Guid.NewGuid(),
            UserId = user.UserId,
            CompanyUserRole = CompanyUserRoleEnum.Member
        };

        CompanyUserBase sut = CreateSut(context, user);

        // Act & Assert;
        await Assert.ThrowsAsync<KeyNotFoundException>(() => sut.Validate(input, user.UserId, isCreate: true));
    }

    [Fact]
    public async Task Validate_ShouldThrow_WhenCompanyNotVerified_AndNotFirstAdmin()
    {
        // Arrange;
        Context context = Fixture.CreateContext();

        User user = UserMock.Create();
        await Fixture.Save(context, user);

        Company company = CompanyMock.Create();
        await Fixture.Save(context, company);

        CompanyUser existing = new()
        {
            CompanyId = company.CompanyId,
            UserId = user.UserId,
            CompanyUserRole = CompanyUserRoleEnum.Administrator,
            Status = true
        };

        await context.CompanyUsers.AddAsync(existing);
        await context.SaveChangesAsync();

        CompanyUserInput input = new()
        {
            CompanyId = company.CompanyId,
            UserId = user.UserId,
            CompanyUserRole = CompanyUserRoleEnum.Administrator
        };

        CompanyUserBase sut = CreateSut(context, user);

        // Act & Assert;
        await Assert.ThrowsAsync<InvalidOperationException>(() => sut.Validate(input, user.UserId, isCreate: true));
    }

    [Fact]
    public async Task Validate_ShouldThrow_WhenUserAlreadyExistsInCompany()
    {
        // Arrange;
        Context context = Fixture.CreateContext();

        User user = UserMock.Create();
        await Fixture.Save(context, user);

        Company company = CompanyMock.Create();
        await Fixture.Save(context, company);

        CompanyUser existing = new()
        {
            CompanyId = company.CompanyId,
            UserId = user.UserId,
            CompanyUserRole = CompanyUserRoleEnum.Administrator,
            Status = true
        };

        await context.CompanyUsers.AddAsync(existing);
        await context.SaveChangesAsync();

        CompanyUserInput input = new()
        {
            CompanyId = company.CompanyId,
            UserId = user.UserId,
            CompanyUserRole = CompanyUserRoleEnum.Administrator
        };

        CompanyUserBase sut = CreateSut(context, user);

        // Act & Assert;
        await Assert.ThrowsAsync<InvalidOperationException>(() => sut.Validate(input, user.UserId, isCreate: true));
    }

    [Fact]
    public async Task Validate_ShouldThrow_WhenUpdatingAndUserNotLinked()
    {
        // Arrange;
        Context context = Fixture.CreateContext();

        User user = UserMock.Create();
        await Fixture.Save(context, user);

        Company company = CompanyMock.Create();
        await Fixture.Save(context, company);

        CompanyUserInput input = new()
        {
            CompanyId = company.CompanyId,
            UserId = user.UserId,
            CompanyUserRole = CompanyUserRoleEnum.Member
        };

        CompanyUserBase sut = CreateSut(context, user);

        // Act & Assert;
        await Assert.ThrowsAsync<InvalidOperationException>(() => sut.Validate(input, user.UserId, isCreate: false));
    }

    [Fact]
    public async Task Validate_ShouldPass_WhenUpdatingExistingCompanyUser()
    {
        // Arrange;
        Context context = Fixture.CreateContext();

        User user = UserMock.Create();
        await Fixture.Save(context, user);

        Company company = CompanyMock.Create();
        await Fixture.Save(context, company);

        CompanyUser existing = new()
        {
            CompanyId = company.CompanyId,
            UserId = user.UserId,
            CompanyUserRole = CompanyUserRoleEnum.Administrator,
            Status = true
        };

        await context.CompanyUsers.AddAsync(existing);
        await context.SaveChangesAsync();

        CompanyUserInput input = new()
        {
            CompanyId = company.CompanyId,
            UserId = user.UserId,
            CompanyUserRole = CompanyUserRoleEnum.Administrator
        };

        CompanyUserBase sut = CreateSut(context, user);

        // Act & Assert;
        await sut.Validate(input, user.UserId, isCreate: false);
    }

    [Fact]
    public async Task Validate_ShouldThrow_WhenUserIsNotAdmin()
    {
        // Arrange;
        Context context = Fixture.CreateContext();

        User user = UserMock.Create();
        await Fixture.Save(context, user);

        Company company = CompanyMock.Create();
        await Fixture.Save(context, company);

        CompanyUser existing = new()
        {
            CompanyId = company.CompanyId,
            UserId = user.UserId,
            CompanyUserRole = CompanyUserRoleEnum.Member,
            Status = true
        };

        await context.CompanyUsers.AddAsync(existing);
        await context.SaveChangesAsync();

        CompanyUserInput input = new()
        {
            CompanyId = company.CompanyId,
            UserId = user.UserId,
            CompanyUserRole = CompanyUserRoleEnum.Member
        };

        CompanyUserBase sut = CreateSut(context, user);

        // Act & Assert;
        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => sut.Validate(input, user.UserId, isCreate: false));
    }

    #region helpers
    private static CompanyUserBase CreateSut(Context context, User user)
    {
        IHttpContextAccessor httpContextAccessor = Fixture.CreateIHttpContextAccessor(user);
        GetCompanyUserByCompanyId getCompanyUserByCompanyId = new(context);
        CheckIfUserIsLinkedCompanyUser checkIfUserIsLinkedCompanyUser = new(getCompanyUserByCompanyId, httpContextAccessor);

        CompanyUserBase sut = new(context, checkIfUserIsLinkedCompanyUser);

        return sut;
    }
    #endregion
}