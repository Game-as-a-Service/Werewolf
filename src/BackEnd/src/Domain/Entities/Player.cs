using Wsa.Gaas.Werewolf.Domain.Entities.Rules;
using Wsa.Gaas.Werewolf.Domain.Enums;

namespace Wsa.Gaas.Werewolf.Domain.Entities
{
    public class Player
    {
        public long Id { get; init; }
        public int PlayerNumber { get; internal set; }
        public Role? Role { get; internal set; }
        public BuffStatus BuffStatus { get; internal set; }
        public bool IsDead { get; internal set; }

        public Player(long id)
        {
            Id = id;
        }

        internal void SetRole(Role role, int playerNumber)
        {
            Role = role;
            PlayerNumber = playerNumber;
        }
    }
}
