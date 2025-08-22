using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Moq;
using Orquestra.Application.UseCases.CompanyUsers.CheckIfUserIsLinked;
using Orquestra.Application.UseCases.CompanyUsers.CreateRange;
using Orquestra.Application.UseCases.CompanyUsers.GetAllByCompanyId;
using Orquestra.Application.UseCases.CompanyUsers.Shared;
using Orquestra.Domain.Entities;
using Orquestra.Domain.Enums;
using Orquestra.Infrastructure.Data;
using Orquestra.Infrastructure.Services.Email;
using Orquestra.IntegrationTests.Fixtures;
using Orquestra.IntegrationTests.Fixtures.Mocks;

namespace Orquestra.IntegrationTests.Tests.CompanyUsers;

public sealed class CreateRangeTests
{
    [Fact]
    public async Task Execute_ShouldCreateFirstAdministrator_AndNormalizeFlags()
    {
        // Arrange;
        Context context = Fixture.CreateContext();

        Company company = CompanyMock.Create();
        await Fixture.Save(context, company);

        // Auth user como sys admin (passa em qualquer validação de admin);
        User authUser = UserMock.Create();
        authUser.Role = UserRoleEnum.Administrator;
        await Fixture.Save(context, authUser);

        // Usuário que será vinculado como primeiro administrador da empresa;
        User targetUser = UserMock.Create();
        await Fixture.Save(context, targetUser);

        Mock<IEmailService> emailServiceMock = new();
        CreateRangeCompanyUser sut = CreateSut(context, authUser, emailServiceMock.Object);

        List<CompanyUserInput> input =
        [
            new()
            {
                CompanyId = company.CompanyId,
                UserId = targetUser.UserId,
                CompanyUserRole = CompanyUserRoleEnum.Administrator
            }
        ];

        // Act;
        List<CompanyUserOutput> output = await sut.Execute(authUser.UserId, input);

        // Assert;
        Assert.Single(output);

        CompanyUser? created = await context.CompanyUsers.AsNoTracking().FirstOrDefaultAsync(x => x.CompanyId == company.CompanyId && x.UserId == targetUser.UserId);

        Assert.NotNull(created);
        Assert.Equal(CompanyUserRoleEnum.Administrator, created!.CompanyUserRole);
        Assert.True(created.IsAccountVerified);
        Assert.True(created.IsCurrentMainCompanyUser);
        Assert.False(string.IsNullOrWhiteSpace(created.VerifyToken));
    }

    [Fact]
    public async Task Execute_ShouldCreateMember_WithDefaultFlags()
    {
        // Arrange;
        Context context = Fixture.CreateContext();

        Company company = CompanyMock.Create();
        await Fixture.Save(context, company);

        User authUser = UserMock.Create();
        authUser.Role = UserRoleEnum.Administrator; // Sys admin;
        await Fixture.Save(context, authUser);

        User memberUser = UserMock.Create();
        await Fixture.Save(context, memberUser);

        Mock<IEmailService> emailServiceMock = new();
        CreateRangeCompanyUser sut = CreateSut(context, authUser, emailServiceMock.Object);

        List<CompanyUserInput> input =
        [
            new()
            {
                CompanyId = company.CompanyId,
                UserId = memberUser.UserId,
                CompanyUserRole = CompanyUserRoleEnum.Member
            }
        ];

        // Act;
        List<CompanyUserOutput> output = await sut.Execute(authUser.UserId, input);

        // Assert;
        Assert.Single(output);

        CompanyUser? created = await context.CompanyUsers.AsNoTracking().FirstOrDefaultAsync(x => x.CompanyId == company.CompanyId && x.UserId == memberUser.UserId);

        Assert.NotNull(created);
        Assert.Equal(CompanyUserRoleEnum.Member, created.CompanyUserRole);
        Assert.False(created.IsAccountVerified);
        Assert.False(created.IsCurrentMainCompanyUser);
        Assert.False(string.IsNullOrWhiteSpace(created.VerifyToken));
    }

    [Fact]
    public async Task Execute_ShouldCreateMultipleUsers_AdminNotAutoVerified_WhenBatch()
    {
        // Arrange;
        Context context = Fixture.CreateContext();

        Company company = CompanyMock.Create();
        await Fixture.Save(context, company);

        User authUser = UserMock.Create();
        authUser.Role = UserRoleEnum.Common;
        await Fixture.Save(context, authUser);

        User adminUser = UserMock.Create();
        await Fixture.Save(context, adminUser);

        User memberUser = UserMock.Create();
        await Fixture.Save(context, memberUser);

        Mock<IEmailService> emailServiceMock = new();
        CreateRangeCompanyUser sut = CreateSut(context, authUser, emailServiceMock.Object);

        // Batch com admin + member (não é "primeiro admin" isolado);
        List<CompanyUserInput> input =
        [
            new()
            {
                CompanyId = company.CompanyId,
                UserId = adminUser.UserId,
                CompanyUserRole = CompanyUserRoleEnum.Administrator
            },
            new()
            {
                CompanyId = company.CompanyId,
                UserId = memberUser.UserId,
                CompanyUserRole = CompanyUserRoleEnum.Member
            }
        ];

        // Act;
        var output = await sut.Execute(authUser.UserId, input);

        // Assert;
        Assert.Equal(2, output.Count);

        List<CompanyUser> created = await context.CompanyUsers.
                                    AsNoTracking().
                                    Where(x =>
                                       x.CompanyId == company.CompanyId &&
                                       (x.UserId == adminUser.UserId || x.UserId == memberUser.UserId)
                                    ).ToListAsync();

        Assert.Equal(2, created.Count);

        var createdAdmin = created.First(x => x.UserId == adminUser.UserId);
        var createdMember = created.First(x => x.UserId == memberUser.UserId);

        // Como foi batch, o admin NÃO recebe auto-verificação/flag de main;
        Assert.Equal(CompanyUserRoleEnum.Administrator, createdAdmin.CompanyUserRole);
        Assert.False(createdAdmin.IsAccountVerified);
        Assert.False(createdAdmin.IsCurrentMainCompanyUser);
        Assert.False(string.IsNullOrWhiteSpace(createdAdmin.VerifyToken));

        Assert.Equal(CompanyUserRoleEnum.Member, createdMember.CompanyUserRole);
        Assert.False(createdMember.IsAccountVerified);
        Assert.False(createdMember.IsCurrentMainCompanyUser);
        Assert.False(string.IsNullOrWhiteSpace(createdMember.VerifyToken));
    }

    [Fact]
    public async Task Execute_ShouldThrow_WhenUserAlreadyLinked()
    {
        // Arrange;
        Context context = Fixture.CreateContext();

        Company company = CompanyMock.Create();
        await Fixture.Save(context, company);

        User authUser = UserMock.Create();
        authUser.Role = UserRoleEnum.Administrator;
        await Fixture.Save(context, authUser);

        User linkedUser = UserMock.Create();
        await Fixture.Save(context, linkedUser);

        // Já vinculado na empresa;
        CompanyUser existing = new()
        {
            CompanyId = company.CompanyId,
            UserId = linkedUser.UserId,
            CompanyUserRole = CompanyUserRoleEnum.Member
        };

        await Fixture.Save(context, existing);

        Mock<IEmailService> emailServiceMock = new();
        CreateRangeCompanyUser sut = CreateSut(context, authUser, emailServiceMock.Object);

        List<CompanyUserInput> input =
        [
            new()
            {
                CompanyId = company.CompanyId,
                UserId = linkedUser.UserId,
                CompanyUserRole = CompanyUserRoleEnum.Administrator
            }
        ];

        // Act + Assert;
        await Assert.ThrowsAsync<Exception>(() => sut.Execute(authUser.UserId, input));
    }

    [Fact]
    public async Task Execute_ShouldThrow_WhenAuthUserNotAuthorized()
    {
        // Arrange;
        Context context = Fixture.CreateContext();

        Company company = CompanyMock.Create();
        await Fixture.Save(context, company);

        User authUser = UserMock.Create();
        authUser.Role = UserRoleEnum.Common; // Não é sys admin;
        await Fixture.Save(context, authUser);

        User targetUser = UserMock.Create();
        await Fixture.Save(context, targetUser);

        Mock<IEmailService> emailServiceMock = new();
        CreateRangeCompanyUser sut = CreateSut(context, authUser, emailServiceMock.Object);

        List<CompanyUserInput> input =
        [
            new()
            {
                CompanyId = company.CompanyId,
                UserId = targetUser.UserId,
                CompanyUserRole = CompanyUserRoleEnum.Member
            }
        ];

        // Act + Assert;
        await sut.Execute(targetUser.UserId, input); // Criar primeiro administrador;
        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => sut.Execute(authUser.UserId, input)); // Testar com usuário não linkado à empresa;
    }

    [Fact]
    public async Task Execute_ShouldThrow_WhenInputIsEmpty()
    {
        // Arrange;
        Context context = Fixture.CreateContext();

        Company company = CompanyMock.Create();
        await Fixture.Save(context, company);

        User authUser = UserMock.Create();
        authUser.Role = UserRoleEnum.Administrator;
        await Fixture.Save(context, authUser);

        Mock<IEmailService> emailServiceMock = new();
        CreateRangeCompanyUser sut = CreateSut(context, authUser, emailServiceMock.Object);

        List<CompanyUserInput> input = [];

        // Act + Assert;
        await Assert.ThrowsAsync<ArgumentException>(() => sut.Execute(authUser.UserId, input));
    }

    [Fact]
    public async Task Execute_ShouldNormalizeFlags_WhenFirstAdminExistsButNotVerified()
    {
        // Arrange;
        Context context = Fixture.CreateContext();

        Company company = CompanyMock.Create();
        await Fixture.Save(context, company);

        User authUser = UserMock.Create();
        authUser.Role = UserRoleEnum.Administrator;
        await Fixture.Save(context, authUser);

        User existingAdmin = UserMock.Create();
        await Fixture.Save(context, existingAdmin);

        // Admin já existe, mas não verificado;
        CompanyUser existing = new()
        {
            CompanyId = company.CompanyId,
            UserId = existingAdmin.UserId,
            CompanyUserRole = CompanyUserRoleEnum.Administrator,
            IsAccountVerified = false,
            IsCurrentMainCompanyUser = false
        };

        await Fixture.Save(context, existing);

        User newAdmin = UserMock.Create();
        await Fixture.Save(context, newAdmin);

        Mock<IEmailService> emailServiceMock = new();
        CreateRangeCompanyUser sut = CreateSut(context, authUser, emailServiceMock.Object);

        List<CompanyUserInput> input =
        [
            new()
            {
                CompanyId = company.CompanyId,
                UserId = newAdmin.UserId,
                CompanyUserRole = CompanyUserRoleEnum.Administrator
            }
        ];

        // Act;
        List<CompanyUserOutput> output = await sut.Execute(authUser.UserId, input);

        // Assert;
        Assert.Single(output);

        CompanyUser? created = await context.CompanyUsers.AsNoTracking().FirstOrDefaultAsync(x => x.CompanyId == company.CompanyId && x.UserId == newAdmin.UserId);

        Assert.NotNull(created);
        Assert.Equal(CompanyUserRoleEnum.Administrator, created!.CompanyUserRole);
        Assert.False(created.IsCurrentMainCompanyUser);
    }

    #region helpers
    private static CreateRangeCompanyUser CreateSut(Context context, User authUser, IEmailService emailService)
    {
        IHttpContextAccessor httpContextAccessor = Fixture.CreateIHttpContextAccessor(authUser);
        GetCompanyUserByCompanyId getCompanyUserByCompanyId = new(context);
        var checkIfUserIsLinked = new CheckIfUserIsLinkedCompanyUser(getCompanyUserByCompanyId, httpContextAccessor);

        return new CreateRangeCompanyUser(context, checkIfUserIsLinked, emailService);
    }
    #endregion
}