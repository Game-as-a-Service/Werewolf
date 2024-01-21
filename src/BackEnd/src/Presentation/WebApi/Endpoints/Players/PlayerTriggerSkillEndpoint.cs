using Wsa.Gaas.Werewolf.Application.UseCases.Players;

namespace Wsa.Gaas.Werewolf.WebApi.Endpoints.Players;


public class PlayerTriggerSkillEndpoint : WebApiEndpoint<PlayerTriggerSkillRequest, PlayerTriggerSkillResponse>
{
    public override void Configure()
    {
        Post("/games/{DiscordVoiceChannelId}/players/{PlayerId}:killPlayer");
        AllowAnonymous();
    }

}
