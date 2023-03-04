using Wsa.Gaas.Werewolf.Domain.Enums;

namespace Wsa.Gaas.Werewolf.Domain.Entities.Rules;

public abstract class Role
{
    public int Id { get; internal set; }
    public string Name { get; internal set; } = string.Empty;
    public Faction Faction { get; internal set; }

}