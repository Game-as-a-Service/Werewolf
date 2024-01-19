namespace Wsa.Gaas.Werewolf.Application;
public class EndGameEndpoint : WebApiEndpoint<EndGameRequest, EndGameResponse>
{
    public override void Configure()
    {
        Post("/games/{DiscordVoiceChannelId}/end");
        AllowAnonymous();
    }

    public override async Task<EndGameResponse> ExecuteAsync(EndGameRequest req, CancellationToken ct)
    {
        return await UseCase.ExecuteAsync(req, ct);
    }
}
