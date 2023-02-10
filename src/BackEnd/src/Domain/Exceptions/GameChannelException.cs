namespace Wsa.Gaas.Werewolf.Domain.Exceptions
{
    public class GameChannelException : Exception
    {
        public GameChannelException()
            : base("Only one active game per voice channel.")
        {
        }
    }
}
