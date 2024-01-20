namespace Wsa.Gaas.Werewolf.WebApi.Endpoints;
public class WerewolfVoteEndpoint : WebApiEndpoint<WerewolfVoteRequest, WerewolfVoteResponse>
{
    public override void Configure()
    {
        Post("/games/{DiscordChannelId}/werewolf/vote");
        AllowAnonymous();
    }
}
