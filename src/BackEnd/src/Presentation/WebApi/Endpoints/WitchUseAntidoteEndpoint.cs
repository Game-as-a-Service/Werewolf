namespace Wsa.Gaas.Werewolf.WebApi.Endpoints;
public class WitchUseAntidoteEndpoint : WebApiEndpoint<WitchUseAntidoteRequest, WitchUseAntidoteResponse>
{
    public override void Configure()
    {
        Post("/games/{DiscordVoiceChannelId}/players/{playerId}:useAnitdote");
        AllowAnonymous();
    }
}


