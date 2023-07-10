using Wsa.Gaas.Werewolf.Application.Common;
using Wsa.Gaas.Werewolf.Domain.Events;
using Wsa.Gaas.Werewolf.Domain.Exceptions;

namespace Wsa.Gaas.Werewolf.Application.UseCases
{
    public class DiscoverPlayerRoleRequest
    {
        public ulong DiscordVoiceChannelId { get; set; }
        public ulong PlayerId { get; set; }
        public int DiscoverPlayerNumber { get; set; }
    }

    public class DiscoverPlayerRoleUseCase : UseCase<DiscoverPlayerRoleRequest, SeerDiscoveredEvent>
    {
        public DiscoverPlayerRoleUseCase(IRepository repository, GameEventBus gameEventBus) : base(repository, gameEventBus)
        {
        }

        public override async Task ExecuteAsync(DiscoverPlayerRoleRequest request, IPresenter<SeerDiscoveredEvent> presenter, CancellationToken cancellationToken = default)
        {
            // Query
            var game = await Repository.FindByDiscordChannelIdAsync(request.DiscordVoiceChannelId);

            if (game == null)
            {
                throw new GameNotFoundException(request.DiscordVoiceChannelId);
            }

            // Update
            var seerDiscoveredEvent = game.DiscoverPlayerRole(request.PlayerId, request.DiscoverPlayerNumber);

            // Save

            // Push
            await presenter.PresentAsync(seerDiscoveredEvent, cancellationToken);
        }
    }
}
