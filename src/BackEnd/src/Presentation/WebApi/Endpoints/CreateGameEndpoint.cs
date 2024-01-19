using Wsa.Gaas.Werewolf.WebApi.Common;

namespace Wsa.Gaas.Werewolf.WebApi.Endpoints;
public class CreateGameEndpoint : WebApiEndpoint<CreateGameRequest, CreateGameResponse>
{
    public override void Configure()
    {
        Post("/games");
        AllowAnonymous();
    }

    public override async Task<CreateGameResponse> ExecuteAsync(CreateGameRequest req, CancellationToken ct)
    {
        return await UseCase.ExecuteAsync(req, ct);
    }
}
