using Wsa.Gaas.Werewolf.Domain.Common;
using Wsa.Gaas.Werewolf.Domain.Objects;

namespace Wsa.Gaas.Werewolf.Domain.Events;
public class WitchPoisonRoundStartedEvent : GameEvent
{
    public WitchPoisonRoundStartedEvent(Game data) : base(data)
    {
    }
}

