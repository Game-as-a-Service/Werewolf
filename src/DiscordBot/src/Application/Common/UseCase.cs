namespace Wsa.Gaas.Werewolf.ChatBot.Application.Common;

public abstract class UseCase<TRequest>
{
    public abstract Task ExecuteAsync(TRequest request, CancellationToken cancellationToken = default);
}

public abstract class UseCase<TRequest, TResponse>
{
    public abstract Task<TResponse> ExecuteAsync(TRequest request, CancellationToken cancellationToken = default);
}