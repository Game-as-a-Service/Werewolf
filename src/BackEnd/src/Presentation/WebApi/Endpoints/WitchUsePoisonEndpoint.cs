using Wsa.Gaas.Werewolf.WebApi.Common;

namespace Wsa.Gaas.Werewolf.WebApi.Endpoints;


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

