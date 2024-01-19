using Wsa.Gaas.Werewolf.Application;

namespace Wsa.Gaas.Werewolf.WebApi.Endpoints;


public class PlayerTriggerSkillEndpoint : WebApiEndpoint<PlayerTriggerSkillRequest, PlayerTriggerSkillResponse>
{
    public override void Configure()
    {
        Post("/games/{DiscordVoiceChannelId}/players/{PlayerId}:killPlayer");
        AllowAnonymous();
    }

    public override async Task<PlayerTriggerSkillResponse> ExecuteAsync(PlayerTriggerSkillRequest req, CancellationToken ct)
    {
        return await UseCase.ExecuteAsync(req, ct);
    }

}
