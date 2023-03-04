namespace Wsa.Gaas.Werewolf.Domain.Objects
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
