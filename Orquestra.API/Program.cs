using Orquestra.API;
using Orquestra.Application;
using Orquestra.Infrastructure;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
{
    builder.Services.AddDependencyInjectionAPI();
    builder.Services.AddDependencyInjectionApplication(builder);
    builder.Services.AddDependencyInjectionInfrastructure(builder);
}

AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true); // Normalizar DateTime;

WebApplication app = builder.Build();
{
    await app.UseAppConfiguration(builder);
    app.Run();
}
