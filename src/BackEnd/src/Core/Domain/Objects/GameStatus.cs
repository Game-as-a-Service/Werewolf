namespace Wsa.Gaas.Werewolf.Domain.Objects;
public enum GameStatus
{
    Created,
    Started,
    PlayerRoleConfirmationStarted,
    LastNightResultAnnounced,
    PlayerSpeaking,
    SeerRoundStarted,
    Ended,
    WitchAntidoteRoundStarted,
    WitchPoisonRoundStarted,
    NightEnded,
    WerewolfRoundStarted,
}