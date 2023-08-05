using Microsoft.Extensions.DependencyInjection;
using Wsa.Gaas.Werewolf.Application.Common;

namespace Wsa.Gaas.Werewolf.EntityFrameworkCore;

public static class DependencyInjection
{
    public static IServiceCollection AddEntityFrameworkCoreRepository(this IServiceCollection services)
    {
        //services.AddDbContext<IRepository, EntityFrameworkCoreRepository>(opt =>
        //{
        //    opt.UseInMemoryDatabase(nameof(EntityFrameworkCoreRepository));
        //    //opt.UseSqlServer("Server=(localdb)\\MSSQLLocalDB;Integrated Security=true;Initial Catalog=TestDb");

        //    opt.EnableDetailedErrors();
        //    opt.EnableSensitiveDataLogging();
        //});

        services.AddSingleton<IRepository, InMemoryRepository>();

        return services;
    }
}
