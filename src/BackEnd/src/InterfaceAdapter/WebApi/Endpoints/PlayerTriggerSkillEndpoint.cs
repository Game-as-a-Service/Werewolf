using Wsa.Gaas.Werewolf.Application.UseCases;
using Wsa.Gaas.Werewolf.Domain.Events;
using Wsa.Gaas.Werewolf.WebApi.Common;

namespace Wsa.Gaas.Werewolf.WebApi.Endpoints
{
    public class PlayerTriggerSkillResponse { }

    public class PlayerTriggerSkillEndpoint : WebApiEndpoint<PlayerTriggerSkillRequest, PlayerTriggerSkillEvent, PlayerTriggerSkillResponse>
    {
        public override void Configure()
        {
            Post("/games/{DiscordVoiceChannelId}/players/{PlayerId}:killPlayer");
            AllowAnonymous();
        }

        public override async Task<PlayerTriggerSkillResponse> ExecuteAsync(PlayerTriggerSkillRequest req, CancellationToken ct)
        {
            await UseCase.ExecuteAsync(req, this, ct);

            if (ViewModel == null)
            {
                throw new Exception("View Model is null");
            }

            return ViewModel;
        }

        public override Task PresentAsync(PlayerTriggerSkillEvent gameEvent, CancellationToken cancellationToken = default)
        {
            ViewModel = new PlayerTriggerSkillResponse(
                //gameEvent.Data.DiscordVoiceChannelId.ToString(),
                //gameEvent.PlayerId.ToString()
            );

            return Task.CompletedTask;
        }
    }
}