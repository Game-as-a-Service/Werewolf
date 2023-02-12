using Domain.Events;
using Wsa.Gaas.Werewolf.Application.Common;
using Wsa.Gaas.Werewolf.Domain.Exceptions;

namespace Application.Policies
{
    public class SunrisePolicy : Policy<SunriseEvent>
    {
        public SunrisePolicy(IRepository repository, GameEventBus eventPublisher) : base(repository, eventPublisher)
        {
        }

        public override async Task ExecuteAsync(SunriseEvent request, CancellationToken cancellationToken = default)
        {
            // Query
            var game = await Repository.FindByDiscordChannelIdAsync(request.Data.DiscordVoiceChannelId);

            if (game == null)
            {
                throw new GameNotFoundException(request.Data.DiscordVoiceChannelId);
            }

            // Update
            game.StartSunrise();

            // Save
            Repository.Save(game);

            // Push
            await EventPublisher.BroadcastAsync(new SunriseEvent(game), cancellationToken);
        }
    }
}