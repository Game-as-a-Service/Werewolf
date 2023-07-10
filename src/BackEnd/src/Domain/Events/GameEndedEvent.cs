using Wsa.Gaas.Werewolf.Domain.Objects;

namespace Wsa.Gaas.Werewolf.Domain.Events
{
    public class GameEndedEvent : GameEvent
    {
        public GameEndedEvent(Game data) : base(data)
        {
        }
    }
}
