using Microsoft.EntityFrameworkCore;
using Orquestra.Application.UseCases.Companies.Shared;
using Orquestra.Application.UseCases.CompanyUsers.GetCurrentMain;
using Orquestra.Application.UseCases.Logs.Shared;
using Orquestra.Application.UseCases.Shared;
using Orquestra.Domain.Entities;
using Orquestra.Domain.Enums;
using Orquestra.Infrastructure.Data;
using static Orquestra.Utils.Fixtures.Get;

namespace Orquestra.Application.UseCases.Logs.GetNotification;

public sealed class GetNotificationLog(Context context, IGetCurrentMainCompanyUser getCurrentMainCompanyUser) : IGetNotificationLog
{
    private readonly Context _context = context;
    private readonly IGetCurrentMainCompanyUser _getCurrentMainCompanyUser = getCurrentMainCompanyUser;

    public async Task<(List<LogNotificationOutput> output, int count)> Execute(PaginationInput pagination, Guid userIdAuth)
    {
        (CompanyOutput? currentMainCompany, bool isUserAdm) = await _getCurrentMainCompanyUser.Execute(userId: userIdAuth);

        if (currentMainCompany is null)
        {
            throw new InvalidOperationException("No momento, você não faz parte de nenhuma empresa ou não definiu nenhuma como sua principal, portanto não é possível gerar nenhuma notificação.");
        }

        (IEnumerable<Log> linq, int count) = await GetLogs(pagination, userIdAuth, currentMainCompany);

        if (count < 1)
        {
            return ([], 0);
        }

        (List<User> users, List<Client> clients) = await GetExtraDataFromDb(userIdAuth);

        List<LogNotificationOutput> output = [.. linq.Select(log =>
        {
            #region normalize_general_data
            string emoji = log.LogType switch
            {
                LogTypeEnum.Exception => "🔴",
                LogTypeEnum.Request => "🟢",
                LogTypeEnum.Job => "🟡",
                _ => "⚪"
            };

            string logTypeNormalized = log.LogType switch
            {
                LogTypeEnum.Exception => "Falha",
                LogTypeEnum.Request => "Consulta",
                _ => string.Empty
            };

            if (string.IsNullOrEmpty(logTypeNormalized))
            {
                return null;
            }

            string? requestTypeNormalized = log.RequestType?.ToUpperInvariant() switch
            {
                "GET" => "Consulta",
                "POST" => "Registro",
                "PUT" => "Atualização",
                "DELETE" => "Remoção",
                _ => string.Empty
            };

            if (string.IsNullOrEmpty(requestTypeNormalized))
            {
                return null;
            }

            string? endpointName = log.Endpoint switch
            {
                string x when x.Contains("/Client") => "Cliente",
                string x when x.Contains("/CompanyInvoice") => "Fatura",
                string x when x.Contains("/CompanyUser") => "Colaborador",
                string x when x.Contains("/Schedule") => "Agendamento",
                string x when x.Contains("/User") => "Usuário",
                string x when x.Contains("/Company") => "Empresa",
                _ => string.Empty
            };

            if (string.IsNullOrEmpty(endpointName))
            {
                return null;
            }
            #endregion

            #region normalize_description
            string? description = log.Description;
            string marker = "Mais informações:";
            int index = log.Description?.IndexOf(marker) ?? -1;

            if (index != -1)
            {
                description = log.Description?[(index + marker.Length)..].Trim();
            }
            #endregion

            #region normalize_story
            string story = $"{requestTypeNormalized} de {endpointName.ToLowerInvariant()}";
            bool found = false;

            if (!string.IsNullOrEmpty(log.Parameters))
            {
                Guid? userId = GetPropertyValueFromStringJson<Guid>(log.Parameters, "UserId");

                if (!found && userId is not null && userId != Guid.Empty)
                {
                    found = true;
                    User? user = users.FirstOrDefault(x => x.UserId == userId);

                    if (user is not null)
                    {
                        // story += $" ({user.FullName})";
                        description = user.FullName;
                    }
                }

                Guid? clientId = GetPropertyValueFromStringJson<Guid>(log.Parameters, "ClientId");

                if (!found && clientId is not null && clientId != Guid.Empty)
                {
                    found = true;
                    Client? client = clients.FirstOrDefault(x => x.ClientId == clientId);

                    if (client is not null)
                    {
                        // story += $" ({client.FullName})";
                        description = client.FullName;
                    }
                }

                string? email = GetPropertyValueFromStringJson<string>(log.Parameters, "Email");

                if (!found && !string.IsNullOrEmpty(email))
                {
                    found = true;
                    // story += $" ({email})";
                    description = email;
                }
            }
            #endregion

            return new LogNotificationOutput
            {
                LogId = log.LogId,
                Emoji = emoji,
                LogType = logTypeNormalized,
                RequestType = requestTypeNormalized,
                EndpointName = endpointName,
                RawEndpoint = log.Endpoint,
                Description = RemoveHtmlTags(description),
                Story = story,
                Date = log.CreatedDate
            };
        }).Where(x => x is not null).Select(x => x)];

        return (output, count);
    }

    #region extras
    private async Task<(IEnumerable<Log> linq, int count)> GetLogs(PaginationInput pagination, Guid userIdAuth, CompanyOutput currentMainCompany)
    {
        var query = _context.Logs.
                    AsNoTracking().
                    Where(x => x.Parameters!.Contains($"\"CompanyId\":\"{currentMainCompany.CompanyId}\"") || x.UserId == userIdAuth).
                    OrderByDescending(x => x.CreatedDate);

        (IEnumerable<Log> linq, int count) = await PagedQuery.Execute(query, pagination);

        return (linq, count);
    }

    private async Task<(List<User> users, List<Client> clients)> GetExtraDataFromDb(Guid userIdAuth)
    {
        List<User> users = await _context.Users.AsNoTracking().ToListAsync();
        List<Client> clients = await (from c in _context.Clients.AsNoTracking() join cu in _context.CompanyUsers.AsNoTracking() on c.CompanyId equals cu.CompanyId where cu.UserId == userIdAuth select c).ToListAsync();

        return (users, clients);
    }
    #endregion
}