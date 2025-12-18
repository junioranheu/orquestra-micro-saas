using Microsoft.AspNetCore.Http;
using Orquestra.Application.UseCases.CompanyInvoices.Get;
using Orquestra.Application.UseCases.CompanyUsers.CheckIfUserIsLinked;
using Orquestra.Application.UseCases.CompanyUsers.GetAllByCompanyId;
using Orquestra.Application.UseCases.Shared;
using Orquestra.Domain.Entities;
using Orquestra.Domain.Enums;
using Orquestra.Infrastructure.Data;
using Orquestra.IntegrationTests.Fixtures;
using Orquestra.IntegrationTests.Fixtures.Mocks;
using static Orquestra.Utils.Fixtures.Get;

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
            PlanType = PlanTypeEnum.Premium,
            Amount = 199.90m,
            Description = "Plano Premium"
        };

        await Fixture.Save(context, invoice);

        GetCompanyInvoice sut = CreateSut(context, adminUser);

        // Act;
        CompanyInvoice result = await sut.Execute(adminUser.UserId, companyInvoiceId: invoice.CompanyInvoiceId);

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
            PlanType = PlanTypeEnum.Basic,
            Amount = 49.90m,
            Description = "Plano Básico"
        };

        await Fixture.Save(context, invoice);

        GetCompanyInvoice sut = CreateSut(context, memberUser);

        // Act & Assert;
        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => sut.Execute(memberUser.UserId, companyInvoiceId: invoice.CompanyInvoiceId));
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
            PlanType = PlanTypeEnum.Basic,
            Amount = 79.90m,
            Description = "Plano Básico"
        };

        await Fixture.Save(context, invoice);

        GetCompanyInvoice sut = CreateSut(context, adminUser);

        // Act & Assert;
        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => sut.Execute(adminUser.UserId, companyInvoiceId: invoice.CompanyInvoiceId));
    }

    [Fact]
    public async Task Execute_ShouldReturnPaginatedInvoices_WhenUserIsAdmin()
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

        // Cria 3 invoices;
        await Fixture.Save(context, new CompanyInvoice()
        {
            CompanyInvoiceId = Guid.NewGuid(),
            CompanyId = company.CompanyId,
            PlanType = PlanTypeEnum.Premium,
            Amount = 199.90m,
            Description = "Plano Premium",
            CompanyInvoiceSituation = CompanyInvoiceSituationEnum.Paid,
            Status = true,
            CreatedDate = GetDate().AddDays(-1)
        });

        await Fixture.Save(context, new CompanyInvoice()
        {
            CompanyInvoiceId = Guid.NewGuid(),
            CompanyId = company.CompanyId,
            PlanType = PlanTypeEnum.Basic,
            Amount = 49.90m,
            Description = "Plano Básico",
            CompanyInvoiceSituation = CompanyInvoiceSituationEnum.Pending,
            Status = true,
            CreatedDate = GetDate().AddDays(-2)
        });

        await Fixture.Save(context, new CompanyInvoice()
        {
            CompanyInvoiceId = Guid.NewGuid(),
            CompanyId = company.CompanyId,
            PlanType = PlanTypeEnum.Premium,
            Amount = 199.90m,
            Description = "Plano Premium 2",
            CompanyInvoiceSituation = CompanyInvoiceSituationEnum.Paid,
            Status = true,
            CreatedDate = GetDate()
        });

        GetCompanyInvoice sut = CreateSut(context, adminUser);
        PaginationInput pagination = new() { Index = 0, Limit = 2 };

        // Act;
        (IEnumerable<CompanyInvoice> output, int count) = await sut.Execute(pagination, adminUser.UserId, company.CompanyId, null);

        // Assert;
        Assert.Equal(3, count);
        Assert.Equal(2, output.Count());
        Assert.True(output.First().CreatedDate > output.Last().CreatedDate);
    }

    [Fact]
    public async Task Execute_ShouldReturnFilteredInvoices_WhenSituationIsProvided()
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

        CompanyInvoice invoicePaid = new()
        {
            CompanyInvoiceId = Guid.NewGuid(),
            CompanyId = company.CompanyId,
            PlanType = PlanTypeEnum.Basic,
            Amount = 49.90m,
            Description = "Fatura Paga",
            CompanyInvoiceSituation = CompanyInvoiceSituationEnum.Paid,
            Status = true
        };

        await Fixture.Save(context, invoicePaid);

        CompanyInvoice invoicePending = new()
        {
            CompanyInvoiceId = Guid.NewGuid(),
            CompanyId = company.CompanyId,
            PlanType = PlanTypeEnum.Premium,
            Amount = 199.90m,
            Description = "Fatura Pendente",
            CompanyInvoiceSituation = CompanyInvoiceSituationEnum.Pending,
            Status = true
        };

        await Fixture.Save(context, invoicePending);

        GetCompanyInvoice sut = CreateSut(context, adminUser);
        PaginationInput pagination = new() { Index = 0, Limit = 10 };

        // Act;
        (IEnumerable<CompanyInvoice> output, int count) = await sut.Execute(pagination, adminUser.UserId, company.CompanyId, CompanyInvoiceSituationEnum.Paid);

        // Assert;
        Assert.Single(output);
        Assert.Equal(1, count);
        Assert.All(output, i => Assert.Equal(CompanyInvoiceSituationEnum.Paid, i.CompanyInvoiceSituation));
    }

    [Fact]
    public async Task Execute_Paginated_ShouldThrow_WhenUserIsNotAdmin()
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
            CompanyUserRole = CompanyUserRoleEnum.Member,
            Status = true
        };

        await Fixture.Save(context, member);

        GetCompanyInvoice sut = CreateSut(context, memberUser);
        PaginationInput pagination = new() { Index = 0, Limit = 5 };

        // Act & Assert;
        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => sut.Execute(pagination, memberUser.UserId, company.CompanyId, null));
    }

    [Fact]
    public async Task Execute_ShouldThrow_WhenUserIsNotLinkedToCompany()
    {
        // Arrange;
        Context context = Fixture.CreateContext();

        Company company = CompanyMock.Create();
        await Fixture.Save(context, company);

        User adminUser = UserMock.Create();
        await Fixture.Save(context, adminUser);

        // Usuário não vinculado à empresa;
        GetCompanyInvoice sut = CreateSut(context, adminUser);
        PaginationInput pagination = new() { Index = 0, Limit = 5 };

        // Cria um outro usuário vinculado à empresa, mas não o user original;
        User user2 = UserMock.Create();
        await Fixture.Save(context, user2);

        CompanyUser companyUser = new()
        {
            CompanyUserId = Guid.NewGuid(),
            CompanyId = company.CompanyId,
            UserId = user2.UserId,
            CompanyUserRole = CompanyUserRoleEnum.Member
        };

        await Fixture.Save(context, companyUser);

        // Act & Assert;
        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => sut.Execute(pagination, adminUser.UserId, company.CompanyId, null));
    }

    [Fact]
    public async Task Execute_ShouldReturnEmpty_WhenNoInvoicesExist()
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
        PaginationInput pagination = new() { Index = 0, Limit = 5 };

        // Act;
        (IEnumerable<CompanyInvoice> output, int count) = await sut.Execute(pagination, adminUser.UserId, company.CompanyId, null);

        // Assert;
        Assert.Empty(output);
        Assert.Equal(0, count);
    }

    #region helpers
    private static GetCompanyInvoice CreateSut(Context context, User user)
    {
        IHttpContextAccessor httpContextAccessor = Fixture.CreateIHttpContextAccessor(user);
        GetAllCompanyUserByCompanyId getCompanyUserByCompanyId = new(context);
        CheckIfUserIsLinkedCompanyUser checkIfUserIsLinkedCompanyUser = new(getCompanyUserByCompanyId, httpContextAccessor);

        GetCompanyInvoice getCompanyInvoice = new(context, checkIfUserIsLinkedCompanyUser);

        return getCompanyInvoice;
    }
    #endregion
}