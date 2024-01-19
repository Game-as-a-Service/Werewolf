using Wsa.Gaas.Werewolf.WebApi.Common;

namespace Wsa.Gaas.Werewolf.WebApi.Endpoints;
public class StartGameEndpoint : WebApiEndpoint<StartGameRequest, StartGameResponse>
{
    public override void Configure()
    {
        Post("/games/{DiscordVoiceChannelId}/start");
        AllowAnonymous();
    }

    public override async Task<StartGameResponse> ExecuteAsync(StartGameRequest req, CancellationToken ct)
    {
        return await UseCase.ExecuteAsync(req, ct);
    }
}
