using Wsa.Gaas.Werewolf.Domain.Common;

namespace Wsa.Gaas.Werewolf.Application.Common
{
    public abstract class Policy<TGameEvent>
        where TGameEvent : GameEvent
    {
        protected IRepository Repository { get; }
        protected GameEventBus GameEventBus { get; }

        protected Policy(IRepository repository, GameEventBus gameEventBus)
        {
            Repository = repository;
            GameEventBus = gameEventBus;
        }

        public abstract Task ExecuteAsync(TGameEvent request, CancellationToken cancellationToken = default);
    }
}
