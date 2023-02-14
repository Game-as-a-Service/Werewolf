using Wsa.Gaas.Werewolf.Domain.Objects;

namespace Wsa.Gaas.Werewolf.Domain.Events
{
    public class PlayerRoleConfirmedEvent : GameEvent
    {
        public ulong PlayerId { get; set; }
        public string Role { get; set; } = string.Empty;

        public PlayerRoleConfirmedEvent(Game data) : base(data)
        {
        }
    }
}
