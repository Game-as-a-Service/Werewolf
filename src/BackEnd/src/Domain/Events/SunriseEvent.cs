using Wsa.Gaas.Werewolf.Domain.Objects;

namespace Domain.Events
{
    public class SunriseEvent : GameEvent
    {
        public SunriseEvent(Game data) : base(data)
        {
        }
    }
}