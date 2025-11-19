using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Orquestra.Domain.Entities;
using Orquestra.Domain.Enums;
using System.Security.Claims;
using System.Text.Json;

namespace Orquestra.Infrastructure.Interceptors;

public sealed class ChangeLogInterceptor(IHttpContextAccessor httpContextAccessor) : SaveChangesInterceptor
{
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
    private static readonly string[] KEYS_TO_HIDE = ["senha", "password", "token"];

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
    {
        DbContext? context = eventData.Context;

        if (context is null)
        {
            return new ValueTask<InterceptionResult<int>>(result);
        }

        List<EntityEntry> entries = [.. context.ChangeTracker.Entries()];

        foreach (var entry in entries)
        {
            if (entry.State == EntityState.Detached || entry.State == EntityState.Unchanged)
            {
                continue;
            }

            Dictionary<string, object?>? before = null;
            Dictionary<string, object?> after = [];

            if (entry.State == EntityState.Modified)
            {
                before = [];

                foreach (var prop in entry.Properties)
                {
                    if (!prop.IsModified)
                    {
                        continue;
                    }

                    if (KEYS_TO_HIDE.Any(k => prop.Metadata.Name.Contains(k, StringComparison.OrdinalIgnoreCase)))
                    {
                        continue;
                    }

                    before[prop.Metadata.Name] = prop.OriginalValue;
                    after[prop.Metadata.Name] = prop.CurrentValue;
                }
            }
            else if (entry.State == EntityState.Added)
            {
                foreach (var prop in entry.Properties)
                {
                    if (KEYS_TO_HIDE.Any(k => prop.Metadata.Name.Contains(k, StringComparison.OrdinalIgnoreCase)))
                    {
                        continue;
                    }

                    after[prop.Metadata.Name] = prop.CurrentValue;
                }
            }
            else if (entry.State == EntityState.Deleted)
            {
                before = [];

                foreach (var prop in entry.Properties)
                {
                    if (KEYS_TO_HIDE.Any(k => prop.Metadata.Name.Contains(k, StringComparison.OrdinalIgnoreCase)))
                    {
                        continue;
                    }

                    before[prop.Metadata.Name] = prop.OriginalValue;
                }
            }

            #region validations
            // #1 - Se o entry.Entity.GetType().Name for Logs, não salva;
            string entity = entry.Entity.GetType().Name;

            if (entity == "Log")
            {
                return new ValueTask<InterceptionResult<int>>(result);
            }

            // #2 - Se não for status 200, não salva;
            bool hasNon200Status = false;

            if (before is not null && before.TryGetValue("Status", out object? beforeStatus) && beforeStatus is int bStatus && bStatus != 200)
            {
                hasNon200Status = true;
            }

            if (after is not null && after.TryGetValue("Status", out object? afterStatus) && afterStatus is int aStatus && aStatus != 200)
            {
                hasNon200Status = true;
            }

            if (hasNon200Status)
            {
                return new ValueTask<InterceptionResult<int>>(result);
            }
            #endregion

            Log log = new()
            {
                LogType = LogTypeEnum.Audit,
                RequestType = entry.State switch
                {
                    EntityState.Added => "POST",
                    EntityState.Modified => "PUT",
                    EntityState.Deleted => "DELETE",
                    _ => "UNKNOWN"
                },
                Endpoint = entity,
                Parameters = JsonSerializer.Serialize(new { Before = before, After = after }),
                Status = StatusCodes.Status200OK,
                UserId = GetUserId(_httpContextAccessor)
            };

            context.Add(log);
        }

        return new ValueTask<InterceptionResult<int>>(result);
    }

    #region extras
    private static Guid? GetUserId(IHttpContextAccessor httpContextAccessor)
    {
        string? userIdString = httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
        return Guid.TryParse(userIdString, out Guid guid) ? guid : null;
    }
    #endregion
}