using Wsa.Gaas.Werewolf.Domain.Objects;

namespace Wsa.Gaas.Werewolf.Domain.Events;

using Wsa.Gaas.Werewolf.Domain.Common;

public class PlayerRoleConfirmationStartedEvent : GameEvent
{
    public PlayerRoleConfirmationStartedEvent(Game data) : base(data)
    {
    }
}
