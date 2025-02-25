using Microsoft.EntityFrameworkCore;
using Orquestra.Infrastructure.Data;
using Orquestra.Infrastructure.Seed.Seeds;
using static Orquestra.Utils.Fixtures.Get;

namespace Orquestra.Infrastructure.Seed;

public static class DbInitializer
{
    public static async Task Initialize(Context context, bool isApplyReset, bool isApplyMigrations, bool isApplySeed)
    {
        context.Database.SetCommandTimeout(600);
        // string script = context.Database.GenerateCreateScript();

        if (isApplyReset)
        {
            await context.Database.EnsureDeletedAsync();
            await context.Database.EnsureCreatedAsync();
        }

        if (isApplyMigrations)
        {
            await context.Database.MigrateAsync();
        }

        if (isApplySeed)
        {
            await Seed(context, GetDate());
        }
    }

    public static async Task Seed(Context context, DateTime date)
    {
        await SeedLocationStates.Seed(context);
        await SeedLocationCities.Seed(context);

        await context.SaveChangesAsync();
    }
}