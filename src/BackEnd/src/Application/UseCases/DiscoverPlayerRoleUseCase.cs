using Wsa.Gaas.Werewolf.Application.Common;
using Wsa.Gaas.Werewolf.Domain.Events;
using Wsa.Gaas.Werewolf.Domain.Exceptions;
using Wsa.Gaas.Werewolf.Domain.Objects;

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

            var discoverPlayer = Repository.FindAll().Where(x => x.DiscordVoiceChannelId == request.DiscordVoiceChannelId)
                .Where(x => x.Status == GameStatus.SeerRoundStarted)
                .SelectMany(y => y.Players)
                .Where(y => y.IsDead == false)
                .Where(y => y.PlayerNumber == request.DiscoverPlayerNumber)
                .FirstOrDefault();

            if (discoverPlayer == null)
            {
                throw new PlayerNotSurvivedException(request.DiscoverPlayerNumber);
            }

            // Update
            var seerDiscoveredEvent = game.DiscoverPlayerRole(request.PlayerId, discoverPlayer);

            // Save

            // Push
            var gameEvent = new GameCreatedEvent(game);
            await GameEventBus.BroadcastAsync(gameEvent, cancellationToken);
            await presenter.PresentAsync(seerDiscoveredEvent, cancellationToken);
        }
    }
}
