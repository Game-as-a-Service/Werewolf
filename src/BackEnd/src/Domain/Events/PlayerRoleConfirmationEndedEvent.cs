using Wsa.Gaas.Werewolf.Domain.Objects;

namespace Wsa.Gaas.Werewolf.Domain.Events
{
    public class PlayerRoleConfirmationEndedEvent : GameEvent
    {
        public PlayerRoleConfirmationEndedEvent(Game data) : base(data)
        {
        }
    }
}
