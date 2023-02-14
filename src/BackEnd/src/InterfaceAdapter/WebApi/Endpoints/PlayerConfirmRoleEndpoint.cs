using Wsa.Gaas.Werewolf.Application.UseCases;
using Wsa.Gaas.Werewolf.Domain.Events;
using Wsa.Gaas.Werewolf.WebApi.Common;

namespace Wsa.Gaas.Werewolf.WebApi.Endpoints
{ 
    public class PlayerConfirmRoleResponse
    {
        public string GameId { get; set; } = string.Empty;
        public string PlayerId { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
    }

    public class PlayerConfirmRoleEndpoint : WebApiEndpoint<PlayerConfirmRoleRequest, PlayerRoleConfirmedEvent, PlayerConfirmRoleResponse>
    {
        /// <summary>
        /// 註冊api Route
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
        public async override Task<PlayerConfirmRoleResponse> ExecuteAsync(PlayerConfirmRoleRequest req, CancellationToken ct)
        {
            await UseCase.ExecuteAsync(req, this, ct);

            if (ViewModel == null)
            {
                throw new Exception("View Model is null");
            }

            return ViewModel;
        }

        public override Task PresentAsync(PlayerRoleConfirmedEvent gameEvent, CancellationToken cancellationToken = default)
        {
            ViewModel = new PlayerConfirmRoleResponse
            {
                GameId = gameEvent.Data.DiscordVoiceChannelId.ToString(),
                PlayerId = gameEvent.PlayerId.ToString(),
                Role = gameEvent.Role,
            };

            return Task.CompletedTask;
        }
    }
}
