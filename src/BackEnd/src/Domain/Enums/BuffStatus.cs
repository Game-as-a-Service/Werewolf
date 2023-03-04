namespace Wsa.Gaas.Werewolf.Domain.Enums;

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