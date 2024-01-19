using Wsa.Gaas.Werewolf.Domain.Objects;

namespace Wsa.Gaas.Werewolf.Domain.Events;

using Wsa.Gaas.Werewolf.Domain.Common;

public class SeerEyesOpenedEvent : GameEvent
{
    public SeerEyesOpenedEvent(Game data) : base(data)
    {
    }
}
