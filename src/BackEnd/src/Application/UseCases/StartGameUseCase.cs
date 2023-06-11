using Wsa.Gaas.Werewolf.Application.Common;
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
            GameStartedEvent? gameEvent;

            lock (_lock)
            {
                // 查
                game = Repository
                    .FindByDiscordChannelIdAsync(request.DiscordVoiceChannelId)
                    .GetAwaiter()
                    .GetResult()
                    ;

                if (game == null)
                {
                    throw new GameNotFoundException(request.DiscordVoiceChannelId);
                }

                // 改
                gameEvent = game.StartGame(request.Players);

                // 存
                Repository.Save(game);
            }

            // SignalR
            await GameEventBus.BroadcastAsync(gameEvent, cancellationToken);

            // Restful API
            await presenter.PresentAsync(gameEvent, cancellationToken);
        }
    }

}
