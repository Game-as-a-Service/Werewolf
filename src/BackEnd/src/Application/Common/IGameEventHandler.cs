using Wsa.Gaas.Werewolf.Domain.Events;

namespace Wsa.Gaas.Werewolf.Application.Common
{
    public interface IGameEventHandler
    {
        Task Handle(GameEvent gameEvent, CancellationToken cancellationToken = default);
    }
}