namespace Wsa.Gaas.Werewolf.Application.Dtos;

public class PlayerDto
{
    public ulong UserId { get; set; }
    public int PlayerNumber { get; set; }
    public string Role { get; set; } = null!;
}
