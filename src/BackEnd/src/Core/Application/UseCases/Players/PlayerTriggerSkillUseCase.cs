namespace Wsa.Gaas.Werewolf.Application.UseCases.Players;
public class PlayerTriggerSkillRequest
{
    public ulong DiscordVoiceChannelId { get; set; }
    public ulong PlayerId { get; set; }
    public ulong TargetPlayerId { get; set; }
}

public class PlayerTriggerSkillResponse { }
public class PlayerTriggerSkillUseCase : UseCase<PlayerTriggerSkillRequest, PlayerTriggerSkillResponse>
{
    public PlayerTriggerSkillUseCase(IRepository repository, GameEventBus gameEventBus) : base(repository, gameEventBus)
    {
    }

    public override async Task<PlayerTriggerSkillResponse> ExecuteAsync(PlayerTriggerSkillRequest request, CancellationToken cancellationToken = default)
    {
        // 查
        var game = Repository.FindByDiscordChannelId(request.DiscordVoiceChannelId);

        if (game == null)
        {
            throw new GameNotFoundException(request.DiscordVoiceChannelId);
        }

        // 改
        var @event = game.TriggerPlayerSkill(
            request.PlayerId,
            request.TargetPlayerId
        );

        // 存
        await Repository.SaveAsync(game);

        // 推
        return new PlayerTriggerSkillResponse
        {

        };

    }
}

