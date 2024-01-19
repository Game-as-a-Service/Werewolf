using Wsa.Gaas.Werewolf.Domain.Objects;

namespace Wsa.Gaas.Werewolf.Domain.Events;

using Wsa.Gaas.Werewolf.Domain.Common;

public class WerewolfRoundStartedEvent : GameEvent
{
    public WerewolfRoundStartedEvent(Game data) : base(data)
    {
    }
}
