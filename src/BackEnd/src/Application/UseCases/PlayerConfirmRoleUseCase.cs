using Wsa.Gaas.Werewolf.Application.Common;
using Wsa.Gaas.Werewolf.Domain.Events;
using Wsa.Gaas.Werewolf.Domain.Exceptions;
using Wsa.Gaas.Werewolf.Domain.Objects;

namespace Wsa.Gaas.Werewolf.Application.UseCases
{
    public class PlayerConfirmRoleRequest
    {
        public ulong DiscordVoiceChannelId { get; set; }
        public ulong PlayerId { get; set; }
    }

    public class PlayerConfirmRoleUseCase : UseCase<PlayerConfirmRoleRequest, PlayerRoleConfirmedEvent>
    {
        public PlayerConfirmRoleUseCase(IRepository repository, GameEventBus gameEventBus) : base(repository, gameEventBus)
        {
        }

        public async override Task ExecuteAsync(PlayerConfirmRoleRequest request, IPresenter<PlayerRoleConfirmedEvent> presenter, CancellationToken cancellationToken = default)
        {
            // Query
            var game = await Repository.FindByDiscordChannelIdAsync(request.DiscordVoiceChannelId);

            if (game == null)
            {
                throw new GameNotFoundException(request.DiscordVoiceChannelId);
            }

            // Update (Query)
            var gameEvent = game.ConfirmPlayerRole(request.PlayerId);

            // Save


            // Push
            await GameEventBus.BroadcastAsync(gameEvent, cancellationToken);
            await presenter.PresentAsync(gameEvent, cancellationToken);
        }
    }
}
