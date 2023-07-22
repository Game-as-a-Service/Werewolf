using Wsa.Gaas.Werewolf.Application.Common;
using Wsa.Gaas.Werewolf.Domain.Events;
using Wsa.Gaas.Werewolf.Domain.Exceptions;

namespace Wsa.Gaas.Werewolf.Application.UseCases
{
    public class WitchUseAntidoteRequest
    {
        public ulong DiscordVoiceChannelId { get; set; }
        public ulong PlayerId { get; set; }
    }

    public class WitchUseAntidoteUseCase : UseCase<WitchUseAntidoteRequest, WitchUseAntidoteEvent>
    {
        public WitchUseAntidoteUseCase(IRepository repository, GameEventBus gameEventBus) : base(repository, gameEventBus)
        {
        }

        public override async Task ExecuteAsync(WitchUseAntidoteRequest request, IPresenter<WitchUseAntidoteEvent> presenter, CancellationToken cancellationToken = default)
        {
            // 查
            var game = Repository.FindByDiscordChannelId(request.DiscordVoiceChannelId);

            if (game == null)
            {
                throw new GameNotFoundException(request.DiscordVoiceChannelId);
            }

            // 改
            var events = game.WitchUseAntidote(request.PlayerId);

            // 存
            await Repository.SaveAsync(game);

            // 推
            await presenter.PresentAsync(events, cancellationToken);
        }
    }
}
