namespace Wsa.Gaas.Werewolf.DiscordBot.Dtos;

public class PlayerDto
{
    public required ulong UserId { get; set; }
    public required int PlayerNumber { get; set; }
    public required string Role { get; set; }
}
