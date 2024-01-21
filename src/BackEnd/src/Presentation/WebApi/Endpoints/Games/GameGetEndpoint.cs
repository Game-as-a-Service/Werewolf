using Wsa.Gaas.Werewolf.Application.UseCases.Games;

namespace Wsa.Gaas.Werewolf.WebApi.Endpoints.Games;

public class GameGetEndpoint : WebApiEndpoint<GameGetRequest, GameGetResponse>
{
    public override void Configure()
    {
        Get("/games/{DiscordVoiceChannelId}");
        AllowAnonymous();
    }
}
