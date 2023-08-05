using Wsa.Gaas.Werewolf.Domain.Objects;

namespace Wsa.Gaas.Werewolf.Domain.Events;

public class PlayerRoleConfirmationStartedEvent : GameEvent
{
    public PlayerRoleConfirmationStartedEvent(Game data) : base(data)
    {
    }
}
