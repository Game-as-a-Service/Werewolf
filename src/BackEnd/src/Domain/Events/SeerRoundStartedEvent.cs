using Wsa.Gaas.Werewolf.Domain.Objects;

namespace Wsa.Gaas.Werewolf.Domain.Events;

using Wsa.Gaas.Werewolf.Domain.Common;

public class SeerRoundStartedEvent : GameEvent
{
    public SeerRoundStartedEvent(Game data) : base(data)
    {
    }
}
