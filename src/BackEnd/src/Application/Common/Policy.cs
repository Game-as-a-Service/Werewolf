using Microsoft.Extensions.Options;
using Wsa.Gaas.Werewolf.Application.Options;

namespace Wsa.Gaas.Werewolf.Application.Common;
internal abstract class Policy<T> : IGameEventHandler<T>
    where T : GameEvent
{
    protected readonly GameSettingOptions Options;
    protected readonly IRepository Repository;
    protected readonly GameEventBus GameEventBus;

    public Policy(
        IOptions<GameSettingOptions> options,
        IRepository repository,
        GameEventBus gameEventBus
    )
    {
        Options = options.Value;
        Repository = repository;
        GameEventBus = gameEventBus;
    }

    public abstract Task Handle(T gameEvent, CancellationToken cancellationToken = default);
}
