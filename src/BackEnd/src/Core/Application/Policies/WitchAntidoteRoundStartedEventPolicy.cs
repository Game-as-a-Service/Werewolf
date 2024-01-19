using Microsoft.Extensions.Options;
using Wsa.Gaas.Werewolf.Application.Options;

namespace Wsa.Gaas.Werewolf.Application.Policies;
internal class WitchAntidoteRoundStartedEventPolicy : Policy<WitchAntidoteRoundStartedEvent>
{
    public WitchAntidoteRoundStartedEventPolicy(IOptions<GameSettingOptions> options, IRepository repository, GameEventBus gameEventBus) : base(options, repository, gameEventBus)
    {
    }

    public override async Task Handle(WitchAntidoteRoundStartedEvent gameEvent, CancellationToken cancellationToken = default)
    {
        // 15 seconds
        await Task.Delay(Options.WitchAntidoteRoundTimer, cancellationToken);

        // 時間到 
        var game = Repository.FindByDiscordChannelId(gameEvent.Data.DiscordVoiceChannelId)
            ?? throw new GameNotFoundException(gameEvent.Data.DiscordVoiceChannelId)
            ;

        var @event = game.StartWitchPoisonRound();

        await GameEventBus.BroadcastAsync(@event, cancellationToken);

    }
}

