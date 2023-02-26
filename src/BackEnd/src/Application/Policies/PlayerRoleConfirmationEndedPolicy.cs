using Wsa.Gaas.Werewolf.Application.Common;
using Wsa.Gaas.Werewolf.Domain.Events;

namespace Wsa.Gaas.Werewolf.Application.Policies;

internal class PlayerRoleConfirmationEndedPolicy : Policy<PlayerRoleConfirmationEndedEvent>
{
    public PlayerRoleConfirmationEndedPolicy(IRepository repository, GameEventBus gameEventBus) : base(repository, gameEventBus)
    {
    }

    public async override Task ExecuteAsync(PlayerRoleConfirmationEndedEvent request, CancellationToken cancellationToken = default)
    {
        var game = await Repository.FindByDiscordChannelIdAsync(request.Data.DiscordVoiceChannelId);

        if (game != null)
        {
            NightfallStartedEvent gameEvent = game.StartNightfall();

            Repository.Save(game);

            await GameEventBus.BroadcastAsync(gameEvent, cancellationToken);
        }
    }
}
