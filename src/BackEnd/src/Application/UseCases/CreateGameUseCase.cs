using Wsa.Gaas.Werewolf.Application.Common;
using Wsa.Gaas.Werewolf.Domain.Events;
using Wsa.Gaas.Werewolf.Domain.Exceptions;
using Wsa.Gaas.Werewolf.Domain.Objects;

namespace Wsa.Gaas.Werewolf.Application.UseCases
{
    public class CreateGameRequest
    {
        public long RoomId { get; set; }
    }

    public class CreateGameUseCase : UseCase<CreateGameRequest, GameCreatedEvent>
    {
        private readonly static object _lock = new();

        public CreateGameUseCase(IRepository repository, GameEventBus eventPublisher) : base(repository, eventPublisher)
        {
        }

        public override async Task ExecuteAsync(CreateGameRequest request, IPresenter<GameCreatedEvent> presenter, CancellationToken cancellationToken = default)
        {
            Game game;

            lock (_lock)
            {
                // Query
                var anyExistingActiveGame = Repository.FindAll()
                    .Where(x => x.RoomId == request.RoomId)
                    .Where(x => x.Status != GameStatus.Ended)
                    .Any();

                if (anyExistingActiveGame)
                {
                    throw new GameChannelException();
                }

                // Update
                game = new Game(request.RoomId);

                // Save
                Repository.Save(game);
            }

            // Push
            var gameEvent = new GameCreatedEvent(game);

            // SignalR
            await GameEventBus.BroadcastAsync(gameEvent, cancellationToken);

            // Restful API
            await presenter.PresentAsync(gameEvent, cancellationToken);
        }
    }

}
