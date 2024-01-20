using FastEndpoints;
using FastEndpoints.Swagger;
using Wsa.Gaas.Werewolf.Application.Common;
using Wsa.Gaas.Werewolf.Application.Options;
using Wsa.Gaas.Werewolf.InMemory;

namespace Wsa.Gaas.Werewolf.WebApi;
public static class DependencyInjection
{
    public static IServiceCollection AddWerewolfWebApi(this IServiceCollection services, IConfiguration configuration)
    {
        services
            .AddExceptionHandler<GlobalExceptionHandler>()
            .AddScoped<IGameEventHandler, GameEventHubHandler>()
            .Configure<GameSettingOptions>(
                opt => configuration.Bind(nameof(GameSettingOptions), opt)
            )
            .AddFastEndpoints()
            .SwaggerDocument(opt => { })
            .AddSignalR()
            ;

        return services;
    }

    public static IServiceCollection AddWerewolfInfrastructure(this IServiceCollection services)
    {
        return services.AddInMemoryRepository();
    }
}
