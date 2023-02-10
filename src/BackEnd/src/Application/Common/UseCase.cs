using Wsa.Gaas.Werewolf.Domain.Common;

namespace Wsa.Gaas.Werewolf.Application.Common
{
    public abstract class UseCase<TRequest, TGameEvent>
        where TGameEvent : GameEvent
    {
        protected IRepository Repository { get; }
        protected GameEventBus GameEventBus { get; }

        protected UseCase(IRepository repository, GameEventBus gameEventBus)
        {
            Repository = repository;
            GameEventBus = gameEventBus;
        }

        public abstract Task ExecuteAsync(TRequest request, IPresenter<TGameEvent> presenter, CancellationToken cancellationToken = default);
    }
}
