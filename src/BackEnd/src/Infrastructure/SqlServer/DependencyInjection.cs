using Microsoft.Extensions.DependencyInjection;
using Wsa.Gaas.Werewolf.Application.Common;

namespace Wsa.Gaas.Werewolf.SqlServer;
public static class DependencyInjection
{
    public static IServiceCollection AddSqlServer(this IServiceCollection services)
    {
        //services.AddDbContext<IRepository, SqlServerRepository>(opt =>
        //{
        //    opt.UseSqlServer("Server=(localdb)\\MSSQLLocalDB;Integrated Security=true;Initial Catalog=TestDb");

        //    opt.EnableDetailedErrors();
        //    opt.EnableSensitiveDataLogging();
        //});

        return services;
    }
}
