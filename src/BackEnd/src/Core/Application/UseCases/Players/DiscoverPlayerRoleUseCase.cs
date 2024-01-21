namespace Wsa.Gaas.Werewolf.Application.UseCases.Players;
public class DiscoverPlayerRoleRequest
{
    public ulong DiscordVoiceChannelId { get; set; }
    public ulong PlayerId { get; set; }
    public int DiscoverPlayerNumber { get; set; }
}

public record DiscoverPlayerRoleResponse(string GameId, string PlayerId, int DiscoveredPlayerNumber, string DiscoveredRoleFaction);

public class DiscoverPlayerRoleUseCase : UseCase<DiscoverPlayerRoleRequest, DiscoverPlayerRoleResponse>
{
    public DiscoverPlayerRoleUseCase(IRepository repository, GameEventBus gameEventBus) : base(repository, gameEventBus)
    {
    }

    public override async Task<DiscoverPlayerRoleResponse> ExecuteAsync(DiscoverPlayerRoleRequest request, CancellationToken cancellationToken = default)
    {
        // Query
        var game = await Repository.FindByDiscordChannelIdAsync(request.DiscordVoiceChannelId);

        if (game == null)
        {
            throw new GameNotFoundException(request.DiscordVoiceChannelId);
        }

        // Update
        var gameEvent = game.DiscoverPlayerRole(request.PlayerId, request.DiscoverPlayerNumber);

        // Save

        // Push
        return new DiscoverPlayerRoleResponse(
            gameEvent.Data.DiscordVoiceChannelId.ToString(),
            gameEvent.PlayerId.ToString(),
            gameEvent.DiscoveredPlayerNumber,
            gameEvent.DiscoveredRoleFaction.ToString()
        );
    }
}
