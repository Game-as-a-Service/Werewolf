using Microsoft.AspNetCore.SignalR;
using Wsa.Gaas.Werewolf.Application.Common;
using Wsa.Gaas.Werewolf.Domain.Common;

namespace Wsa.Gaas.Werewolf.WebApi
{
    public class GameEventHub : Hub, IGameEventHandler
    {
        private readonly IHubContext<GameEventHub> _hubContext;

        public GameEventHub(IHubContext<GameEventHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public Task Handle(GameEvent gameEvent, CancellationToken cancellationToken = default) 
        {
            return _hubContext.Clients.All.SendAsync(WebApiDefaults.SignalrPublishMethodName, gameEvent, cancellationToken);
        }
    }


}
