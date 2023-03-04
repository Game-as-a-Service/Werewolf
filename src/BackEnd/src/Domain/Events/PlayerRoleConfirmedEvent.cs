using Wsa.Gaas.Werewolf.Domain.Entities;

namespace Wsa.Gaas.Werewolf.Domain.Events
{
    public class PlayerRoleConfirmedEvent : GameEvent
    {
        public long PlayerId { get; set; }
        public string Role { get; set; } = string.Empty;

        public PlayerRoleConfirmedEvent(Game data) : base(data)
        {
        }
    }
}
