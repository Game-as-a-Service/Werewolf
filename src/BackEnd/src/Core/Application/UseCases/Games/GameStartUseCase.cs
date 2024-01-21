using Wsa.Gaas.Werewolf.Application.Dtos;
using Wsa.Gaas.Werewolf.Domain.Objects;

namespace Wsa.Gaas.Werewolf.Application.UseCases.Games;
public class GameStartRequest
{
    public ulong DiscordVoiceChannelId { get; set; }

    public ulong[] Players { get; set; } = Array.Empty<ulong>();
}

public class GameStartResponse
{
    public GameStartResponse() { }
    public GameStartResponse(GameEvent gameEvent)
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

public class GameStartUseCase : UseCase<GameStartRequest, GameStartResponse>
{
    private readonly static object _lock = new();

    public GameStartUseCase(IRepository repository, GameEventBus eventPublisher) : base(repository, eventPublisher)
    {
    }

    public override async Task<GameStartResponse> ExecuteAsync(GameStartRequest request, CancellationToken cancellationToken = default)
    {
        Game? game;
        IEnumerable<GameEvent> events;

        lock (_lock)
        {
            // 查
            game = Repository.FindByDiscordChannelId(request.DiscordVoiceChannelId);

            if (game == null)
            {
                throw new GameNotFoundException(request.DiscordVoiceChannelId);
            }

            // 改
            events = game.StartGame(request.Players);

            // 存
            Repository.Save(game);
        }

        // SignalR
        await GameEventBus.BroadcastAsync(events, cancellationToken);

        // Restful API
        return new GameStartResponse(events.First());
    }
}
