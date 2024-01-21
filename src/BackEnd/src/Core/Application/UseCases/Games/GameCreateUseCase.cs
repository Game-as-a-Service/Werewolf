using Wsa.Gaas.Werewolf.Application.Dtos;
using Wsa.Gaas.Werewolf.Domain.Objects;

namespace Wsa.Gaas.Werewolf.Application.UseCases.Games;
public class GameCreateRequest
{
    public ulong DiscordVoiceChannelId { get; set; }
}

public class GameCreateResponse
{
    public GameCreateResponse(GameEvent gameEvent)
    {
        Id = gameEvent.Data.DiscordVoiceChannelId;
        Players = gameEvent.Data.Players.Select(p => new PlayerDto
        {
            UserId = p.UserId,
            Role = p.Role.Name,
            PlayerNumber = p.PlayerNumber
        }).ToList();
        Status = gameEvent.Data.Status;
    }

    public ulong Id { get; set; }
    public List<PlayerDto> Players { get; set; } = new List<PlayerDto>();
    public GameStatus Status { get; set; }
}

public class GameCreateUseCase : UseCase<GameCreateRequest, GameCreateResponse>
{
    private readonly static object _lock = new();

    public GameCreateUseCase(IRepository repository, GameEventBus eventPublisher) : base(repository, eventPublisher)
    {
    }

    public override async Task<GameCreateResponse> ExecuteAsync(GameCreateRequest request, CancellationToken cancellationToken = default)
    {
        Game? game;

        lock (_lock)
        {
            // Query
            game = Repository.FindByDiscordChannelId(request.DiscordVoiceChannelId);

            if (game != null)
            {
                throw new GameChannelException();
            }

            // Update
            game = new Game(request.DiscordVoiceChannelId);

            // Save
            Repository.Save(game);
        }

        // Push
        var gameEvent = new GameCreatedEvent(game);

        // SignalR
        await GameEventBus.BroadcastAsync(new[] { gameEvent }, cancellationToken);

        // Restful API
        return new GameCreateResponse(gameEvent);
    }
}
