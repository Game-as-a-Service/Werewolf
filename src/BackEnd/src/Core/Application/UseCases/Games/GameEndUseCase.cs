using Wsa.Gaas.Werewolf.Domain.Objects;

namespace Wsa.Gaas.Werewolf.Application.UseCases.Games;
public class GameEndRequest
{
    public ulong DiscordVoiceChannelId { get; set; }
}

public record GameEndResponse();

public class GameEndUseCase : UseCase<GameEndRequest, GameEndResponse>
{
    private readonly static object _lock = new();

    public GameEndUseCase(IRepository repository, GameEventBus gameEventBus) : base(repository, gameEventBus)
    {
    }

    public override Task<GameEndResponse> ExecuteAsync(GameEndRequest request, CancellationToken cancellationToken = default)
    {
        Game? game;
        lock (_lock)
        {
            // 查
            game = Repository.FindByDiscordChannelId(request.DiscordVoiceChannelId);

            if (game == null)
            {
                throw new GameNotFoundException(request.DiscordVoiceChannelId);
            }

            game.EndGame();

            Repository.Save(game);
        }

        // 推
        var gameEvent = new GameEndedEvent(game);

        // SignalR 中斷連線

        // Restful API
        return Task.FromResult(new GameEndResponse());
    }
}
