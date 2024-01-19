using Wsa.Gaas.Werewolf.Domain.Objects;

namespace Wsa.Gaas.Werewolf.Domain.Events;

using Wsa.Gaas.Werewolf.Domain.Common;

public class GameCreatedEvent : GameEvent
{
    public GameCreatedEvent(Game data) : base(data)
    {
    }
}
