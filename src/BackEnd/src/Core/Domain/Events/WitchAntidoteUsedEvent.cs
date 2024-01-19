using Wsa.Gaas.Werewolf.Domain.Objects;

namespace Wsa.Gaas.Werewolf.Domain.Events;

using Wsa.Gaas.Werewolf.Domain.Common;

public class WitchAntidoteUsedEvent : GameEvent
{
    public WitchAntidoteUsedEvent(Game data) : base(data)
    {
    }
}
