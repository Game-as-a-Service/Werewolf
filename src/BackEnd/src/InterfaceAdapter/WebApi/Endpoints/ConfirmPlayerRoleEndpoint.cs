namespace Wsa.Gaas.Werewolf.Application;

public class ConfirmPlayerRoleEndpoint : WebApiEndpoint<ConfirmPlayerRoleRequest, ConfirmPlayerRoleResponse>
{
    /// <summary>
    /// Register API Route
    /// </summary>
    public override void Configure()
    {
        Get("/games/{DiscordVoiceChannelId}/players/{PlayerId}/Role");
        AllowAnonymous();
    }

    /// <summary>
    /// api request
    /// </summary>
    /// <param name="req"></param>
    /// <param name="ct"></param>
    /// <returns></returns>
    public async override Task<ConfirmPlayerRoleResponse> ExecuteAsync(ConfirmPlayerRoleRequest req, CancellationToken ct)
    {
        return await UseCase.ExecuteAsync(req, ct);
    }

}
