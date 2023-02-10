namespace Wsa.Gaas.Werewolf.Domain.Exceptions
{
    internal class PlayersDuplicatedException : Exception
    {
        public PlayersDuplicatedException() : base("Duplicate player id found")
        {
        }
    }
}