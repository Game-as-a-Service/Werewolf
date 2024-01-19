using FastEndpoints;
using Wsa.Gaas.Werewolf.Application.Common;

namespace Wsa.Gaas.Werewolf.WebApi.Common;
public abstract class WebApiEndpoint<TRequest, TViewModel> : Endpoint<TRequest, TViewModel>
    where TRequest : notnull, new()
    where TViewModel : notnull
{
    public required UseCase<TRequest, TViewModel> UseCase { get; set; }

    public abstract override Task<TViewModel> ExecuteAsync(TRequest req, CancellationToken ct);

}
