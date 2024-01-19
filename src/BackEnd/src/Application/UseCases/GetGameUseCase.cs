using Wsa.Gaas.Werewolf.Application.Common;
using Wsa.Gaas.Werewolf.Application.Dtos;
using Wsa.Gaas.Werewolf.Domain.Common;
using Wsa.Gaas.Werewolf.Domain.Events;
using Wsa.Gaas.Werewolf.Domain.Exceptions;
using Wsa.Gaas.Werewolf.Domain.Objects;

namespace Wsa.Gaas.Werewolf.Application.UseCases;
public class GetGameRequest
{
    public ulong DiscordVoiceChannelId { get; set; }
}

public class GetGameResponse
{
    public GetGameResponse()
    {

    }

    public GetGameResponse(GameEvent gameEvent)
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

public class GetGamesUseCase : UseCase<GetGameRequest, GetGameResponse>
{
    public GetGamesUseCase(IRepository repository, GameEventBus gameEventBus) : base(repository, gameEventBus)
    {
    }

    public override async Task<GetGameResponse> ExecuteAsync(GetGameRequest request, CancellationToken cancellationToken = default)
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

        return new GetGameResponse(gameEvent);
    }
}
