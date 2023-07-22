using Wsa.Gaas.Werewolf.Application.Common;
using Wsa.Gaas.Werewolf.Domain.Events;
using Wsa.Gaas.Werewolf.Domain.Exceptions;

namespace Wsa.Gaas.Werewolf.Application.UseCases
{
    public class WerewolfVoteRequest
    {
        public ulong DiscordChannelId { get; set; }
        public ulong CallerId { get; set; }
        public ulong TargetId { get; set; }
    }

    public class WerewolfVoteUseCase : UseCase<WerewolfVoteRequest, WerewolfVotedEvent>
    {
        public WerewolfVoteUseCase(IRepository repository, GameEventBus gameEventBus) : base(repository, gameEventBus)
        {
        }

        public override async Task ExecuteAsync(WerewolfVoteRequest request, IPresenter<WerewolfVotedEvent> presenter, CancellationToken cancellationToken = default)
        {
            // 查
            var game = await Repository.FindByDiscordChannelIdAsync(request.DiscordChannelId) ?? throw new GameNotFoundException(request.DiscordChannelId);

            // 改 => Vote
            var events = game.WerewolfVote(request.CallerId, request.TargetId);

            // 存
            await Repository.SaveAsync(game);

            // 推 => SignalR
            //await GameEventBus.BroadcastAsync(events, cancellationToken);

            // 推 => Restful API
            await presenter.PresentAsync(events, cancellationToken);

        }
    }
}
