using Microsoft.Extensions.Options;
using Wsa.Gaas.Werewolf.Application.Common;
using Wsa.Gaas.Werewolf.Application.Options;
using Wsa.Gaas.Werewolf.Domain.Events;
using Wsa.Gaas.Werewolf.Domain.Exceptions;
using Wsa.Gaas.Werewolf.Domain.Objects;

namespace Wsa.Gaas.Werewolf.Application.Policies;

internal class PlayerRoleConfirmationStartedPolicy : Policy<PlayerRoleConfirmationStartedEvent>
{
    private readonly GameSettingOptions _options;
    private readonly IRepository _repository;
    private readonly GameEventBus _gameEventBus;

    public PlayerRoleConfirmationStartedPolicy(
        IOptions<GameSettingOptions> options,
        IRepository repository,
        GameEventBus gameEventBus
    )
    {
        _options = options.Value;
        _repository = repository;
        _gameEventBus = gameEventBus;
    }

    public override async Task Handle(PlayerRoleConfirmationStartedEvent gameEvent, CancellationToken cancellationToken = default)
    {
        await Task.Delay(_options.PlayerRoleConfirmation, cancellationToken);

        // 時間到 raise PlayerRoleConfirmationEndedEvent
        var game = _repository.FindByDiscordChannelId(gameEvent.Data.DiscordVoiceChannelId)
            ?? throw new GameNotFoundException(gameEvent.Data.DiscordVoiceChannelId)
            ;

        //

        var newGameEvent = new PlayerRoleConfirmationEndedEvent(game);

        await _gameEventBus.BroadcastAsync(newGameEvent, cancellationToken);
    }
}
