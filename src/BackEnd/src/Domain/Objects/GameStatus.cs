namespace Wsa.Gaas.Werewolf.Domain.Objects
{
    public enum GameStatus
    {
        Created,
        Started,
        PlayerRoleConfirmationStarted,
        PlayerRoleConfirmationEnded,

        NightfallStarted,
        WereWolvesRoundStarted,

        LastNightResultAnnounced,
        PlayerSpeaking,


        Ended,
        
    }
}