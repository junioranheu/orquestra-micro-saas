using Orquestra.API;
using Orquestra.Application;
using Orquestra.Domain.Consts;
using Orquestra.Infrastructure;

Console.Title = SystemConsts.App.NameApi;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
{
    builder.Services.AddDependencyInjectionAPI(builder);
    builder.Services.AddDependencyInjectionApplication(builder);
    builder.Services.AddDependencyInjectionInfrastructure(builder);
}

WebApplication app = builder.Build();
{
    await app.UseAppConfiguration(builder);
    app.Run();
}