using Mapster;
using Microsoft.EntityFrameworkCore;
using Orquestra.Application.UseCases.Clients.Get;
using Orquestra.Application.UseCases.Companies.Get;
using Orquestra.Application.UseCases.CompanyUsers.CheckIfUserIsLinked;
using Orquestra.Application.UseCases.Schedules.Shared;
using Orquestra.Application.UseCases.Users.Shared;
using Orquestra.Domain.Consts;
using Orquestra.Domain.Entities;
using Orquestra.Domain.Enums;
using Orquestra.Infrastructure.Data;
using Orquestra.Infrastructure.Services.Email;
using static Orquestra.Utils.Fixtures.Get;
using static Orquestra.Utils.Fixtures.RegexPatterns;

namespace Orquestra.Application.UseCases.Schedules.Base;

public record ScheduleBaseDependencies(
    Context Context,
    ICheckIfUserIsLinkedCompanyUser CheckIfUserIsLinkedCompanyUser,
    IGetClient GetClient,
    IGetCompany GetCompany,
    IEmailService EmailService
);

public partial class ScheduleBase(ScheduleBaseDependencies deps)
{
    private readonly Context _context = deps.Context;
    private readonly ICheckIfUserIsLinkedCompanyUser _checkIfUserIsLinkedCompanyUser = deps.CheckIfUserIsLinkedCompanyUser;
    private readonly IGetClient _getClient = deps.GetClient;
    private readonly IGetCompany _getCompany = deps.GetCompany;
    private readonly IEmailService _emailService = deps.EmailService;

    public async Task Validate(ScheduleInput input, Guid userIdAuth, bool isCreate, bool mustValidateDate = true)
    {
        await _checkIfUserIsLinkedCompanyUser.Execute(companyId: input.CompanyId, userId: userIdAuth, needCompanyAdmin: false);

        bool checkCustomUrl = IsCustomUrlValid(input.CustomUrl);

        if (!checkCustomUrl)
        {
            throw new ArgumentException("A URL não é válida. Insira uma URL válida, por favor.");
        }

        if (isCreate)
        {
            if (input.ScheduleStatus != ScheduleStatusEnum.Scheduled)
            {
                throw new ArgumentException($"O status de uma consulta recém criada deve ser {GetStatusDesc(ScheduleStatusEnum.Scheduled)}.");
            }
        }

        if (mustValidateDate)
        {
            // Normalizar a data do input (que vem do front-end), para ficar UTC;
            DateTime dateStartUtc = ConvertToUtc(input.DateStart);

            if (dateStartUtc <= GetDate())
            {
                throw new ArgumentException("Não é possível agendar para uma data/hora passada.");
            }
        }

        // Verifica se a data final é anterior a de início;
        if (input.DateEnd <= input.DateStart)
        {
            throw new ArgumentException("A data de término deve ser posterior à data de início.");
        }

        // Normalizar a data do input (que vem do front-end), para ficar UTC;
        DateTime dateStartBr = ConvertToBrasiliaTime(input.DateStart);
        DateTime dateEndBr = ConvertToBrasiliaTime(input.DateEnd);

        // Verifica se os dias (início e fim) estão diferentes;
        if (dateStartBr.Date != dateEndBr.Date)
        {
            throw new ArgumentException($"O agendamento deve terminar até 23:59 do dia {GetDateDetails(date: input.DateStart, withHour: false)}.");
        }

        _ = await _getClient.Execute(userIdAuth: userIdAuth, clientId: input.ClientId) ?? throw new KeyNotFoundException(SystemConsts.Warn_NotFound_Client);

        _ = await _getCompany.Execute(userIdAuth: userIdAuth, companyId: input.CompanyId, throwIfStatusFalse: true) ?? throw new KeyNotFoundException(SystemConsts.Warn_NotFound_Company);

        await ValidateUsersAndRemoveNotLinkedOnes(input);
    }

    public async Task<List<string>> CheckForObservations(ScheduleOutput? schedule)
    {
        List<string> observations = [];

        if (schedule is null)
        {
            return observations;
        }

        if (schedule.DateStart == default || schedule.DateStart == DateTime.MinValue)
        {
            observations.Add("Agendamento sem data definida.");
        }

        if (schedule.DateEnd == default || schedule.DateEnd == DateTime.MinValue)
        {
            observations.Add("Agendamento sem data de encerramento definida.");
        }

        if (schedule.ScheduleStatus == ScheduleStatusEnum.Scheduled && schedule.DateStart < GetDate())
        {
            observations.Add($"Agendamento consta como {GetStatusDesc(ScheduleStatusEnum.Scheduled)}, mas já passou da data prevista.");
        }

        if (schedule.ScheduleStatus == ScheduleStatusEnum.Canceled && schedule.DateStart < GetDate())
        {
            observations.Add($"Agendamento consta como {GetStatusDesc(ScheduleStatusEnum.Canceled)} após a data prevista.");
        }

        if (schedule.ScheduleStatus == ScheduleStatusEnum.Completed && schedule.DateStart > GetDate())
        {
            observations.Add($"Agendamento consta {GetStatusDesc(ScheduleStatusEnum.Completed)}, mas a data ainda não ocorreu.");
        }

        var conflicts = await _context.Schedules.
                        AsNoTracking().
                        Where(x =>
                           x.CompanyId == schedule.CompanyId &&
                           x.DateStart.Date == schedule.DateStart.Date && // Mesma data;
                           x.DateStart.Hour == schedule.DateStart.Hour && // Mesma hora;
                           x.DateStart.Minute == schedule.DateStart.Minute && // Mesmo minuto;
                           x.ScheduleStatus == ScheduleStatusEnum.Scheduled &&
                           x.ScheduleId != schedule.ScheduleId && // Ignora o próprio registro;
                           x.Status == true
                        ).ToListAsync();

        if (conflicts.Count != 0)
        {
            string msg = conflicts.Count == 1 ? $"Existe outro agendamento ativo na mesma data e hora." : $"Existem {conflicts.Count} outros agendamentos ativos na mesma data e hora.";
            observations.Add(msg);
        }

        return observations;
    }

    public async Task<UserOutput[]> GetUsers(Guid[]? users)
    {
        if (users is null || users?.Length == 0)
        {
            return [];
        }

        var result = await _context.Users.
                     AsNoTracking().
                     Where(x => users!.Contains(x.UserId) && x.Status == true).
                     ToListAsync();

        if (result?.Count == 0)
        {
            return [];
        }

        var output = result.Adapt<List<UserOutput>>();

        return [.. output];
    }

    public async Task SendEmail(Schedule schedule, bool isCreate)
    {
        if (schedule is null)
        {
            throw new ArgumentException($"O agendamento ({nameof(schedule)}) não foi fornecido. Não é possível enviar os e-mails sem os dados do agendamento.");
        }

        if (schedule.Client is null)
        {
            Client? clientSearch = await _context.Clients.Where(x => x.ClientId == schedule.ClientId).AsNoTracking().FirstOrDefaultAsync();

            if (clientSearch is null)
            {
                return;
            }

            schedule.Client = clientSearch;
        }

        if (schedule.Company is null)
        {
            Company? companySearch = await _context.Companies.Where(x => x.CompanyId == schedule.CompanyId).AsNoTracking().FirstOrDefaultAsync();

            if (companySearch is null)
            {
                return;
            }

            schedule.Company = companySearch;
        }

        string date = GetDateDetails(schedule.DateStart);
        string client = schedule.Client?.FullName ?? string.Empty;

        List<string> membersNames = [];
        List<string> membersEmails = [];

        if (schedule.UsersIds is not null && schedule.UsersIds?.Length != 0)
        {
            List<User> members = await _context.Users.Where(x => schedule.UsersIds!.Contains(x.UserId)).AsNoTracking().ToListAsync();
            membersNames = [.. members.Select(x => x.FullName).Distinct()];
            membersEmails = [.. members.Select(x => x.Email).Distinct()];
        }

        Dictionary<string, string> values = new()
        {
            { "[NameApp]", SystemConsts.NameApp },
            { "[ClientName]", client},
            { "[ProfessionalName]", membersNames.Count != 0 ? string.Join(", ", membersNames) : "Profissional específico não definido" },
            { "[ScheduleDateTime]",date },
            { "[LocationAddress]", schedule.Company?.Address ?? "Local não informado" },
            { "[ScheduleUrl]", schedule.CustomUrl ?? string.Empty },
            { "[ScheduleDateCreated]", GetDateDetails(schedule.CreatedDate) },
            { "[Value]", $"R$ {schedule.AmountReceived?.ToString("0.##")}" ?? string.Empty },
            { "[PaymentType]", schedule.PaymentType > 0 ? GetEnumDesc(schedule.PaymentType) : string.Empty },
            { "[Observations]", schedule.Observation ?? string.Empty }
        };

        string title = isCreate ? $"Novo agendamento — {date} — {client}" : $"Atualização de agendamento — {date} — {client}";

        if (!string.IsNullOrEmpty(schedule.CustomTitle))
        {
            title = isCreate ? $"{schedule.CustomTitle} — {date} — {client}" : $"Atualização — {schedule.CustomTitle} — {date} — {client}";
        }

        string bodyHtml = _emailService.RenderTemplate("EmailSchedule.html", values);
        await _emailService.SendEmail(to: schedule.Company!.Email, subject: title, body: bodyHtml, cc: membersEmails);
    }

    #region extras
    private static string GetStatusDesc(ScheduleStatusEnum status)
    {
        return GetEnumDesc(status).ToLowerInvariant();
    }

    private async Task ValidateUsersAndRemoveNotLinkedOnes(ScheduleInput input)
    {
        Guid[]? usersIds = input.UsersIds;

        if (usersIds is null || usersIds.Length == 0)
        {
            return;
        }

        List<Guid> validUsers = [];

        foreach (var item in usersIds)
        {
            bool existAndIsLinkedToCompany = await _context.CompanyUsers.
                                             AsNoTracking().
                                             AnyAsync(x => x.CompanyId == input.CompanyId && x.UserId == item && x.Status == true);

            if (existAndIsLinkedToCompany)
            {
                validUsers.Add(item);
            }
        }

        if (validUsers is null || validUsers.Count == 0)
        {
            string msg = usersIds.Length == 1 ?
                "O usuário que você referenciou para o agendamento não é válido ou não está vinculado à empresa." :
                $"Os {usersIds.Length} usuários que você referenciou para o agendamento não são válidos ou não estão vinculados à empresa.";

            throw new UnauthorizedAccessException(msg);
        }

        input.UsersIds = [.. validUsers];
    }

    private static bool IsCustomUrlValid(string? url)
    {
        if (string.IsNullOrWhiteSpace(url))
        {
            return true;
        }

        return RegexCustomUrl().IsMatch(url);
    }
    #endregion
}