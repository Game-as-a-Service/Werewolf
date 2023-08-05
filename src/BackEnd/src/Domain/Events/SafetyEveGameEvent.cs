using Wsa.Gaas.Werewolf.Domain.Objects;

namespace Wsa.Gaas.Werewolf.Domain.Events;

public class SafetyEveGameEvent : GameEvent
{
    public SafetyEveGameEvent(Game data) : base(data) { }
}