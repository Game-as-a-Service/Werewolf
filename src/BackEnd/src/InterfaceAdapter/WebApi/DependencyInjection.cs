using FastEndpoints;
using Wsa.Gaas.Werewolf.Application;
using Wsa.Gaas.Werewolf.Application.Common;
using Wsa.Gaas.Werewolf.EntityFrameworkCore;

namespace Wsa.Gaas.Werewolf.WebApi
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddWebApi(this IServiceCollection services)
        {
            services.AddWerewolfApplication()
                    .AddFastEndpoints()
                    .AddRepository()
                    .AddScoped<IGameEventHandler, GameEventHubHandler>()
                    .AddSingleton<ITaskService, TaskService>()
                    .AddSignalR();

            return services;
        }
    }
}