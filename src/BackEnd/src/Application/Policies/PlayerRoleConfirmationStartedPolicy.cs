using Microsoft.Extensions.Options;
using Wsa.Gaas.Werewolf.Application.Common;
using Wsa.Gaas.Werewolf.Application.Options;
using Wsa.Gaas.Werewolf.Domain.Events;
using Wsa.Gaas.Werewolf.Domain.Exceptions;

namespace Wsa.Gaas.Werewolf.Application.Policies;

internal class PlayerRoleConfirmationStartedPolicy : Policy<PlayerRoleConfirmationStartedEvent>
{
    public PlayerRoleConfirmationStartedPolicy(
        IOptions<GameSettingOptions> options,
        IRepository repository,
        GameEventBus gameEventBus
    ) : base(options, repository, gameEventBus)
    {
    }

    public override async Task Handle(PlayerRoleConfirmationStartedEvent gameEvent, CancellationToken cancellationToken = default)
    {
        // 30 seconds
        await Task.Delay(Options.PlayerRoleConfirmationTimer, cancellationToken);

        // 時間到 raise PlayerRoleConfirmationEndedEvent
        var game = Repository.FindByDiscordChannelId(gameEvent.Data.DiscordVoiceChannelId)
            ?? throw new GameNotFoundException(gameEvent.Data.DiscordVoiceChannelId)
            ;

        var @event = game.StartWerewolfRound();

        await GameEventBus.BroadcastAsync(@event, cancellationToken);
    }
}
