using Wsa.Gaas.Werewolf.Domain.Objects;

namespace Wsa.Gaas.Werewolf.Domain.Events;

using Wsa.Gaas.Werewolf.Domain.Common;

public class GameEndedEvent : GameEvent
{
    public GameEndedEvent(Game data) : base(data)
    {
    }
}
