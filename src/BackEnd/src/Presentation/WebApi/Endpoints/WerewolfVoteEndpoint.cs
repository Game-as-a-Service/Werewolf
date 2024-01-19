using Wsa.Gaas.Werewolf.WebApi.Common;

namespace Wsa.Gaas.Werewolf.WebApi.Endpoints;
public class WerewolfVoteEndpoint : WebApiEndpoint<WerewolfVoteRequest, WerewolfVoteResponse>
{
    public override void Configure()
    {
        Post("/games/{DiscordChannelId}/werewolf/vote");
        AllowAnonymous();
    }

    public override async Task<WerewolfVoteResponse> ExecuteAsync(WerewolfVoteRequest req, CancellationToken ct)
    {
        return await UseCase.ExecuteAsync(req, ct);
    }
}
