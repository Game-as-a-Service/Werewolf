namespace Wsa.Gaas.Werewolf.WebApi.Endpoints;

public class ConfirmPlayerRoleEndpoint : WebApiEndpoint<ConfirmPlayerRoleRequest, ConfirmPlayerRoleResponse>
{
    public override void Configure()
    {
        Get("/games/{DiscordVoiceChannelId}/players/{PlayerId}/Role");
        AllowAnonymous();
    }
}
