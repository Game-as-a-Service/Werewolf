using Wsa.Gaas.Werewolf.Domain.Objects;

namespace Wsa.Gaas.Werewolf.Domain.Events;

public class WitchAntidoteRoundStartedEvent : GameEvent
{
    public WitchAntidoteRoundStartedEvent(Game data) : base(data)
    {
    }
}
