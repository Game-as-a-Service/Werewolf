using Wsa.Gaas.Werewolf.Domain.Objects;

namespace Wsa.Gaas.Werewolf.Domain.Events;

public class NightEndedEvent : GameEvent
{
    public NightEndedEvent(Game data) : base(data)
    {
    }
}
