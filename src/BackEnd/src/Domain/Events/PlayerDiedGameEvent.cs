using Wsa.Gaas.Werewolf.Domain.Objects;

namespace Wsa.Gaas.Werewolf.Domain.Events;

public class PlayerDiedGameEvent : GameEvent
{
    public PlayerDiedGameEvent(Game data) : base(data)
    {
    }
}
