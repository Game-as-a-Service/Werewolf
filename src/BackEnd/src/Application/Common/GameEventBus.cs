using Microsoft.Extensions.DependencyInjection;
using Wsa.Gaas.Werewolf.Domain.Common;

namespace Wsa.Gaas.Werewolf.Application.Common
{
    public class GameEventBus
    {
        private readonly IServiceScopeFactory _factory;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        protected GameEventBus() { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

        public GameEventBus(IServiceScopeFactory facotry)
        {
            _factory = facotry;
        }

        public virtual Task BroadcastAsync<T>(T gameEvent, CancellationToken cancellationToken = default)
            where T : GameEvent
        {
            // Run this in sparate Thread
            Task.Run(async () =>
            {
                using var scope = _factory.CreateScope();
                var provider = scope.ServiceProvider;

                // Trigger Handlers

                // 1. Game Event Hub Handler
                var handler = provider.GetRequiredService<IGameEventHandler>();
                await handler.Handle(gameEvent, cancellationToken);

                // 2. Policies
                if (provider.GetService<Policy<T>>() is Policy<T> policy)
                {
                    await policy.ExecuteAsync(gameEvent, cancellationToken);
                }

            }, cancellationToken);

            return Task.CompletedTask;
        }
    }
}
