using Microsoft.Extensions.DependencyInjection;
using Wsa.Gaas.Werewolf.Domain.Common;

namespace Wsa.Gaas.Werewolf.Application.Common
{
    public class GameEventPublisher
    {
        private readonly IServiceScopeFactory _factory;

        public GameEventPublisher(IServiceScopeFactory facotry)
        {
            _factory = facotry;
        }

        public async Task PublishAsync<T>(T gameEvent, CancellationToken cancellationToken = default)
            where T : GameEvent
        {
            using var scope = _factory.CreateScope();
            var provider = scope.ServiceProvider;

            // Trigger Handlers, e.g. SignalR Hub
            var handlers = provider.GetServices<IGameEventHandler>();
            await Task.WhenAll(handlers.Select(x => x.Handle(gameEvent, cancellationToken)));

            // Trigger next Policy
            if (provider.GetService<Policy<T>>() is Policy<T> policy)
            {
                await policy.ExecuteAsync(gameEvent);
            }
        }
    }
}
