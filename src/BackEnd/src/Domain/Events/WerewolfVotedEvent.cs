using Wsa.Gaas.Werewolf.Domain.Objects;

namespace Wsa.Gaas.Werewolf.Domain.Events;

using Wsa.Gaas.Werewolf.Domain.Common;

public class WerewolfVotedEvent : GameEvent
{
    public WerewolfVotedEvent(Game data) : base(data)
    {
    }
}
