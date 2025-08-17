using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Orquestra.Domain.Entities;
using System.Security.Claims;
using static Orquestra.Utils.Fixtures.Get;

namespace Orquestra.Infrastructure.Data;

public class Context(DbContextOptions<Context> options, IHttpContextAccessor httpContextAccessor) : DbContext(options)
{
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

    public DbSet<Company> Companies { get; set; }
    public DbSet<CompanyUser> CompanyUsers { get; set; }
    public DbSet<LocationCity> LocationCities { get; set; }
    public DbSet<LocationState> LocationStates { get; set; }
    public DbSet<Log> Logs { get; set; }
    public DbSet<RefreshToken> RefreshTokens { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<Client> Clients { get; set; }
    public DbSet<Schedule> Schedules { get; set; }

    #region extras
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        #region delete_behavior
        foreach (var relationship in modelBuilder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
        {
            relationship.DeleteBehavior = DeleteBehavior.Cascade;
        }
        #endregion

        #region postgreSQL_datetime_normalize_utc
        var utcConverter = new ValueConverter<DateTime, DateTime>(
           v => v.Kind == DateTimeKind.Utc ? v : v.ToUniversalTime(), // Salva como UTC;
           v => DateTime.SpecifyKind(v, DateTimeKind.Utc)             // Lê como UTC;
        );

        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            foreach (var property in entityType.GetProperties())
            {
                if (property.ClrType == typeof(DateTime))
                {
                    property.SetValueConverter(utcConverter);
                }
                else if (property.ClrType == typeof(DateTime?))
                {
                    property.SetValueConverter(
                        new ValueConverter<DateTime?, DateTime?>(
                            v => v.HasValue ? (v.Value.Kind == DateTimeKind.Utc ? v.Value : v.Value.ToUniversalTime()) : v,
                            v => v.HasValue ? DateTime.SpecifyKind(v.Value, DateTimeKind.Utc) : v
                        )
                    );
                }
            }
        }
        #endregion
    }

    public Guid CurrentUserId
    {
        get
        {
            var user = _httpContextAccessor.HttpContext?.User;

            if (user?.Identity?.IsAuthenticated ?? false)
            {
                string? userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (Guid.TryParse(userIdClaim, out Guid userId))
                {
                    return userId;
                }
            }

            return Guid.Empty;
        }
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        foreach (var entry in ChangeTracker.Entries<Audit>())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    if (entry.Entity.CreatedDate is null)
                    {
                        entry.Entity.CreatedDate = GetDate();
                        entry.Entity.CreatedBy = CurrentUserId;
                        entry.Entity.Status = true;
                    }

                    break;

                case EntityState.Modified:
                    entry.Entity.LastModificationDate = GetDate();
                    entry.Entity.LastModificationBy = CurrentUserId;

                    break;
            }
        }

        return base.SaveChangesAsync(cancellationToken);
    }
    #endregion
}