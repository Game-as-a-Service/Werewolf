using Wsa.Gaas.Werewolf.Application.UseCases.Games;

namespace Wsa.Gaas.Werewolf.WebApi.Endpoints.Games;
public class GameEndEndpoint : WebApiEndpoint<GameEndRequest, GameEndResponse>
{
    public override void Configure()
    {
        Post("/games/{DiscordVoiceChannelId}/end");
        AllowAnonymous();
    }
}
