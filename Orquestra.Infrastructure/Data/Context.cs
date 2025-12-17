using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Orquestra.Domain.Entities;
using System.Security.Claims;
using static Orquestra.Utils.Fixtures.Get;

namespace Orquestra.Infrastructure.Data;

public class Context(DbContextOptions<Context> options, IHttpContextAccessor httpContextAccessor) : DbContext(options)
{
    public DbSet<Company> Companies { get; set; }
    public DbSet<CompanyUser> CompanyUsers { get; set; }
    public DbSet<LocationCity> LocationCities { get; set; }
    public DbSet<LocationState> LocationStates { get; set; }
    public DbSet<Log> Logs { get; set; }
    public DbSet<RefreshToken> RefreshTokens { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<Client> Clients { get; set; }
    public DbSet<ClientFollowUp> ClientsFollowUps { get; set; }
    public DbSet<Schedule> Schedules { get; set; }
    public DbSet<Verification> Verifications { get; set; }
    public DbSet<CompanyInvoice> CompanyInvoices { get; set; }
    public DbSet<IntegrationWhatsApp> IntegrationsWhatsApp { get; set; }
    public DbSet<Inventory> Inventories { get; set; }
    public DbSet<Quote> Quotes { get; set; }
    public DbSet<QuoteItem> QuoteItems { get; set; }
    public DbSet<ServiceOrder> ServiceOrders { get; set; }

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
           x => x.Kind == DateTimeKind.Utc ? x : x.ToUniversalTime(), // Salva como UTC;
           x => DateTime.SpecifyKind(x, DateTimeKind.Utc)             // Lê como UTC;
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
                            x => x.HasValue ? (x.Value.Kind == DateTimeKind.Utc ? x.Value : x.Value.ToUniversalTime()) : x,
                            x => x.HasValue ? DateTime.SpecifyKind(x.Value, DateTimeKind.Utc) : x
                        )
                    );
                }
            }
        }
        #endregion
    }

    private Guid UserIdAuth
    {
        get
        {
            ClaimsPrincipal? user = httpContextAccessor.HttpContext?.User;

            if (user?.Identity?.IsAuthenticated ?? false)
            {
                string? userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (Guid.TryParse(userIdClaim, out Guid userIdAuth))
                {
                    return userIdAuth;
                }
            }

            return Guid.Empty;
        }
    }

    private void ApplyAuditLogRules()
    {
        foreach (var entry in ChangeTracker.Entries<Audit>())
        {
            if (entry.Entity is Audit audit)
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        if (audit.CreatedDate is null)
                        {
                            audit.CreatedDate = GetDate();
                            audit.CreatedBy = UserIdAuth;
                            audit.Status = true;
                        }

                        break;

                    case EntityState.Modified:
                        audit.LastModificationDate = GetDate();
                        audit.LastModificationBy = UserIdAuth;

                        break;
                }
            }
        }
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        ApplyAuditLogRules();
        return base.SaveChangesAsync(cancellationToken);
    }

    public override int SaveChanges()
    {
        ApplyAuditLogRules();
        return base.SaveChanges();
    }
    #endregion
}