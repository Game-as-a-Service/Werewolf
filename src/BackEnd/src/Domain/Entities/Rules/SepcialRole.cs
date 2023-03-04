using Wsa.Gaas.Werewolf.Domain.Enums;

namespace Wsa.Gaas.Werewolf.Domain.Entities.Rules;

public abstract class SepcialRole : Role
{
    public SepcialRole()
    {
        Name = GetType().Name;
        Faction = Faction.Alliance;
    }
}