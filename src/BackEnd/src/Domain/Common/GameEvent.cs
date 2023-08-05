using Wsa.Gaas.Werewolf.Domain.Objects;

namespace Wsa.Gaas.Werewolf.Domain.Common;

public class GameEvent
{
    public GameEvent(Game data)
    {
        Data = data;
    }

    public Game Data { get; init; }
    public DateTimeOffset TriggeredOn { get; } = DateTimeOffset.UtcNow;
}
