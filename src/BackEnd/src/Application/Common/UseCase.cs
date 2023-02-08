using Wsa.Gaas.Werewolf.Domain.Common;

namespace Wsa.Gaas.Werewolf.Application.Common
{
    public abstract class UseCase<TRequest, TGameEvent>
        where TGameEvent : GameEvent
    {
        protected IRepository Repository { get; }
        protected GameEventPublisher EventPublisher { get; }

        protected UseCase(IRepository repository, GameEventPublisher eventPublisher)
        {
            Repository = repository;
            EventPublisher = eventPublisher;
        }

        public abstract Task ExecuteAsync(TRequest request, IPresenter<TGameEvent> presenter, CancellationToken cancellationToken = default);
    }
}
