using Wsa.Gaas.Werewolf.Application.UseCases.Games;

namespace Wsa.Gaas.Werewolf.WebApi.Endpoints.Games;
public class GameStartEndpoint : WebApiEndpoint<GameStartRequest, GameStartResponse>
{
    public override void Configure()
    {
        Post("/games/{DiscordVoiceChannelId}/start");
        AllowAnonymous();
    }
}
