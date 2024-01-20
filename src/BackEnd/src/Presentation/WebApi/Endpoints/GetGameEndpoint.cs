namespace Wsa.Gaas.Werewolf.WebApi.Endpoints;

public class GetGameEndpoint : WebApiEndpoint<GetGameRequest, GetGameResponse>
{
    public override void Configure()
    {
        Get("/games/{DiscordVoiceChannelId}");
        AllowAnonymous();
    }
}
