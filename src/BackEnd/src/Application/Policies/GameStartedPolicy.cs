using Wsa.Gaas.Werewolf.Application.Common;
using Wsa.Gaas.Werewolf.Domain.Events;
using Wsa.Gaas.Werewolf.Domain.Exceptions;

namespace Wsa.Gaas.Werewolf.Application.Policies
{
    internal class GameStartedPolicy : Policy<GameStartedEvent>
    {
        public GameStartedPolicy(IRepository repository, GameEventBus eventPublisher) : base(repository, eventPublisher)
        {
        }

        public override async Task ExecuteAsync(GameStartedEvent request, CancellationToken cancellationToken = default)
        {
            // Query
            var game = await Repository.FindByDiscordChannelIdAsync(request.Data.DiscordVoiceChannelId);

            if (game == null)
            {
                throw new GameNotFoundException(request.Data.DiscordVoiceChannelId);
            }

            // Update
            game.StartPlayerRoleConfirmation();

            // Save
            Repository.Save(game);

            // Push
            await EventPublisher.BroadcastAsync(new PlayerRoleConfirmationStartedEvent(game), cancellationToken); ;
        }
    }
}
