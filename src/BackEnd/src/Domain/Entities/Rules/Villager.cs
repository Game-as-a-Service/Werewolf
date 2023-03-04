using Wsa.Gaas.Werewolf.Domain.Enums;

namespace Wsa.Gaas.Werewolf.Domain.Entities.Rules;

public class Villager : Role
{
    public Villager()
    {
        Id = 3;
        Name = GetType().Name;
        Faction = Faction.Alliance;
    }
}