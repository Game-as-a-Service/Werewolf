using Wsa.Gaas.Werewolf.Domain.Objects;

namespace Wsa.Gaas.Werewolf.Domain.Events;

public class GameCreatedEvent : GameEvent
{
    public GameCreatedEvent(Game data) : base(data)
    {
    }
}
