using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Wsa.Gaas.Werewolf.Application.Common;

namespace Wsa.Gaas.Werewolf.EntityFrameworkCore
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddEntityFrameworkCoreRepository(this IServiceCollection services)
        {
            services.AddDbContext<IRepository, EntityFrameworkCoreRepository>(opt =>
            {
                opt.UseInMemoryDatabase(nameof(EntityFrameworkCoreRepository));
            });

            return services;
        }
    }
}
