using Microsoft.Extensions.Options;
using Wsa.Gaas.Werewolf.Application.Common;
using Wsa.Gaas.Werewolf.Application.Options;
using Wsa.Gaas.Werewolf.Domain.Events;
using Wsa.Gaas.Werewolf.Domain.Exceptions;

namespace Wsa.Gaas.Werewolf.Application.Policies
{
    internal class SeerRoundStartedEventPolicy : Policy<SeerRoundStartedEvent>
    {
        public SeerRoundStartedEventPolicy(IOptions<GameSettingOptions> options, IRepository repository, GameEventBus gameEventBus) : base(options, repository, gameEventBus)
        {
        }

        public override async Task Handle(SeerRoundStartedEvent gameEvent, CancellationToken cancellationToken = default)
        {
            // 30 seconds
            await Task.Delay(Options.SeerRoundTimer, cancellationToken);

            // 時間到 raise PlayerRoleConfirmationEndedEvent
            var game = Repository.FindByDiscordChannelId(gameEvent.Data.DiscordVoiceChannelId)
                ?? throw new GameNotFoundException(gameEvent.Data.DiscordVoiceChannelId)
                ;

            var @event = game.StartWitchAntidoteRound();

            await GameEventBus.BroadcastAsync(@event, cancellationToken);
        }
    }
}
