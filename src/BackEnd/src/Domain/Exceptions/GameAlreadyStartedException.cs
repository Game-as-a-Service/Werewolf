namespace Wsa.Gaas.Werewolf.Domain.Exceptions
{
    internal class GameAlreadyStartedException : Exception
    {
        public GameAlreadyStartedException() : base("Game already started")
        {
        }
    }
}