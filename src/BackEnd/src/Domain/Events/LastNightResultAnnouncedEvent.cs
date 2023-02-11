using Wsa.Gaas.Werewolf.Domain.Objects;

namespace Wsa.Gaas.Werewolf.Domain.Events;

public class LastNightResultAnnouncedEvent : GameEvent
{
    public LastNightResultAnnouncedEvent(Game data) : base(data) { }
}