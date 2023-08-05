using Wsa.Gaas.Werewolf.Domain.Common;

namespace Wsa.Gaas.Werewolf.Application.Common;

public interface IGameEventHandler
{
    Task Handle(GameEvent events, CancellationToken cancellationToken = default);
}

public interface IGameEventHandler<T>
    where T : GameEvent
{
    Task Handle(T gameEvent, CancellationToken cancellationToken = default);
}