using Mapster;
using Microsoft.AspNetCore.Http;
using Orquestra.Application.UseCases.Clients.Get;
using Orquestra.Application.UseCases.Companies.Get;
using Orquestra.Application.UseCases.CompanyUsers.CheckIfUserIsLinked;
using Orquestra.Application.UseCases.CompanyUsers.GetAllByCompanyId;
using Orquestra.Application.UseCases.Schedules.Base;
using Orquestra.Application.UseCases.Schedules.Create;
using Orquestra.Application.UseCases.Schedules.Shared;
using Orquestra.Domain.Entities;
using Orquestra.Domain.Enums;
using Orquestra.Infrastructure.Data;
using Orquestra.IntegrationTests.Fixtures;
using Orquestra.IntegrationTests.Fixtures.Mocks;
using static Orquestra.Utils.Fixtures.Get;

namespace Orquestra.IntegrationTests.Tests.Schedules;

public sealed class CreateScheduleTests
{
    [Fact]
    public async Task Execute_ShouldCreateSchedule_WhenInputIsValid()
    {
        (Context context, User user, Client _, Company company, ScheduleInput input) = await ArrangeValidScheduleAsync();
        await AddCompanyUserAsync(context, company, user);

        CreateSchedule service = CreateScheduleService(context, user);

        ScheduleOutput output = await service.Execute(user.UserId, input);
        Schedule? savedSchedule = await context.Schedules.FindAsync(output.ScheduleId);

        Assert.NotNull(output);
        Assert.Equal(input.ClientId, output.ClientId);
        Assert.Equal(input.CompanyId, output.CompanyId);
        Assert.Equal(input.Date, output.Date);

        Assert.NotNull(savedSchedule);
        Assert.Equal(output.ScheduleId, savedSchedule.ScheduleId);
    }

    [Fact]
    public async Task Execute_ShouldThrow_WhenUserNotLinkedToCompany()
    {
        (Context context, User user, Client client, Company company, ScheduleInput input) = await ArrangeValidScheduleAsync();

        // Empresa já tem outro usuário, mas não o usuário do teste;
        CompanyUser anotherUser = new()
        {
            CompanyId = company.CompanyId,
            UserId = Guid.NewGuid(),
            CompanyUserRole = CompanyUserRoleEnum.Member
        };

        await Fixture.Save(context, anotherUser);

        CreateSchedule service = CreateScheduleService(context, user);

        await Assert.ThrowsAsync<Exception>(() => service.Execute(user.UserId, input));
    }

    [Fact]
    public async Task Execute_ShouldThrow_WhenScheduleDateIsBeforeToday()
    {
        var (context, user, client, company, input) = await ArrangeValidScheduleAsync();
        await AddCompanyUserAsync(context, company, user);

        input.Date = GetDate().AddDays(-1);

        CreateSchedule service = CreateScheduleService(context, user);

        await Assert.ThrowsAsync<Exception>(() => service.Execute(user.UserId, input));
    }

    [Fact]
    public async Task Execute_ShouldThrow_WhenScheduleTypeIsInvalid()
    {
        (Context context, User user, Client _, Company company, ScheduleInput input) = await ArrangeValidScheduleAsync();
        await AddCompanyUserAsync(context, company, user);

        input.ScheduleStatus = ScheduleStatusEnum.Completed; // Tipo inválido;

        CreateSchedule service = CreateScheduleService(context, user);

        await Assert.ThrowsAsync<Exception>(() => service.Execute(user.UserId, input));
    }

    #region helpers
    private static async Task<(Context context, User user, Client client, Company company, ScheduleInput input)> ArrangeValidScheduleAsync()
    {
        Context context = Fixture.CreateContext();

        User user = UserMock.Create();
        await Fixture.Save(context, user);

        Client client = ClientMock.Create();
        await Fixture.Save(context, client);

        Company company = CompanyMock.Create();
        await Fixture.Save(context, company);

        ScheduleInput input = ScheduleMock.Create(client.ClientId, company.CompanyId).Adapt<ScheduleInput>();

        return (context, user, client, company, input);
    }

    private static CreateSchedule CreateScheduleService(Context context, User user)
    {
        IHttpContextAccessor httpContextAccessor = Fixture.CreateIHttpContextAccessor(user);
        GetCompanyUserByCompanyId getCompanyUserByCompanyId = new(context);
        CheckIfUserIsLinkedCompanyUser checkIfUserIsLinkedCompanyUser = new(getCompanyUserByCompanyId, httpContextAccessor);
        GetClient getClient = new(context, checkIfUserIsLinkedCompanyUser);
        GetCompany getCompany = new(context, checkIfUserIsLinkedCompanyUser);

        ScheduleBaseDependencies deps = new(context, checkIfUserIsLinkedCompanyUser, getClient, getCompany);

        return new CreateSchedule(deps);
    }

    private static async Task AddCompanyUserAsync(Context context, Company company, User user, CompanyUserRoleEnum role = CompanyUserRoleEnum.Member)
    {
        CompanyUser companyUser = new()
        {
            CompanyId = company.CompanyId,
            UserId = user.UserId,
            CompanyUserRole = role
        };

        await Fixture.Save(context, companyUser);
    }
    #endregion
}