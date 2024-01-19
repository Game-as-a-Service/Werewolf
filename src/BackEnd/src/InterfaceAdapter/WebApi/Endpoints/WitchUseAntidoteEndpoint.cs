namespace Wsa.Gaas.Werewolf.Application;
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


