namespace Wsa.Gaas.Werewolf.WebApi.Endpoints;
public class DiscoverPlayerRoleEndpoint : WebApiEndpoint<DiscoverPlayerRoleRequest, DiscoverPlayerRoleResponse>
{
    public override void Configure()
    {
        Post("/games/{DiscordVoiceChannelId}/players/{PlayerId}/DiscoverRole");
        AllowAnonymous();
    }
}
