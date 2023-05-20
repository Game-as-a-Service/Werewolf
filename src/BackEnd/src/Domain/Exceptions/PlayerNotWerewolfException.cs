using System.Runtime.Serialization;

namespace Wsa.Gaas.Werewolf.Domain.Exceptions
{
    public class PlayerNotWerewolfException : Exception
    {
        public PlayerNotWerewolfException(string? message) : base(message)
        {
        }
    }
}