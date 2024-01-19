using Wsa.Gaas.Werewolf.Domain.Objects;

namespace Wsa.Gaas.Werewolf.Domain.Events;

using Wsa.Gaas.Werewolf.Domain.Common;

public class SafetyEveGameEvent : GameEvent
{
    public SafetyEveGameEvent(Game data) : base(data) { }
}