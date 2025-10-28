using Microsoft.AspNetCore.Mvc.Rendering;
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

        var query = _context.Logs.
                    AsNoTracking().
                    Where(x => x.Parameters!.Contains($"\"CompanyId\":\"{currentMainCompany.CompanyId}\"") || x.UserId == userIdAuth).
                    OrderByDescending(x => x.CreatedDate);

        (IEnumerable<Log> linq, int count) = await PagedQuery.Execute(query, pagination);

        if (count < 1)
        {
            return ([], 0);
        }

        List<User> users = await _context.Users.AsNoTracking().ToListAsync();

        List<LogNotificationOutput> output = [.. linq.Select(log =>
        {
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

            string story = $"{requestTypeNormalized} de {endpointName.ToLowerInvariant()}";

            if (!string.IsNullOrEmpty(log.Parameters) && users.Count != 0)
            {
                Guid? userId = GetPropertyValueFromStringJson<Guid>(log.Parameters, "UserId");
                
                if (userId is not null && userId != Guid.Empty)
                {
                    User? user = users.FirstOrDefault(x => x.UserId == userId);

                    if (user is not null)
                    {
                        story += $" ({user.FullName})";
                    }
                }
            }             

            string? description = log.Description;
            string marker = "Mais informações:";
            int index = log.Description?.IndexOf(marker) ?? -1;

            if (index != -1)
            {
                description = log.Description?[(index + marker.Length)..].Trim();
            }  

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
}