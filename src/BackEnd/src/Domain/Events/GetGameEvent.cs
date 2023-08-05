using Wsa.Gaas.Werewolf.Domain.Objects;

namespace Wsa.Gaas.Werewolf.Domain.Events;

public class GetGameEvent : GameEvent
{
    public GetGameEvent(Game data) : base(data)
    {
    }
}
