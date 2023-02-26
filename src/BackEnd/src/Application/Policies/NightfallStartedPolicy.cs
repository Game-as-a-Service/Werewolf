using Wsa.Gaas.Werewolf.Application.Common;
using Wsa.Gaas.Werewolf.Domain.Events;
using Wsa.Gaas.Werewolf.Domain.Exceptions;

namespace Wsa.Gaas.Werewolf.Application.Policies;

internal class NightfallStartedPolicy : Policy<NightfallStartedEvent>
{
    public NightfallStartedPolicy(IRepository repository, GameEventBus gameEventBus) : base(repository, gameEventBus)
    {
    }

    public async override Task ExecuteAsync(NightfallStartedEvent request, CancellationToken cancellationToken = default)
    {
        // Query
        var game = await Repository.FindByDiscordChannelIdAsync(request.Data.DiscordVoiceChannelId);

        if (game == null)
            throw new GameNotFoundException(request.Data.DiscordVoiceChannelId);

        WereWolvesRoundStartedEvent gameEvent = game.StartWereWolvesRound();


    }
}
