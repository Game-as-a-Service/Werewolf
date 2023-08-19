using Wsa.Gaas.Werewolf.Domain.Objects;

namespace Wsa.Gaas.Werewolf.Domain.Events;

public class SeerRoundStartedEvent : GameEvent
{
    public SeerRoundStartedEvent(Game data) : base(data)
    {
    }
}
