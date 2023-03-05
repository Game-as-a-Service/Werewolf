using Wsa.Gaas.Werewolf.Domain.Enums;

namespace Wsa.Gaas.Werewolf.Domain.Entities.Rules;

public abstract class SepcialRole : Role
{
    protected SepcialRole()
    {
        Name = GetType().Name;
        Faction = Faction.Alliance;
    }
}