using Wsa.Gaas.Werewolf.Application.Common;
using Wsa.Gaas.Werewolf.Domain.Events;
using Wsa.Gaas.Werewolf.Domain.Exceptions;

namespace Wsa.Gaas.Werewolf.Application.Policies;

internal class GameStartedPolicy : Policy<GameStartedEvent>
{
    private readonly ITaskService _taskService;

    public GameStartedPolicy(IRepository repository, GameEventBus eventPublisher, ITaskService taskService) : base(repository, eventPublisher)
    {
        _taskService = taskService;
    }

    public override async Task ExecuteAsync(GameStartedEvent request, CancellationToken cancellationToken = default)
    {
        await _taskService.Delay(TimeSpan.FromSeconds(60));

        var game = await Repository.FindByDiscordChannelIdAsync(request.Data.DiscordVoiceChannelId);

        if (game == null)
            throw new GameNotFoundException(request.Data.DiscordVoiceChannelId);

        game.StopPlayerRoleConfirmation();

        await Repository.SaveAsync(game);

        // Push
        await EventPublisher.BroadcastAsync(new PlayerRoleConfirmationStoppedEvent(game), cancellationToken);
    }
}