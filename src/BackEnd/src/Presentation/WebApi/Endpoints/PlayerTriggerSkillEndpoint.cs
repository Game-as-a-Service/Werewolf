namespace Wsa.Gaas.Werewolf.WebApi.Endpoints;


public class PlayerTriggerSkillEndpoint : WebApiEndpoint<PlayerTriggerSkillRequest, PlayerTriggerSkillResponse>
{
    public override void Configure()
    {
        Post("/games/{DiscordVoiceChannelId}/players/{PlayerId}:killPlayer");
        AllowAnonymous();
    }

}
