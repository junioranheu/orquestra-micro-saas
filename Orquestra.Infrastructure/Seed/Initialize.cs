using Microsoft.EntityFrameworkCore;
using Orquestra.Infrastructure.Data;
using Orquestra.Infrastructure.Seed.Seeds;
using static Orquestra.Utils.Fixtures.Get;

namespace Orquestra.Infrastructure.Seed;

public static class DbInitializer
{
    public static async Task Initialize(Context context, bool isDev, bool isApplyReset, bool isApplyMigrations, bool isApplySeed)
    {
        if (!isDev)
        {
            return;
        }

        context.Database.SetCommandTimeout(600);
        // string script = context.Database.GenerateCreateScript();

        if (isApplyReset)
        {
            await context.Database.ExecuteSqlRawAsync(
                "DO $$ DECLARE r RECORD; BEGIN " +
                "FOR r IN (SELECT tablename FROM pg_tables WHERE schemaname = 'public') LOOP " +
                "EXECUTE 'TRUNCATE TABLE ' || quote_ident(r.tablename) || ' CASCADE'; " +
                "END LOOP; END $$;"
            );

            await context.Database.EnsureCreatedAsync();
        }

        if (isApplyMigrations && !isApplyReset)
        {
            await context.Database.MigrateAsync();
        }

        if (isApplySeed)
        {
            await Seed(context, GetDate());
        }
    }

    public static async Task Seed(Context context, DateTime _)
    {
        await SeedLocationStates.Seed(context);
        await SeedLocationCities.Seed(context);
        await SeedUsers.Seed(context);

        await context.SaveChangesAsync();
    }
}