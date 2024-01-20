namespace Wsa.Gaas.Werewolf.WebApi.Endpoints;
public class EndGameEndpoint : WebApiEndpoint<EndGameRequest, EndGameResponse>
{
    public override void Configure()
    {
        Post("/games/{DiscordVoiceChannelId}/end");
        AllowAnonymous();
    }
}
