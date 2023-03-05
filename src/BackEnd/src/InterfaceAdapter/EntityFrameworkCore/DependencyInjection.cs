using Microsoft.Extensions.DependencyInjection;
using Wsa.Gaas.Werewolf.Application.Common;

namespace Wsa.Gaas.Werewolf.EntityFrameworkCore
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddRepository(this IServiceCollection services)
        {
            //services.AddDbContext<IRepository, EntityFrameworkCoreRepository>(opt =>
            //{
            //    opt.UseInMemoryDatabase(nameof(EntityFrameworkCoreRepository));
            //});

            services.AddSingleton<IIdGenerator, SimpleIdGenerator>();
            services.AddSingleton<IRepository, InMemoryRepository>();

            return services;
        }
    }
}
