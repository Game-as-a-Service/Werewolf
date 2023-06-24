namespace Wsa.Gaas.Werewolf.DiscordBot.Dtos;

public class GameDto
{
    public required string Id { get; set; }
    public required string Status { get; set; }
    public List<PlayerDto> Players { get; set; } = new();
    public List<RoundDto> Rounds { get; set; } = new();

}
