using Wsa.Gaas.Werewolf.Domain.Entities;

namespace Wsa.Gaas.Werewolf.Domain.Events
{
    public class PlayerRoleConfirmedEvent : GameEvent
    {
        public long PlayerId { get; init; }
        public string Role { get; init; } = string.Empty;

        public PlayerRoleConfirmedEvent(Game data) : base(data)
        {
        }
    }
}
