using FastEndpoints;
using Wsa.Gaas.Werewolf.Application.Common;
using Wsa.Gaas.Werewolf.Domain.Common;

namespace Wsa.Gaas.Werewolf.WebApi.Common;

public abstract class WebApiEndpoint<TRequest, TGameEvent, TViewModel> : Endpoint<TRequest, TViewModel>, IPresenter<TGameEvent>
    where TRequest : notnull, new()
    where TViewModel : notnull
    where TGameEvent : GameEvent
{
    protected TViewModel? ViewModel { get; set; }

    public required UseCase<TRequest, TGameEvent> UseCase { get; set; }

    public abstract Task PresentAsync(TGameEvent gameEvent, CancellationToken cancellationToken = default);

    public abstract override Task<TViewModel> ExecuteAsync(TRequest req, CancellationToken ct);

}
