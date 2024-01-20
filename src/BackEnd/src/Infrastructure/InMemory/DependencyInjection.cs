using Microsoft.Extensions.DependencyInjection;
using Wsa.Gaas.Werewolf.Application.Common;

namespace Wsa.Gaas.Werewolf.InMemory;
public static class DependencyInjection
{
    public static IServiceCollection AddInMemoryRepository(this IServiceCollection services)
    {
        services.AddSingleton<IRepository, InMemoryRepository>();

        return services;
    }
}
