using Wsa.Gaas.Werewolf.Domain.Entities;

namespace Wsa.Gaas.Werewolf.Domain.Events;

public class PlayerRoleConfirmationStoppedEvent : GameEvent
{
    public PlayerRoleConfirmationStoppedEvent(Game data) : base(data)
    {
    }
}