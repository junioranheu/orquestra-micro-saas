using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Moq;
using Orquestra.Application.UseCases.Clients.Get;
using Orquestra.Application.UseCases.Companies.Get;
using Orquestra.Application.UseCases.CompanyUsers.CheckIfUserIsLinked;
using Orquestra.Application.UseCases.CompanyUsers.GetAllByCompanyId;
using Orquestra.Application.UseCases.Schedules.Base;
using Orquestra.Application.UseCases.Schedules.Delete;
using Orquestra.Domain.Entities;
using Orquestra.Domain.Enums;
using Orquestra.Infrastructure.Data;
using Orquestra.Infrastructure.Services.Email;
using Orquestra.IntegrationTests.Fixtures;
using Orquestra.IntegrationTests.Fixtures.Mocks;
using static Orquestra.Utils.Fixtures.Get;

namespace Orquestra.IntegrationTests.Tests.Schedules;

public sealed class DeleteScheduleTests
{
    [Fact]
    public async Task Execute_ShouldSoftDeleteSchedule_WhenUserHasPermission()
    {
        (Context context, User user, Schedule schedule) = await ArrangeScheduleWithUserAsync();

        DeleteSchedule sut = CreateSut(context, user);

        await sut.Execute(user.UserId, schedule.ScheduleId);

        Schedule? deleted = await context.Schedules.FindAsync(schedule.ScheduleId);
        Assert.NotNull(deleted);
        Assert.False(deleted.Status);
    }

    [Fact]
    public async Task Execute_ShouldSoftDelete_WhenUserIsAdministratorEvenIfNotInSpecificMembers()
    {
        (Context context, User admin, Schedule schedule) = await ArrangeScheduleWithUserAsync();

        // Cria novo user como Admin da mesma empresa;
        User adminUser = UserMock.Create();
        await Fixture.Save(context, adminUser);

        CompanyUser companyAdmin = new()
        {
            CompanyUserId = Guid.NewGuid(),
            CompanyId = schedule.CompanyId,
            UserId = adminUser.UserId,
            CompanyUserRole = CompanyUserRoleEnum.Administrator,
            Status = true
        };

        await Fixture.Save(context, companyAdmin);

        DeleteSchedule sut = CreateSut(context, admin);

        await sut.Execute(adminUser.UserId, schedule.ScheduleId);

        Schedule? deleted = await context.Schedules.FindAsync(schedule.ScheduleId);
        Assert.NotNull(deleted);
        Assert.False(deleted.Status);
    }

    [Fact]
    public async Task Execute_ShouldThrow_WhenUserIsNotLinkedToCompany()
    {
        (Context context, User user, Schedule schedule) = await ArrangeScheduleWithUserAsync();

        // Remove o vínculo do usuário com a empresa;
        context.CompanyUsers.RemoveRange(context.CompanyUsers);
        await context.SaveChangesAsync();

        DeleteSchedule sut = CreateSut(context, user);

        await Assert.ThrowsAsync<KeyNotFoundException>(() => sut.Execute(user.UserId, schedule.ScheduleId));
    }

    [Fact]
    public async Task Execute_ShouldThrow_WhenUserNotInSpecificMembers()
    {
        (Context context, User user, Schedule schedule) = await ArrangeScheduleWithUserAsync();

        // Adiciona outro usuário diferente ao schedule (user não está nos colaboradores específicos);
        User other = UserMock.Create();
        await Fixture.Save(context, other);

        // Atualiza os UsersIds e persiste;
        schedule.UsersIds = [other.UserId];
        context.Schedules.Update(schedule);
        await context.SaveChangesAsync();

        // Recarrega o schedule do banco pra garantir que o valor armazenado está correto;
        Schedule dbSchedule = await context.Schedules.AsNoTracking().Where(x => x.ScheduleId == schedule.ScheduleId).FirstAsync();

        // checagem auxiliar (ajuda a diagnosticar se o problema era persistência);
        Assert.NotNull(dbSchedule.UsersIds);
        Assert.Single(dbSchedule.UsersIds);
        Assert.Equal(other.UserId, dbSchedule.UsersIds[0]);

        DeleteSchedule sut = CreateSut(context, user);

        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => sut.Execute(user.UserId, schedule.ScheduleId));
    }

    [Fact]
    public async Task Execute_ShouldThrow_WhenScheduleDoesNotExist()
    {
        (Context context, User user, Schedule _) = await ArrangeScheduleWithUserAsync();

        DeleteSchedule sut = CreateSut(context, user);

        await Assert.ThrowsAsync<KeyNotFoundException>(() => sut.Execute(user.UserId, Guid.NewGuid()));
    }

    [Fact]
    public async Task Execute_ShouldThrow_WhenScheduleIsInThePast()
    {
        (Context context, User user, Schedule schedule) = await ArrangeScheduleWithUserAsync();

        // Força o agendamento para o passado;
        schedule.DateStart = GetDate().AddDays(-1);
        context.Schedules.Update(schedule);
        await context.SaveChangesAsync();

        DeleteSchedule sut = CreateSut(context, user);

        await Assert.ThrowsAsync<InvalidOperationException>(() => sut.Execute(user.UserId, schedule.ScheduleId));
    }

    #region helpers
    private static async Task<(Context context, User user, Schedule schedule)> ArrangeScheduleWithUserAsync()
    {
        Context context = Fixture.CreateContext();

        // Cria user;
        User user = UserMock.Create();
        await Fixture.Save(context, user);

        // Cria company;
        Company company = CompanyMock.Create();
        company.Status = true;
        await Fixture.Save(context, company);

        // Cria client;
        Client client = ClientMock.Create();
        await Fixture.Save(context, client);

        // Cria vínculo do usuário com a empresa;
        CompanyUser companyUser = new()
        {
            CompanyUserId = Guid.NewGuid(),
            CompanyId = company.CompanyId,
            UserId = user.UserId,
            CompanyUserRole = CompanyUserRoleEnum.Member,
            Status = true
        };

        await Fixture.Save(context, companyUser);

        // Cria schedule (sem specific members);
        Schedule schedule = ScheduleMock.Create(client.ClientId, company.CompanyId);
        schedule.Status = true;
        schedule.UsersIds = [user.UserId];
        await Fixture.Save(context, schedule);

        return (context, user, schedule);
    }

    private static DeleteSchedule CreateSut(Context context, User user)
    {
        IHttpContextAccessor httpContextAccessor = Fixture.CreateIHttpContextAccessor(user);
        GetCompanyUserByCompanyId getCompanyUserByCompanyId = new(context);
        CheckIfUserIsLinkedCompanyUser checkIfUserIsLinkedCompanyUser = new(getCompanyUserByCompanyId, httpContextAccessor);
        GetClient getClient = new(context, checkIfUserIsLinkedCompanyUser);
        GetCompany getCompany = new(context, checkIfUserIsLinkedCompanyUser);
        Mock<IEmailService> emailServiceMock = Fixture.CreateEmailService();

        DeleteSchedule sut = new(new ScheduleBaseDependencies(
            context,
            checkIfUserIsLinkedCompanyUser,
            getClient,
            getCompany,
            emailServiceMock.Object
        ));

        return sut;
    }
    #endregion
}