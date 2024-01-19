using Wsa.Gaas.Werewolf.Domain.Objects;

namespace Wsa.Gaas.Werewolf.Domain.Events;

using Wsa.Gaas.Werewolf.Domain.Common;

public class LastNightResultAnnouncedEvent : GameEvent
{
    public LastNightResultAnnouncedEvent(Game data) : base(data) { }
}