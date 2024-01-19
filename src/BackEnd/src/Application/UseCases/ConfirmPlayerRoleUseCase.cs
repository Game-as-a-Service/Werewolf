namespace Wsa.Gaas.Werewolf.Application.UseCases;
public class ConfirmPlayerRoleRequest
{
    public ulong DiscordVoiceChannelId { get; set; }
    public ulong PlayerId { get; set; }
}

public record ConfirmPlayerRoleResponse(string GameId, string PlayerId, string Role);

public class ConfirmPlayerRoleUseCase : UseCase<ConfirmPlayerRoleRequest, ConfirmPlayerRoleResponse>
{
    public ConfirmPlayerRoleUseCase(IRepository repository, GameEventBus gameEventBus) : base(repository, gameEventBus)
    {
    }

    public async override Task<ConfirmPlayerRoleResponse> ExecuteAsync(ConfirmPlayerRoleRequest request, CancellationToken cancellationToken = default)
    {
        // Query
        var game = await Repository.FindByDiscordChannelIdAsync(request.DiscordVoiceChannelId);

        if (game == null)
        {
            throw new GameNotFoundException(request.DiscordVoiceChannelId);
        }

        // Update (Query)
        var gameEvent = game.ConfirmPlayerRole(request.PlayerId);

        // Save

        // Push
        return new ConfirmPlayerRoleResponse(
            gameEvent.Data.DiscordVoiceChannelId.ToString(),
            gameEvent.PlayerId.ToString(),
            gameEvent.Role
        );
    }
}
