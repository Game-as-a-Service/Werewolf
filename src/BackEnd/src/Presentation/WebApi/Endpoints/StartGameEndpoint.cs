namespace Wsa.Gaas.Werewolf.WebApi.Endpoints;
public class StartGameEndpoint : WebApiEndpoint<StartGameRequest, StartGameResponse>
{
    public override void Configure()
    {
        Post("/games/{DiscordVoiceChannelId}/start");
        AllowAnonymous();
    }
}
