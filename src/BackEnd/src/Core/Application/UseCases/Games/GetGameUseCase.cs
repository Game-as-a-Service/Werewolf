using Wsa.Gaas.Werewolf.Application.Dtos;
using Wsa.Gaas.Werewolf.Domain.Objects;

namespace Wsa.Gaas.Werewolf.Application.UseCases.Games;
public class GameGetRequest
{
    public ulong DiscordVoiceChannelId { get; set; }
}

public class GameGetResponse
{
    public GameGetResponse()
    {

    }

    public GameGetResponse(GameEvent gameEvent)
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

public class GamesGetUseCase : UseCase<GameGetRequest, GameGetResponse>
{
    public GamesGetUseCase(IRepository repository, GameEventBus gameEventBus) : base(repository, gameEventBus)
    {
    }

    public override async Task<GameGetResponse> ExecuteAsync(GameGetRequest request, CancellationToken cancellationToken = default)
    {
        // 查
        var game = await Repository.FindByDiscordChannelIdAsync(request.DiscordVoiceChannelId);

        if (game == null)
        {
            throw new GameNotFoundException(request.DiscordVoiceChannelId);
        }

        // 改 (這個 use case 沒有改)
        // 存 (這個 use case 沒有存)

        // 推
        var gameEvent = new GetGameEvent(game);

        return new GameGetResponse(gameEvent);
    }
}
