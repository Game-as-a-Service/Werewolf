using Wsa.Gaas.Werewolf.Domain.Objects.Roles;

namespace Wsa.Gaas.Werewolf.Domain.Objects
{
    public class Player
    {
        public Guid Id { get; init; }
        public ulong UserId { get; init; }
        public int PlayerNumber { get; internal set; }
        public Role Role { get; internal set; }
        public BuffStatus BuffStatus { get; internal set; }
        public bool IsDead { get; internal set; }
        public bool IsAntidoteUsed { get; internal set; }

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        private Player() { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

        public Player(ulong userId, int playerNumber, Role role)
        {
            UserId = userId;
            PlayerNumber = playerNumber;
            Role = role;
        }

        public bool IsWerewolf()
        {
            return Role is Roles.Werewolf or AlphaWerewolf;
        }
    }

    [Flags]
    public enum BuffStatus
    {
        None = 0,
        KilledByWerewolf = 1,
        SavedByWitch = 2,
        KilledByWitch = 4,
        SavedByGuardian = 8,
        VotedByMajority = 16,
    }
}
