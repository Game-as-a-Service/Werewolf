using Wsa.Gaas.Werewolf.Application.UseCases.Games;

namespace Wsa.Gaas.Werewolf.WebApi.Endpoints.Games;
public class GameCreateEndpoint : WebApiEndpoint<GameCreateRequest, GameCreateResponse>
{
    public override void Configure()
    {
        Post("/games");
        AllowAnonymous();
    }
}
