using Wsa.Gaas.Werewolf.WebApi.Common;

namespace Wsa.Gaas.Werewolf.WebApi.Endpoints;
public class WitchUseAntidoteEndpoint : WebApiEndpoint<WitchUseAntidoteRequest, WitchUseAntidoteResponse>
{
    public override void Configure()
    {
        Post("/games/{DiscordVoiceChannelId}/players/{playerId}:useAnitdote");
        AllowAnonymous();
    }

    public override async Task<WitchUseAntidoteResponse> ExecuteAsync(WitchUseAntidoteRequest req, CancellationToken ct)
    {
        return await UseCase.ExecuteAsync(req, ct);
    }
}


