namespace Wsa.Gaas.Werewolf.Application.Common;
public abstract class UseCase<TRequest, TResponse>
{
    protected IRepository Repository { get; }
    protected GameEventBus GameEventBus { get; }

    protected UseCase(IRepository repository, GameEventBus gameEventBus)
    {
        Repository = repository;
        GameEventBus = gameEventBus;
    }

    public abstract Task<TResponse> ExecuteAsync(TRequest request, CancellationToken cancellationToken = default);
}
