using Wsa.Gaas.Werewolf.Domain.Objects;

namespace Wsa.Gaas.Werewolf.Domain.Events;

public class WitchAntidoteUsedEvent : GameEvent
{
    public WitchAntidoteUsedEvent(Game data) : base(data)
    {
    }
}
