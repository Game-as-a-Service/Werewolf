using Wsa.Gaas.Werewolf.Application.Common;
using Wsa.Gaas.Werewolf.Domain.Common;
using Wsa.Gaas.Werewolf.Domain.Events;
using Wsa.Gaas.Werewolf.Domain.Exceptions;
using Wsa.Gaas.Werewolf.Domain.Objects;

namespace Wsa.Gaas.Werewolf.Application.UseCases
{
    public class StartGameRequest
    {
        public ulong DiscordVoiceChannelId { get; set; }

        public ulong[] Players { get; set; } = Array.Empty<ulong>();
    }

    public class StartGameUseCase : UseCase<StartGameRequest, GameStartedEvent>
    {
        private readonly static object _lock = new();

        public StartGameUseCase(IRepository repository, GameEventBus eventPublisher) : base(repository, eventPublisher)
        {
        }

        public override async Task ExecuteAsync(StartGameRequest request, IPresenter<GameStartedEvent> presenter, CancellationToken cancellationToken = default)
        {
            Game? game;
            IEnumerable<GameEvent> events;

            lock (_lock)
            {
                // 查
                game = Repository.FindByDiscordChannelId(request.DiscordVoiceChannelId);

                if (game == null)
                {
                    throw new GameNotFoundException(request.DiscordVoiceChannelId);
                }

                // 改
                events = game.StartGame(request.Players);

                // 存
                Repository.Save(game);
            }

            // SignalR
            await GameEventBus.BroadcastAsync(events, cancellationToken);

            // Restful API
            await presenter.PresentAsync(
                (GameStartedEvent)events.First(x => x is GameStartedEvent),
                cancellationToken
            );
        }
    }

}
