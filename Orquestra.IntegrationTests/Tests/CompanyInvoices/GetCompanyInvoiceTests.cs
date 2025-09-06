using Microsoft.AspNetCore.Http;
using Orquestra.Application.UseCases.CompanyInvoices.Get;
using Orquestra.Application.UseCases.CompanyUsers.CheckIfUserIsLinked;
using Orquestra.Application.UseCases.CompanyUsers.GetAllByCompanyId;
using Orquestra.Domain.Entities;
using Orquestra.Domain.Enums;
using Orquestra.Infrastructure.Data;
using Orquestra.IntegrationTests.Fixtures;
using Orquestra.IntegrationTests.Fixtures.Mocks;

namespace Orquestra.IntegrationTests.Tests.CompanyInvoices;

public sealed class GetCompanyInvoiceTests
{
    [Fact]
    public async Task Execute_ShouldReturnInvoice_WhenUserIsAdmin()
    {
        // Arrange;
        Context context = Fixture.CreateContext();

        Company company = CompanyMock.Create();
        await Fixture.Save(context, company);

        User adminUser = UserMock.Create();
        await Fixture.Save(context, adminUser);

        CompanyUser adminCompanyUser = new()
        {
            CompanyId = company.CompanyId,
            UserId = adminUser.UserId,
            CompanyUserRole = CompanyUserRoleEnum.Administrator,
            Status = true
        };

        await Fixture.Save(context, adminCompanyUser);

        CompanyInvoice invoice = new()
        {
            CompanyInvoiceId = Guid.NewGuid(),
            CompanyId = company.CompanyId,
            Amount = 199.90m,
            Description = "Plano Premium"
        };

        await Fixture.Save(context, invoice);

        GetCompanyInvoice sut = CreateSut(context, adminUser);

        // Act;
        CompanyInvoice result = await sut.Execute(adminUser.UserId, company.CompanyId);

        // Assert;
        Assert.NotNull(result);
        Assert.Equal(invoice.CompanyInvoiceId, result.CompanyInvoiceId);
        Assert.Equal(invoice.Amount, result.Amount);
        Assert.Equal(invoice.Description, result.Description);
    }

    [Fact]
    public async Task Execute_ShouldThrow_WhenInvoiceDoesNotExist()
    {
        // Arrange;
        Context context = Fixture.CreateContext();

        Company company = CompanyMock.Create();
        await Fixture.Save(context, company);

        User adminUser = UserMock.Create();
        await Fixture.Save(context, adminUser);

        CompanyUser adminCompanyUser = new()
        {
            CompanyId = company.CompanyId,
            UserId = adminUser.UserId,
            CompanyUserRole = CompanyUserRoleEnum.Administrator,
            Status = true
        };

        await Fixture.Save(context, adminCompanyUser);

        GetCompanyInvoice sut = CreateSut(context, adminUser);

        // Act & Assert;
        await Assert.ThrowsAsync<KeyNotFoundException>(() => sut.Execute(adminUser.UserId, company.CompanyId));
    }

    [Fact]
    public async Task Execute_ShouldThrow_WhenUserIsNotAdmin()
    {
        // Arrange;
        Context context = Fixture.CreateContext();

        Company company = CompanyMock.Create();
        await Fixture.Save(context, company);

        User memberUser = UserMock.Create();
        await Fixture.Save(context, memberUser);

        CompanyUser member = new()
        {
            CompanyId = company.CompanyId,
            UserId = memberUser.UserId,
            CompanyUserRole = CompanyUserRoleEnum.Member, // Não é admin;
            Status = true
        };

        await Fixture.Save(context, member);

        CompanyInvoice invoice = new()
        {
            CompanyInvoiceId = Guid.NewGuid(),
            CompanyId = company.CompanyId,
            Amount = 49.90m,
            Description = "Plano Básico"
        };

        await Fixture.Save(context, invoice);

        GetCompanyInvoice sut = CreateSut(context, memberUser);

        // Act & Assert;
        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => sut.Execute(memberUser.UserId, company.CompanyId));
    }

    [Fact]
    public async Task Execute_ShouldThrow_WhenAdminUserIsNotLinkedToCompany()
    {
        // Arrange;
        Context context = Fixture.CreateContext();

        Company company = CompanyMock.Create();
        await Fixture.Save(context, company);

        User adminUser = UserMock.Create(); // Usuário criado;
        await Fixture.Save(context, adminUser);

        CompanyUser member = new()
        {
            CompanyId = company.CompanyId,
            UserId = Guid.NewGuid(), // Outro usuário;
            CompanyUserRole = CompanyUserRoleEnum.Administrator, // É admin;
            Status = true
        };

        await Fixture.Save(context, member);

        // Não é criado CompanyUser para esse adminUser → não está vinculado à empresa;
        CompanyInvoice invoice = new()
        {
            CompanyInvoiceId = Guid.NewGuid(),
            CompanyId = company.CompanyId,
            Amount = 79.90m,
            Description = "Plano Intermediário"
        };

        await Fixture.Save(context, invoice);

        GetCompanyInvoice sut = CreateSut(context, adminUser);

        // Act & Assert;
        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => sut.Execute(adminUser.UserId, company.CompanyId));
    }

    #region helpers
    private static GetCompanyInvoice CreateSut(Context context, User user)
    {
        IHttpContextAccessor httpContextAccessor = Fixture.CreateIHttpContextAccessor(user);
        GetCompanyUserByCompanyId getCompanyUserByCompanyId = new(context);
        CheckIfUserIsLinkedCompanyUser checkIfUserIsLinkedCompanyUser = new(getCompanyUserByCompanyId, httpContextAccessor);

        GetCompanyInvoice getCompanyInvoice = new(context, checkIfUserIsLinkedCompanyUser);

        return getCompanyInvoice;
    }
    #endregion
}