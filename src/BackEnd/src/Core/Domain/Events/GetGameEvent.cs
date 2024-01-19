using Wsa.Gaas.Werewolf.Domain.Objects;

namespace Wsa.Gaas.Werewolf.Domain.Events;

using Wsa.Gaas.Werewolf.Domain.Common;

public class GetGameEvent : GameEvent
{
    public GetGameEvent(Game data) : base(data)
    {
    }
}
