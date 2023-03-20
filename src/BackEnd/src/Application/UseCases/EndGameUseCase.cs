using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wsa.Gaas.Werewolf.Application.Common;
using Wsa.Gaas.Werewolf.Domain.Common;
using Wsa.Gaas.Werewolf.Domain.Events;
using Wsa.Gaas.Werewolf.Domain.Exceptions;
using Wsa.Gaas.Werewolf.Domain.Objects;

namespace Wsa.Gaas.Werewolf.Application.UseCases
{
    public class EndGameRequest
    {
        public ulong DiscordVoiceChannelId { get; set; }
    }

    public class EndGameUseCase : UseCase<EndGameRequest, GameEndedEvent>
    {
        private readonly static object _lock = new();

        public EndGameUseCase(IRepository repository, GameEventBus gameEventBus) : base(repository, gameEventBus)
        {
        }

        public override async Task ExecuteAsync(EndGameRequest request, IPresenter<GameEndedEvent> presenter, CancellationToken cancellationToken = default)
        {
            Game? game;
            lock (_lock)
            { 
                // 查
                game = Repository.FindAll()
                    .Where(x => x.DiscordVoiceChannelId == request.DiscordVoiceChannelId)
                    //.Where(x => x.Status != GameStatus.Ended)
                    .FirstOrDefault();

                if (game == null)
                {
                    throw new GameNotFoundException(request.DiscordVoiceChannelId);
                }

                if (game.Status == GameStatus.Ended)
                {
                    throw new GameAlreadyEndedException(request.DiscordVoiceChannelId);
                }

                game.EndGame();

                Repository.Save(game);
            }

            // 推
            var gameEvent = new GameEndedEvent(game);

            // SignalR 中斷連線

            // Restful API
            await presenter.PresentAsync(gameEvent, cancellationToken);
        }
    }
}
