using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
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

    #region extras
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        foreach (var relationship in modelBuilder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
        {
            relationship.DeleteBehavior = DeleteBehavior.Cascade;
        }
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