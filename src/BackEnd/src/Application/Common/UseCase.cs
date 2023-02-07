using Wsa.Gaas.Werewolf.Domain.Common;

namespace Was.Gaas.Werewolf.Application.Common
{
    public abstract class UseCase<TRequest, TGameEvent>
        where TGameEvent : GameEvent
    {
        protected IRepository Repository { get; }
        protected IEventPublisher EventPublisher { get; }

        protected UseCase(IRepository repository, IEventPublisher eventPublisher)
        {
            Repository = repository;
            EventPublisher = eventPublisher;
        }

        public abstract Task ExecuteAsync(TRequest request, IPresenter<TGameEvent> presenter);
    }
}
