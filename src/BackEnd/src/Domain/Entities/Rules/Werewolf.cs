using Wsa.Gaas.Werewolf.Domain.Enums;

namespace Wsa.Gaas.Werewolf.Domain.Entities.Rules;

public class Werewolf : Role
{
    public Werewolf()
    {
        Id = 1;
        Name = GetType().Name;
        Faction = Faction.Werewolf;
    }
}