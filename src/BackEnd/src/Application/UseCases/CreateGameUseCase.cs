using Was.Gaas.Werewolf.Application.Common;
using Wsa.Gaas.Werewolf.Domain.Events;
using Wsa.Gaas.Werewolf.Domain.Exceptions;
using Wsa.Gaas.Werewolf.Domain.Objects;

namespace Was.Gaas.Werewolf.Application.UseCases
{
    public class CreateGameRequest
    {
        public ulong DiscordVoiceChannelId { get; set; }
    }

    public class CreateGameUseCase : UseCase<CreateGameRequest, GameCreatedEvent>
    {
        private readonly static object _lock = new();

        public CreateGameUseCase(IRepository repository, IEventPublisher eventPublisher) : base(repository, eventPublisher)
        {
        }

        public override async Task ExecuteAsync(CreateGameRequest request, IPresenter<GameCreatedEvent> presenter)
        {
            Game game;

            lock (_lock)
            {
                // 查
                var anyExistingActiveGame = Repository.FindAll()
                    .Where(x => x.DiscordVoiceChannelId == request.DiscordVoiceChannelId)
                    .Where(x => x.IsEnded == false)
                    .Any();

                if (anyExistingActiveGame)
                {
                    throw new OneActiveGamePerChannelException();
                }

                // 改
                game = new Game(request.DiscordVoiceChannelId);

                // 存
                Repository.Save(game);
            }

            // 推
            var gameEvent = new GameCreatedEvent
            {
                GameId = game.Id,
                DiscordVoiceChannelId = game.DiscordVoiceChannelId,
            };
            await EventPublisher.PublishAsync(gameEvent);
            await presenter.PresentAsync(gameEvent);
        }
    }

}
