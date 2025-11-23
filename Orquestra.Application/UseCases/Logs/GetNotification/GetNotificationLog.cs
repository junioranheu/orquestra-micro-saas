using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Orquestra.Application.UseCases.Companies.Shared;
using Orquestra.Application.UseCases.CompanyUsers.GetCurrentMain;
using Orquestra.Application.UseCases.Logs.Shared;
using Orquestra.Application.UseCases.Shared;
using Orquestra.Domain.Consts;
using Orquestra.Domain.Entities;
using Orquestra.Domain.Enums;
using Orquestra.Infrastructure.Data;
using static Orquestra.Utils.Fixtures.Get;

namespace Orquestra.Application.UseCases.Logs.GetNotification;

public sealed class GetNotificationLog(Context context, IMemoryCache cache, IGetCurrentMainCompanyUser getCurrentMainCompanyUser) : IGetNotificationLog
{
    private readonly Context _context = context;
    private readonly IMemoryCache _cache = cache;
    private readonly IGetCurrentMainCompanyUser _getCurrentMainCompanyUser = getCurrentMainCompanyUser;
    private const int CACHE_TIMESPAN = 10;

    public async Task<(List<LogNotificationOutput> output, int count)> Execute(PaginationInput pagination, Guid userIdAuth, bool isDashboard = false)
    {
        (CompanyOutput? currentMainCompany, bool isUserAdm) = await _getCurrentMainCompanyUser.GetCurrentMainCompany(userId: userIdAuth);

        if (currentMainCompany is null)
        {
            throw new InvalidOperationException(SystemConsts.Warnings.NotLinkedOrDontHaveCompany);
        }

        Guid companyId = currentMainCompany.CompanyId;
        string cacheKey = $"key_get_notification_log_isDashboard_{userIdAuth}_{companyId}";

        if (isDashboard)
        {
            // Cache;
            if (_cache.TryGetValue(cacheKey, out (List<LogNotificationOutput> output, int count) cached))
            {
                return cached;
            }
        }

        var (endpointMap, linq, count) = await GetLogs(pagination, companyId, isDashboard);

        if (count < 1)
        {
            return ([], 0);
        }

        List<User>? users = await GetExtraDataFromDb();

        List<LogNotificationOutput> output = [.. linq.Select(log =>
        {
            #region normalize_general_data
            string emoji = log.LogType switch
            {
                LogTypeEnum.Exception => "🔴",
                LogTypeEnum.Request => "🟢",
                LogTypeEnum.Job => "🟡",
                LogTypeEnum.Audit => "🟢",
                _ => "⚪"
            };

            string logTypeNormalized = log.LogType switch
            {
                LogTypeEnum.Exception => "Erro",
                LogTypeEnum.Request => "Sucesso",
                LogTypeEnum.Audit => "Auditoria",
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

            string endpointName = endpointMap.Where(kvp => log.Endpoint!.Contains(kvp.Key)).Select(kvp => kvp.Value).FirstOrDefault() ?? string.Empty;

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
                // Schedule;
                if (log.Endpoint!.Contains("/Schedule"))
                {
                    DateTime? dateStart = GetPropertyValueFromStringJson<DateTime>(log.Parameters, nameof(Schedule.DateStart));

                    if (!found && dateStart is not null && dateStart != DateTime.MinValue)
                    {
                        found = true;
                        string? customTitle = GetPropertyValueFromStringJson<string>(log.Parameters, nameof(Schedule.CustomTitle));
                        description = !string.IsNullOrEmpty(customTitle) ? customTitle : GetDateDetails(dateStart);
                    }
                }

                // Client;
                if (log.Endpoint!.Contains("/Client"))
                {
                    string? fullName = GetPropertyValueFromStringJson<string>(log.Parameters, nameof(Client.FullName));

                    if (!found && !string.IsNullOrEmpty(fullName))
                    {
                        found = true;
                        description = fullName;
                    }
                }
                
                // UserId;
                Guid? userId = GetPropertyValueFromStringJson<Guid>(log.Parameters, nameof(User.UserId));

                if (!found && userId is not null && userId != Guid.Empty)
                {
                    found = true;
                    User? user = users?.FirstOrDefault(x => x.UserId == userId);

                    if (user is not null)
                    {
                        description = user.FullName;
                    }
                }

                // E-mail;
                string? email = GetPropertyValueFromStringJson<string>(log.Parameters, nameof(User.Email));

                if (!found && !string.IsNullOrEmpty(email))
                {
                    found = true;
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
                Description = log.LogType == LogTypeEnum.Audit && !isDashboard ? NormalizeJsonLog(RemoveHtmlTags(log.Parameters)) : RemoveHtmlTags(description),
                Story = story,
                Date = log.CreatedDate,
                ChangedFields = log.LogType == LogTypeEnum.Audit ? GetChangedFieldsFromBeforeAndAfter(log.Parameters) : string.Empty
            };
        }).Where(x => x is not null).Select(x => x!)];

        if (isDashboard)
        {
            var result = (output, count);

            // Salva no cache;
            _cache.Set(cacheKey, result, TimeSpan.FromMinutes(CACHE_TIMESPAN));
        }

        return (output, count);
    }

    #region extras
    private async Task<(Dictionary<string, string> endpointMap, IEnumerable<Log> linq, int count)> GetLogs(PaginationInput pagination, Guid companyId, bool isDashboard)
    {
        Dictionary<string, string> endpointMap = new()
        {
            { "Client", "Cliente" },
            { "CompanyInvoice", "Fatura" },
            { "CompanyUser", "Colaborador" },
            { "Schedule", "Agendamento" },
            { "User", "Usuário" },
            { "Company", "Empresa" }
        };

        bool isXUnit = IsRunningFromXUnit();

        var query = _context.Logs.
                    AsNoTracking().
                    Where(x =>
                        // (x.Parameters!.Contains($"\"CompanyId\":\"{companyId}\"") || x.UserId == userIdAuth) &&
                        (x.Parameters!.Contains($"\"CompanyId\":\"{companyId}\"")) &&
                        (isXUnit || endpointMap.Keys.Any(k => x.Endpoint!.Contains(k)))
                    ).OrderByDescending(x => x.CreatedDate);

        if (isDashboard)
        {
            pagination = new() { Index = 0, Limit = 5, IsSelectAll = false };
        }

        (IEnumerable<Log> linq, int count) = await PagedQuery.Execute(query, pagination);

        return (endpointMap, linq, count);
    }

    private async Task<List<User>?> GetExtraDataFromDb()
    {
        // Key global para todos os usuários;
        string usersCacheKey = "key_get_notification_log_AllUsers";

        // Tenta pegar usuários do cache global
        if (!_cache.TryGetValue(usersCacheKey, out List<User>? users))
        {
            users = await _context.Users.AsNoTracking().ToListAsync();
            _cache.Set(usersCacheKey, users, TimeSpan.FromMinutes(CACHE_TIMESPAN));
        }

        return users;
    }
    #endregion
}