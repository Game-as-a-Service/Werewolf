namespace Wsa.Gaas.Werewolf.Application.Common
{
    public abstract class Policy<TGameEvent>
    {
        protected IRepository Repository { get; }
        protected GameEventPublisher EventPublisher { get; }

        protected Policy(IRepository repository, GameEventPublisher eventPublisher)
        {
            Repository = repository;
            EventPublisher = eventPublisher;
        }

        public abstract Task ExecuteAsync(TGameEvent request, CancellationToken cancellationToken = default);
    }
}
