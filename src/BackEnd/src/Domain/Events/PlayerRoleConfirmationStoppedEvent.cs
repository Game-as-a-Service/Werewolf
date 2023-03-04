using Wsa.Gaas.Werewolf.Domain.Objects;

namespace Wsa.Gaas.Werewolf.Domain.Events;

public class PlayerRoleConfirmationStoppedEvent : GameEvent
{
    public PlayerRoleConfirmationStoppedEvent(Game data) : base(data)
    {
    }
}