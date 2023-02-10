using Wsa.Gaas.Werewolf.Domain.Common;

namespace Wsa.Gaas.Werewolf.Application.Common
{
    public abstract class Policy<TGameEvent>
        where TGameEvent : GameEvent
    {
        protected IRepository Repository { get; }
        protected GameEventBus EventPublisher { get; }

        protected Policy(IRepository repository, GameEventBus eventPublisher)
        {
            Repository = repository;
            EventPublisher = eventPublisher;
        }

        public abstract Task ExecuteAsync(TGameEvent request, CancellationToken cancellationToken = default);
    }
}
