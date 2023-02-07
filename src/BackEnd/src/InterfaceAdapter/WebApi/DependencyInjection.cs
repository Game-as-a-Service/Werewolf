using FastEndpoints;
using Was.Gaas.Werewolf.Application;
using Was.Gaas.Werewolf.Application.Common;
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

            // 實作 IEventPublisher
            services.AddTransient<IEventPublisher, GameEventHub>();

            return services;
        }
    }
}
