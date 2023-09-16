using Microsoft.Extensions.Options;
using Wsa.Gaas.Werewolf.Application.Common;
using Wsa.Gaas.Werewolf.Application.Options;
using Wsa.Gaas.Werewolf.Domain.Events;
using Wsa.Gaas.Werewolf.Domain.Exceptions;

namespace Wsa.Gaas.Werewolf.Application.Policies
{
    internal class WitchPoisonRoundStartedEventPolicy : Policy<WitchPoisonRoundStartedEvent>
    {

        public WitchPoisonRoundStartedEventPolicy(IOptions<GameSettingOptions> options, IRepository repository, GameEventBus gameEventBus) : base(options, repository, gameEventBus)
        {
        }

        public async override Task Handle(WitchPoisonRoundStartedEvent gameEvent, CancellationToken cancellationToken = default)
        {
            // 15 seconds
            await Task.Delay(Options.WitchPoisonRoundTimer, cancellationToken);

            // 時間到 
            var game = Repository.FindByDiscordChannelId(gameEvent.Data.DiscordVoiceChannelId)
                ?? throw new GameNotFoundException(gameEvent.Data.DiscordVoiceChannelId)
                ;

            // 宣布昨晚結果
            var events = game.EndWitchPoisonRound();

            await GameEventBus.BroadcastAsync(events, cancellationToken);
        }
    }
}
