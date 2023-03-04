using Wsa.Gaas.Werewolf.Domain.Common;
using Wsa.Gaas.Werewolf.Domain.Events;
using Wsa.Gaas.Werewolf.Domain.Objects;

namespace Wsa.Gaas.Werewolf.WebApi.ViewModels
{
    public class GameVm
    {
        public string Id { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public List<PlayerVm> Players { get; set; } = new List<PlayerVm>();

        public static GameVm FromDomain(Game game)
        {
            return new GameVm
            {
                Id = game.DiscordVoiceChannelId.ToString(),
                Players = game.Players.Select(PlayerVm.FromDomain).ToList(),
                Status = game.Status.ToString(),
            };
        }
    }
}
