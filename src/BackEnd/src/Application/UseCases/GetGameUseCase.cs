using Wsa.Gaas.Werewolf.Application.Common;
using Wsa.Gaas.Werewolf.Domain.Events;
using Wsa.Gaas.Werewolf.Domain.Exceptions;

namespace Wsa.Gaas.Werewolf.Application.UseCases
{
    public class GetGameRequest
    {
        public ulong DiscordVoiceChannelId { get; set; }
    }

    public class GetGamesUseCase : UseCase<GetGameRequest, GetGameEvent>
    {
        public GetGamesUseCase(IRepository repository, GameEventBus gameEventBus) : base(repository, gameEventBus)
        {
        }

        public override async Task ExecuteAsync(GetGameRequest request, IPresenter<GetGameEvent> presenter, CancellationToken cancellationToken = default)
        {
            // 查
            var game = await Repository.FindByDiscordChannelIdAsync(request.DiscordVoiceChannelId);

            if (game == null)
            {
                throw new GameNotFoundException(request.DiscordVoiceChannelId);
            }

            // 改 (這個 use case 沒有改)
            // 存 (這個 use case 沒有存)

            // 推
            var gameEvent = new GetGameEvent(game);
            await presenter.PresentAsync(gameEvent, cancellationToken);
        }
    }
}
