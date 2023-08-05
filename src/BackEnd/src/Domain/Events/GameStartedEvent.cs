using Wsa.Gaas.Werewolf.Domain.Objects;

namespace Wsa.Gaas.Werewolf.Domain.Events;

public class GameStartedEvent : GameEvent
{
    public GameStartedEvent(Game data) : base(data)
    {
    }
}
