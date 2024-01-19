using Wsa.Gaas.Werewolf.Domain.Objects;

namespace Wsa.Gaas.Werewolf.Domain.Events;

using Wsa.Gaas.Werewolf.Domain.Common;

public class SeerDiscoveredEvent : GameEvent
{
    public ulong PlayerId { get; set; }
    public int DiscoveredPlayerNumber { get; set; }
    public Faction DiscoveredRoleFaction { get; set; }

    public SeerDiscoveredEvent(Game data) : base(data)
    {
    }
}
