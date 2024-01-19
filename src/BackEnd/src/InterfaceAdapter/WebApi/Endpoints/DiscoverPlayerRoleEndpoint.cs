namespace Wsa.Gaas.Werewolf.Application;
public class DiscoverPlayerRoleEndpoint : WebApiEndpoint<DiscoverPlayerRoleRequest, DiscoverPlayerRoleResponse>
{
    public override void Configure()
    {
        Post("/games/{DiscordVoiceChannelId}/players/{PlayerId}/DiscoverRole");
        AllowAnonymous();
    }

    public override async Task<DiscoverPlayerRoleResponse> ExecuteAsync(DiscoverPlayerRoleRequest req, CancellationToken ct)
    {
        return await UseCase.ExecuteAsync(req, ct);
    }
}
