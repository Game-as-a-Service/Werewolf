namespace Wsa.Gaas.Werewolf.WebApi.Endpoints;

public class WitchUsePoisonEndpoint : WebApiEndpoint<WitchUsePoisonRequest, WitchUsePoisonResponse>
{
    public override void Configure()
    {
        Post("/games/{DiscordVoiceChannelId}/players/{playerId}:usePoison");
        AllowAnonymous();
    }
}

