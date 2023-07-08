namespace Wsa.Gaas.Werewolf.Domain.Exceptions
{
    public class GameChannelException : GameException
    {
        public GameChannelException()
            : base("Only one active game per voice channel.")
        {
        }
    }
}
