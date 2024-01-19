using Wsa.Gaas.Werewolf.Domain.Objects;

namespace Wsa.Gaas.Werewolf.Domain.Events;

using Wsa.Gaas.Werewolf.Domain.Common;

public class GameStartedEvent : GameEvent
{
    public GameStartedEvent(Game data) : base(data)
    {
    }
}
