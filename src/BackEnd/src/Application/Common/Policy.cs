using Wsa.Gaas.Werewolf.Domain.Common;

namespace Wsa.Gaas.Werewolf.Application.Common;

internal abstract class Policy<T> : IGameEventHandler<T>
    where T : GameEvent
{
    public abstract Task Handle(T gameEvent, CancellationToken cancellationToken = default);
}
