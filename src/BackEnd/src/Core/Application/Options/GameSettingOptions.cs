namespace Wsa.Gaas.Werewolf.Application.Options;
public class GameSettingOptions
{
    public TimeSpan PlayerRoleConfirmationTimer { get; set; }
    public TimeSpan WerewolfRoundTimer { get; set; }
    public TimeSpan SeerRoundTimer { get; set; }
    public TimeSpan WitchAntidoteRoundTimer { get; set; }
    public TimeSpan WitchPoisonRoundTimer { get; set; }

}
