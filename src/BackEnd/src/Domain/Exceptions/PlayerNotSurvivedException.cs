namespace Wsa.Gaas.Werewolf.Domain.Exceptions
{
    public class PlayerNotSurvivedException : Exception
    {
        public PlayerNotSurvivedException(int playerNumber) : base($"Player {playerNumber} not survived")
        {
        }
    }
}
