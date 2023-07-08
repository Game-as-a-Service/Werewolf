namespace Wsa.Gaas.Werewolf.Domain.Exceptions
{
    internal class PlayersDuplicatedException : GameException
    {
        public PlayersDuplicatedException() : base("Duplicate player id found")
        {
        }
    }
}