using Wsa.Gaas.Werewolf.Domain.Common;
using Wsa.Gaas.Werewolf.Domain.Objects;

namespace Wsa.Gaas.Werewolf.Domain.Events;
public class WitchPoisonUsedEvent : GameEvent
{
    public WitchPoisonUsedEvent(Game data) : base(data)
    {
    }
}

