using Wsa.Gaas.Werewolf.Application.Common;
using Wsa.Gaas.Werewolf.Domain.Events;
using Wsa.Gaas.Werewolf.Domain.Exceptions;

namespace Wsa.Gaas.Werewolf.Application.UseCases
{
    public class PlayerTriggerSkillRequest
    {
        public ulong DiscordVoiceChannelId { get; set; }
        public ulong PlayerId { get; set; }
        public ulong TargetPlayerId { get; set; }
    }

    public class PlayerTriggerSkillUseCase : UseCase<PlayerTriggerSkillRequest, PlayerTriggerSkillEvent>
    {
        public PlayerTriggerSkillUseCase(IRepository repository, GameEventBus gameEventBus) : base(repository, gameEventBus)
        {
        }

        public override async Task ExecuteAsync(PlayerTriggerSkillRequest request, IPresenter<PlayerTriggerSkillEvent> presenter, CancellationToken cancellationToken = default)
        {
            // 查
            var game = Repository.FindByDiscordChannelId(request.DiscordVoiceChannelId);
            
            if(game == null)
            {
                throw new GameNotFoundException(request.DiscordVoiceChannelId);
            }

            // 改
            var @event = game.TriggerPlayerSkill(
                request.PlayerId,
                request.TargetPlayerId
            );

            // 存
            await Repository.SaveAsync(game);

            // 推
            await presenter.PresentAsync(@event, cancellationToken);
            
        }
    }
}
