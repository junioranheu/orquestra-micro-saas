using Orquestra.API;
using Orquestra.Application;
using Orquestra.Infrastructure;
using Orquestra.Infrastructure.Data;
using Orquestra.Infrastructure.Seed;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
{
    builder.Services.AddDependencyInjectionAPI();
    builder.Services.AddDependencyInjectionApplication(builder);
    builder.Services.AddDependencyInjectionInfrastructure(builder);
}

WebApplication app = builder.Build();
{
    app.UseAppConfiguration(builder);
    await HandleDbInitialize(app);

    app.Run();
}

static async Task HandleDbInitialize(WebApplication app)
{
    using IServiceScope scope = app.Services.CreateScope();
    IServiceProvider services = scope.ServiceProvider;
    Context context = services.GetRequiredService<Context>();

    bool isApplyMigrations = false;
    bool isApplyReset = false;
    bool isApplySeed = false;

    if (!isApplyMigrations && !isApplyReset && !isApplySeed)
    {
        return;
    }

    await DbInitializer.Initialize(context, isApplyMigrations, isApplyReset, isApplySeed);
}