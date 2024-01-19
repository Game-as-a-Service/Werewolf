using Microsoft.Extensions.DependencyInjection;
using Wsa.Gaas.Werewolf.Domain.Common;

namespace Wsa.Gaas.Werewolf.Application.Common;
public class GameEventBus
{
    private readonly IServiceScopeFactory _factory;

    public GameEventBus(IServiceScopeFactory facotry)
    {
        _factory = facotry;
    }

    public virtual Task BroadcastAsync<T>(IEnumerable<T> gameEvents, CancellationToken cancellationToken = default)
        where T : GameEvent
    {
        // Run this in sparate Thread
        Task.Run(async () =>
        {
            using var scope = _factory.CreateScope();
            var provider = scope.ServiceProvider;

            // Trigger Handlers

            // Game Event Hub Handler
            var handler = provider.GetRequiredService<IGameEventHandler>();

            foreach (var gameEvent in gameEvents)
            {
                await handler.Handle(gameEvent, cancellationToken);

                var policyType = typeof(Policy<>).MakeGenericType(gameEvent.GetType());

                var policy = provider.GetService(policyType);

                if (policy != null)
                {
                    var method = policy.GetType().GetMethod("Handle");

                    method?.Invoke(policy, new object[] { gameEvent, cancellationToken });
                }
            }
        }, cancellationToken);

        return Task.CompletedTask;
    }

    public virtual Task BroadcastAsync<T>(T gameEvent, CancellationToken cancellationToken = default)
        where T : GameEvent
    {
        return BroadcastAsync(new[] { gameEvent }, cancellationToken);
    }
}
