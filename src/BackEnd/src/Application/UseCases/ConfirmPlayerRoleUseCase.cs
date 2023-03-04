using Wsa.Gaas.Werewolf.Application.Common;
using Wsa.Gaas.Werewolf.Domain.Events;
using Wsa.Gaas.Werewolf.Domain.Exceptions;

namespace Wsa.Gaas.Werewolf.Application.UseCases
{
    public class ConfirmPlayerRoleRequest
    {
        public long RoomId { get; set; }
        public long PlayerId { get; set; }
    }

    public class ConfirmPlayerRoleUseCase : UseCase<ConfirmPlayerRoleRequest, PlayerRoleConfirmedEvent>
    {
        public ConfirmPlayerRoleUseCase(IRepository repository, GameEventBus gameEventBus) : base(repository, gameEventBus) { }

        public override async Task ExecuteAsync(ConfirmPlayerRoleRequest request, IPresenter<PlayerRoleConfirmedEvent> presenter, CancellationToken cancellationToken = default)
        {
            // Query
            var game = await Repository.FindByRoomIdAsync(request.RoomId);

            if (game == null)
            {
                throw new GameNotFoundException(request.RoomId);
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