using Wsa.Gaas.Werewolf.Domain.Objects;

namespace Wsa.Gaas.Werewolf.Domain.Events;

public class WerewolfRoundStartedEvent : GameEvent
{
    public WerewolfRoundStartedEvent(Game data) : base(data)
    {
    }
}
