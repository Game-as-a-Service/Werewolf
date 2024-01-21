namespace Wsa.Gaas.Werewolf.Application.UseCases.Players;
public class PlayerGetRoleRequest
{
    public ulong DiscordVoiceChannelId { get; set; }
    public ulong PlayerId { get; set; }
}

public record PlayerGetRoleResponse(string GameId, string PlayerId, string Role);

public class PlayerGetRoleUseCase : UseCase<PlayerGetRoleRequest, PlayerGetRoleResponse>
{
    public PlayerGetRoleUseCase(IRepository repository, GameEventBus gameEventBus) : base(repository, gameEventBus)
    {
    }

    public async override Task<PlayerGetRoleResponse> ExecuteAsync(PlayerGetRoleRequest request, CancellationToken cancellationToken = default)
    {
        // Query
        var game = await Repository.FindByDiscordChannelIdAsync(request.DiscordVoiceChannelId) 
            ?? throw new GameNotFoundException(request.DiscordVoiceChannelId);

        // Update (Query)
        var gameEvent = game.PlayerGetRole(request.PlayerId);

        // Push
        return new PlayerGetRoleResponse(
            gameEvent.Data.DiscordVoiceChannelId.ToString(),
            gameEvent.PlayerId.ToString(),
            gameEvent.Role
        );
    }
}
