using Wsa.Gaas.Werewolf.Application.UseCases.Players;

namespace Wsa.Gaas.Werewolf.WebApi.Endpoints.Players;

public class PlayerGetRoleEndpoint : WebApiEndpoint<PlayerGetRoleRequest, PlayerGetRoleResponse>
{
    public override void Configure()
    {
        Get("/games/{DiscordVoiceChannelId}/players/{PlayerId}/Role");
        AllowAnonymous();
    }
}
