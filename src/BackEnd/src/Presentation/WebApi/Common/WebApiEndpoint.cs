using FastEndpoints;
using Wsa.Gaas.Werewolf.Application.Common;

namespace Wsa.Gaas.Werewolf.WebApi.Common;
public abstract class WebApiEndpoint<TRequest, TResponse> : Endpoint<TRequest, TResponse>
    where TRequest : notnull, new()
    where TResponse : notnull
{
    public required UseCase<TRequest, TResponse> UseCase { get; set; }

    public override Task<TResponse> ExecuteAsync(TRequest req, CancellationToken ct)
    {
        return UseCase.ExecuteAsync(req, ct);
    }

}
