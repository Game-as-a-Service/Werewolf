namespace Wsa.Gaas.Werewolf.Domain.Exceptions
{
    public class OneActiveGamePerChannelException : Exception
    {
        public OneActiveGamePerChannelException()
            : base("一個語音頻道只能有一個遊戲")
        {
        }
    }
}
