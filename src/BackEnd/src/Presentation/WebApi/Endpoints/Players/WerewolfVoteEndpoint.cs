using Wsa.Gaas.Werewolf.Application.UseCases.Players;

namespace Wsa.Gaas.Werewolf.WebApi.Endpoints.Players;
public class WerewolfVoteEndpoint : WebApiEndpoint<WerewolfVoteRequest, WerewolfVoteResponse>
{
    public override void Configure()
    {
        Post("/games/{DiscordChannelId}/werewolf/vote");
        AllowAnonymous();
    }
}
