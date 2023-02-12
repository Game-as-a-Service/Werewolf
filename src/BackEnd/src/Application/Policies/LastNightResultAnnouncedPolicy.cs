using Wsa.Gaas.Werewolf.Application.Common;
using Wsa.Gaas.Werewolf.Domain.Events;
using Wsa.Gaas.Werewolf.Domain.Exceptions;

namespace Wsa.Gaas.Werewolf.Application.Policies;

internal class LastNightResultAnnouncedPolicy : Policy<LastNightResultAnnouncedEvent>
{
    public LastNightResultAnnouncedPolicy(IRepository repository, GameEventBus eventPublisher) : base(repository, eventPublisher) { }

    public override async Task ExecuteAsync(LastNightResultAnnouncedEvent request, CancellationToken cancellationToken = default)
    {
        // Query
        var game = await Repository.FindByDiscordChannelIdAsync(request.Data.DiscordVoiceChannelId);

        if (game == null)
            throw new GameNotFoundException(request.Data.DiscordVoiceChannelId);

        game.StartPlayerSpeaking();

        await Repository.SaveAsync(game);

        // Push
        await EventPublisher.BroadcastAsync(new PlayerSpeakingEvent(game), cancellationToken);
    }
}