namespace Wsa.Gaas.Werewolf.Application;


public class WitchUsePoisonEndpoint : WebApiEndpoint<WitchUsePoisonRequest, WitchUsePoisonResponse>
{
    public override void Configure()
    {
        Post("/games/{DiscordVoiceChannelId}/players/{playerId}:usePoison");
        AllowAnonymous();
    }

    public override async Task<WitchUsePoisonResponse> ExecuteAsync(WitchUsePoisonRequest req, CancellationToken ct)
    {
        return await UseCase.ExecuteAsync(req, ct);
    }
}

