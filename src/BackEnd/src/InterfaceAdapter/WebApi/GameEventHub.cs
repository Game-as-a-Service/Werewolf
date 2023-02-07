using Microsoft.AspNetCore.SignalR;
using Was.Gaas.Werewolf.Application.Common;
using Wsa.Gaas.Werewolf.Domain.Common;

namespace Wsa.Gaas.Werewolf.WebApi
{
    public class GameEventHub : Hub, IEventPublisher
    {
        private readonly IHubContext<GameEventHub> _hubContext;

        public GameEventHub(IHubContext<GameEventHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public Task PublishAsync(GameEvent gameEvent)
        {
            return _hubContext.Clients.All.SendAsync("Publish", gameEvent);
        }
    }


}
