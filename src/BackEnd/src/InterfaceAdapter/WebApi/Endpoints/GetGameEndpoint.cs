using Wsa.Gaas.Werewolf.Application.UseCases;

namespace Wsa.Gaas.Werewolf.Application;

public class GetGameEndpoint : WebApiEndpoint<GetGameRequest, GetGameResponse>
{
    public override void Configure()
    {
        Get("/games/{DiscordVoiceChannelId}");
        AllowAnonymous();
    }

    public override async Task<GetGameResponse> ExecuteAsync(GetGameRequest req, CancellationToken ct)
    {
        // 執行 UseCase.Execute();
        return await UseCase.ExecuteAsync(req, ct);
    }
}
