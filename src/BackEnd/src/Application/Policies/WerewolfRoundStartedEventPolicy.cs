using Microsoft.Extensions.Options;
using Wsa.Gaas.Werewolf.Application.Common;
using Wsa.Gaas.Werewolf.Application.Options;
using Wsa.Gaas.Werewolf.Domain.Events;
using Wsa.Gaas.Werewolf.Domain.Exceptions;

namespace Wsa.Gaas.Werewolf.Application.Policies;
internal class WerewolfRoundStartedEventPolicy : Policy<WerewolfRoundStartedEvent>
{
    public WerewolfRoundStartedEventPolicy(
        IOptions<GameSettingOptions> options,
        IRepository repository,
        GameEventBus gameEventBus
    ) : base(options, repository, gameEventBus)
    {
    }

    public override async Task Handle(WerewolfRoundStartedEvent gameEvent, CancellationToken cancellationToken = default)
    {
        await Task.Delay(Options.WerewolfRoundTimer, cancellationToken);

        // 時間到 raise WerewolfRoundEndedEvent
        var game = Repository.FindByDiscordChannelId(gameEvent.Data.DiscordVoiceChannelId)
            ?? throw new GameNotFoundException(gameEvent.Data.DiscordVoiceChannelId)
            ;

        var @event = game.StartSeerRound();

        await GameEventBus.BroadcastAsync(@event, cancellationToken);
    }
}

