using Wsa.Gaas.Werewolf.Application.Common;
using Wsa.Gaas.Werewolf.Domain.Events;
using Wsa.Gaas.Werewolf.Domain.Exceptions;

namespace Wsa.Gaas.Werewolf.Application.UseCases
{
    public class WitchUsePoisonRequest
    {
        public ulong DiscordVoiceChannelId { get; set; }
        public ulong PlayerId { get; set; }
        public ulong TargetPlayerId { get; set; }
    }

    public class WitchUsePoisonUseCase : UseCase<WitchUsePoisonRequest, WitchUsePoisonEvent>
    {
        public WitchUsePoisonUseCase(IRepository repository, GameEventBus gameEventBus) : base(repository, gameEventBus)
        {
        }

        public override async Task ExecuteAsync(WitchUsePoisonRequest request, IPresenter<WitchUsePoisonEvent> presenter, CancellationToken cancellationToken = default)
        {
            // 查
            var game = Repository.FindByDiscordChannelId(request.DiscordVoiceChannelId);

            if (game == null)
            {
                throw new GameNotFoundException(request.DiscordVoiceChannelId);
            }

            // 改
            var events = game.WitchUsePoison(request.PlayerId, request.TargetPlayerId);

            // 存
            await Repository.SaveAsync(game);

            // 推
            await presenter.PresentAsync(events, cancellationToken);
        }
    }
}
