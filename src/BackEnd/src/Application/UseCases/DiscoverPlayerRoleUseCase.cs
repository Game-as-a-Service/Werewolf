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
            var discoverPlayer = Repository.FindAll()
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
            // Save
            // Push
        }
    }
}
