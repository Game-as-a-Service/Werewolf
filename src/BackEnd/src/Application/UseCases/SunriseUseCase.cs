using Domain.Events;
using Wsa.Gaas.Werewolf.Application.Common;
using Wsa.Gaas.Werewolf.Domain.Exceptions;
using Wsa.Gaas.Werewolf.Domain.Objects;

namespace Application.UseCases
{
    public class SunriseRequest
    {
        public ulong DiscordVoiceChannelId { get; set; }

        public ulong[] Players { get; set; } = Array.Empty<ulong>();
    }

    public class SunriseUseCase : UseCase<SunriseRequest, SunriseEvent>
    {
        private readonly static object _lock = new();

        public SunriseUseCase(IRepository repository, GameEventBus gameEventBus) : base(repository, gameEventBus)
        {
        }

        public override async Task ExecuteAsync(SunriseRequest request, IPresenter<SunriseEvent> presenter, CancellationToken cancellationToken = default)
        {
            Game? game;

            lock (_lock)
            {
                // 查
                game = Repository.FindAll()
                    .Where(x => x.DiscordVoiceChannelId == request.DiscordVoiceChannelId)
                    .Where(x => x.Status != GameStatus.Ended)
                    .FirstOrDefault();

                if (game == null)
                {
                    throw new GameNotFoundException(request.DiscordVoiceChannelId);
                }

                // 改
                game.StartGame(request.Players);

                // 存
                Repository.Save(game);
            }

            // 推
            var gameEvent = new SunriseEvent(game);

            // SignalR
            await GameEventBus.BroadcastAsync(gameEvent, cancellationToken);

            // Restful API
            await presenter.PresentAsync(gameEvent, cancellationToken);
        }
    }
}