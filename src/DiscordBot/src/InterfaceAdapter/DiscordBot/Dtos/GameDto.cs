namespace Wsa.Gaas.Werewolf.DiscordBot.Dtos;

public class GameDto
{
    public ulong Id { get; set; }
    public GameStatus Status { get; set; }
    public List<PlayerDto> Players { get; set; } = new();
    public List<RoundDto> Rounds { get; set; } = new();
}
