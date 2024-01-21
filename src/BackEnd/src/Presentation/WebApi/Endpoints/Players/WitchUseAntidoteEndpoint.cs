using Wsa.Gaas.Werewolf.Application.UseCases.Players;

namespace Wsa.Gaas.Werewolf.WebApi.Endpoints.Players;
public class WitchUseAntidoteEndpoint : WebApiEndpoint<WitchUseAntidoteRequest, WitchUseAntidoteResponse>
{
    public override void Configure()
    {
        Post("/games/{DiscordVoiceChannelId}/players/{playerId}:useAnitdote");
        AllowAnonymous();
    }
}


