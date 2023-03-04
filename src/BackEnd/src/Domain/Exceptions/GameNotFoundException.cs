namespace Wsa.Gaas.Werewolf.Domain.Exceptions
{
    public class GameNotFoundException : Exception
    {
        public GameNotFoundException(long roomId) : base($"No active Game found in voice channel #{roomId}.")
        {
        }
    }
}
