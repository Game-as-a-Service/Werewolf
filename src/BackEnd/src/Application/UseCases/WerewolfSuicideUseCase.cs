using Wsa.Gaas.Werewolf.Application.Common;
using Wsa.Gaas.Werewolf.Domain.Exceptions;
using Wsa.Gaas.Werewolf.WebApi.Endpoints;

namespace Wsa.Gaas.Werewolf.Application.UseCases
{
    public class WerewolfSuicideRequest
    {
        public ulong DiscordVoiceChannelId { get; set; }
        public ulong PlayerId { get; set; }
    }

    public class WerewolfSuicideUseCase : UseCase<WerewolfSuicideRequest, WerewolfSuicidedEvent>
    {
        public WerewolfSuicideUseCase(IRepository repository, GameEventBus gameEventBus) : base(repository, gameEventBus)
        {
        }

        public override async Task ExecuteAsync(WerewolfSuicideRequest request, IPresenter<WerewolfSuicidedEvent> presenter, CancellationToken cancellationToken = default)
        {
            var game = Repository.FindByDiscordChannelId(request.DiscordVoiceChannelId);

            if (game == null)
            {
                throw new GameNotFoundException(request.DiscordVoiceChannelId);
            }

            var events = game.WerewolfSuicide(request.PlayerId);

            Repository.Save(game);

            await GameEventBus.BroadcastAsync(events);
            await presenter.PresentAsync(events);
        }
    }
}
