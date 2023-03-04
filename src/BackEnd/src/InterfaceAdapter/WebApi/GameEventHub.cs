using Microsoft.AspNetCore.SignalR;
using Wsa.Gaas.Werewolf.Application.Common;
using Wsa.Gaas.Werewolf.Domain.Events;
using Wsa.Gaas.Werewolf.WebApi.ViewModels;

namespace Wsa.Gaas.Werewolf.WebApi
{
    /**
     * https://learn.microsoft.com/en-us/aspnet/core/signalr/hubs?view=aspnetcore-7.0#create-and-use-hubs-3
     * > Hubs are transient:
     * > Don't store state in a property of the hub class. Each hub method call is executed on a new hub instance.
     * > Don't instantiate a hub directly via dependency injection. To send messages to a client from elsewhere in your application use an IHubContext.
     * > Use await when calling asynchronous methods that depend on the hub staying alive. For example, a method such as Clients.All.SendAsync(...) can fail if it's called without await and the hub method completes before SendAsync finishes.
     **/
    public class GameEventHub : Hub { }

    public class GameEventHubHandler : IGameEventHandler
    {
        private readonly IHubContext<GameEventHub> _hubContext;

        public GameEventHubHandler(IHubContext<GameEventHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public async Task Handle(GameEvent gameEvent, CancellationToken cancellationToken = default)
        {
            await _hubContext.Clients.All.SendAsync(
                                                    gameEvent.GetType().Name,
                                                    GameVm.FromDomain(gameEvent.Data),
                                                    cancellationToken
                                                   );
        }
    }
}