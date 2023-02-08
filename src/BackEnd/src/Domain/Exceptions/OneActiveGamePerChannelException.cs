namespace Wsa.Gaas.Werewolf.Domain.Exceptions
{
    public class OneActiveGamePerChannelException : Exception
    {
        public OneActiveGamePerChannelException()
            : base("Only one active game per voice channel.")
        {
        }
    }
}
