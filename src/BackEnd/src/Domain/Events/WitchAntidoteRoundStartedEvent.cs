using Wsa.Gaas.Werewolf.Domain.Objects;

namespace Wsa.Gaas.Werewolf.Domain.Events;

using Wsa.Gaas.Werewolf.Domain.Common;

public class WitchAntidoteRoundStartedEvent : GameEvent
{
    public WitchAntidoteRoundStartedEvent(Game data) : base(data)
    {
    }
}
