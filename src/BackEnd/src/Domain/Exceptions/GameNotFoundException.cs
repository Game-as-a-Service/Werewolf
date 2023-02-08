namespace Wsa.Gaas.Werewolf.Domain.Exceptions
{
    public class GameNotFoundException : Exception
    {
        public GameNotFoundException(Guid id) : base($"Game Id (${id}) not found.")
        { 
        }
    }
}
