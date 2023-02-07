namespace Was.Gaas.Werewolf.Application.Common
{
    public abstract class Policy<TGameEvent>
    {
        protected IRepository Repository { get; }
        protected IEventPublisher EventPublisher { get; }

        protected Policy(IRepository repository, IEventPublisher eventPublisher)
        {
            Repository = repository;
            EventPublisher = eventPublisher;
        }

        public abstract Task ExecuteAsync(TGameEvent request);
    }
}
