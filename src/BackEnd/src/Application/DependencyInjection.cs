using Microsoft.Extensions.DependencyInjection;
using Wsa.Gaas.Werewolf.Application.Common;

namespace Wsa.Gaas.Werewolf.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddWerewolfApplication(this IServiceCollection services)
        {
            return services.AddUseCases()
                    .AddPolicies()
                    .AddScoped<GameEventBus>();
        }

        private static IServiceCollection AddUseCases(this IServiceCollection services)
        {
            var assembly = typeof(DependencyInjection).Assembly;
            var types = assembly.GetTypes();
            var useCaseType = typeof(UseCase<,>);

            foreach (var type in types)
            {
                if (type.BaseType?.IsGenericType == true && type.BaseType?.GetGenericTypeDefinition() == useCaseType)
                {
                    services.AddScoped(type.BaseType, type);
                }
            }

            return services;
        }

        private static IServiceCollection AddPolicies(this IServiceCollection services)
        {
            var assembly = typeof(DependencyInjection).Assembly;
            var types = assembly.GetTypes();
            var policyType = typeof(Policy<>);

            foreach (var type in types)
            {
                if (type.BaseType?.IsGenericType == true && type.BaseType?.GetGenericTypeDefinition() == policyType)
                {
                    services.AddScoped(type.BaseType, type);
                }
            }

            return services;
        }
    }
}