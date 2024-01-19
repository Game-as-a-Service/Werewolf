using FastEndpoints;
using FastEndpoints.Swagger;
using Wsa.Gaas.Werewolf.Application.Common;
using Wsa.Gaas.Werewolf.Application.Options;

namespace Wsa.Gaas.Werewolf.WebApi;
public static class DependencyInjection
{
    public static IServiceCollection AddWebApi(this IServiceCollection services, IConfiguration configuration)
    {
        services
            .AddScoped<IGameEventHandler, GameEventHubHandler>()
            .Configure<GameSettingOptions>(
                opt => configuration.Bind(nameof(GameSettingOptions), opt)
            )
            .AddFastEndpoints()
            .SwaggerDocument(
                opt => { }
            )
            .AddSignalR()
            ;

        return services;
    }
}
