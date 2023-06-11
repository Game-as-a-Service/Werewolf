namespace Wsa.Gaas.Werewolf.ChatBot.Application.Common
{
    public abstract class UseCase<TRequest>
    {
        public abstract Task ExecuteAsync(TRequest request, CancellationToken cancellationToken = default);
    }

    public abstract class UseCase<TRequest, TResponse>
    {
        //private readonly IWerewolfApiClient _werewolfApiClient;

        //protected UseCase(IWerewolfApiClient werewolfApiClient)
        //{
        //    //_werewolfApiClient = werewolfApiClient;
        //}

        public abstract Task<TResponse> ExecuteAsync(TRequest request, CancellationToken cancellationToken = default);
    }
}