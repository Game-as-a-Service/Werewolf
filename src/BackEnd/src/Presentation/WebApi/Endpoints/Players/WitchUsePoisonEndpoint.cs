using Wsa.Gaas.Werewolf.Application.UseCases.Players;

namespace Wsa.Gaas.Werewolf.WebApi.Endpoints.Players;

public class WitchUsePoisonEndpoint : WebApiEndpoint<WitchUsePoisonRequest, WitchUsePoisonResponse>
{
    public override void Configure()
    {
        Post("/games/{DiscordVoiceChannelId}/players/{playerId}:usePoison");
        AllowAnonymous();
    }
}

