using Wsa.Gaas.Werewolf.Domain.Objects;

namespace Wsa.Gaas.Werewolf.Domain.Events
{
    public class SeerEyesOpenedEvent : GameEvent
    {
        public SeerEyesOpenedEvent(Game data) : base(data)
        {
        }
    }
}
