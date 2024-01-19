using Wsa.Gaas.Werewolf.Application.UseCases;

namespace Wsa.Gaas.Werewolf.Application;
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
