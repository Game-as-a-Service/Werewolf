using Wsa.Gaas.Werewolf.Domain.Objects;

namespace Wsa.Gaas.Werewolf.Application.UseCases;
public class EndGameRequest
{
    public ulong DiscordVoiceChannelId { get; set; }
}

public record EndGameResponse();

public class EndGameUseCase : UseCase<EndGameRequest, EndGameResponse>
{
    private readonly static object _lock = new();

    public EndGameUseCase(IRepository repository, GameEventBus gameEventBus) : base(repository, gameEventBus)
    {
    }

    public override Task<EndGameResponse> ExecuteAsync(EndGameRequest request, CancellationToken cancellationToken = default)
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
        return Task.FromResult(new EndGameResponse());
    }
}
