using Wsa.Gaas.Werewolf.Application.Common;
using Wsa.Gaas.Werewolf.Domain.Events;
using Wsa.Gaas.Werewolf.Domain.Exceptions;
using Wsa.Gaas.Werewolf.Domain.Objects;

namespace Wsa.Gaas.Werewolf.Application.UseCases
{
    public class StartGameRequest
    {
        public long RoomId { get; set; }

        public long[] Players { get; set; } = Array.Empty<long>();
    }

    public class StartGameUseCase : UseCase<StartGameRequest, GameStartedEvent>
    {
        private static readonly object Lock = new();

        public StartGameUseCase(IRepository repository, GameEventBus eventPublisher) : base(repository, eventPublisher) { }

        public override async Task ExecuteAsync(StartGameRequest request, IPresenter<GameStartedEvent> presenter, CancellationToken cancellationToken = default)
        {
            Game? game;

            lock (Lock)
            {
                // 查
                game = Repository.FindAll()
                                 .Where(x => x.RoomId == request.RoomId)
                                 .Where(x => x.Status != GameStatus.Ended)
                                 .FirstOrDefault();

                if (game == null)
                {
                    throw new GameNotFoundException(request.RoomId);
                }

                // 改
                game.StartGame(request.Players);

                // 存
                Repository.Save(game);
            }

            // 推
            var gameEvent = new GameStartedEvent(game);

            // SignalR
            await GameEventBus.BroadcastAsync(gameEvent, cancellationToken);

            // Restful API
            await presenter.PresentAsync(gameEvent, cancellationToken);
        }
    }
}