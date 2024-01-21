using Wsa.Gaas.Werewolf.Application.UseCases.Players;

namespace Wsa.Gaas.Werewolf.WebApi.Endpoints.Players;
public class DiscoverPlayerRoleEndpoint : WebApiEndpoint<DiscoverPlayerRoleRequest, DiscoverPlayerRoleResponse>
{
    public override void Configure()
    {
        Post("/games/{DiscordVoiceChannelId}/players/{PlayerId}/DiscoverRole");
        AllowAnonymous();
    }
}
