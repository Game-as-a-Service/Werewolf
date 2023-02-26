using Wsa.Gaas.Werewolf.Application.Common;
using Wsa.Gaas.Werewolf.Domain.Events;

namespace Wsa.Gaas.Werewolf.Application.Policies;

internal class PlayerRoleConfirmationStartedPolicy : Policy<PlayerRoleConfirmationStartedEvent>
{
    public PlayerRoleConfirmationStartedPolicy(IRepository repository, GameEventBus eventPublisher) : base(repository, eventPublisher)
    {
    }

    public async override Task ExecuteAsync(PlayerRoleConfirmationStartedEvent request, CancellationToken cancellationToken = default)
    {
        // TODO: This will be a bottleneck during testing
        await Task.Delay(TimeSpan.FromSeconds(60), cancellationToken);

        var game = await Repository.FindByDiscordChannelIdAsync(request.Data.DiscordVoiceChannelId);

        if(game != null)
        {
            PlayerRoleConfirmationEndedEvent gameEvent = game.EndPlayerRoleConfirmation();

            Repository.Save(game);

            await GameEventBus.BroadcastAsync(gameEvent, cancellationToken);
        }
    }
}
