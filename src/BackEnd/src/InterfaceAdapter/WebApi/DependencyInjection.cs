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
            // Application
            services.AddWerewolfApplication();

            // SignalR
            services.AddSignalR();

            // Web Api
            services.AddFastEndpoints();

            // 實作 IRepository
            services.AddEntityFrameworkCoreRepository();

            // 實作 IGameEventHandler
            services.AddScoped<IGameEventHandler, GameEventHub>();

            return services;
        }
    }
}
