using Wsa.Gaas.Werewolf.Application.UseCases;
using Wsa.Gaas.Werewolf.Domain.Events;
using Wsa.Gaas.Werewolf.WebApi.Common;

namespace Wsa.Gaas.Werewolf.WebApi.Endpoints
{
    public record ConfirmPlayerRoleResponse(string GameId, string PlayerId, string Role);
    

    public class ConfirmPlayerRoleEndpoint : WebApiEndpoint<ConfirmPlayerRoleRequest, PlayerRoleConfirmedEvent, ConfirmPlayerRoleResponse>
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
            // MVP
            // Restful API Presenter => JSON
            // CLI Presenter         => Text
            
            await UseCase.ExecuteAsync(req, this, ct);

            if (ViewModel == null)
            {
                throw new Exception("View Model is null");
            }

            return ViewModel;
        }

        public override Task PresentAsync(PlayerRoleConfirmedEvent gameEvent, CancellationToken cancellationToken = default)
        {
            ViewModel = new ConfirmPlayerRoleResponse(
                gameEvent.Data.DiscordVoiceChannelId.ToString(),
                gameEvent.PlayerId.ToString(),
                gameEvent.Role
            );

            return Task.CompletedTask;
        }
    }
}
