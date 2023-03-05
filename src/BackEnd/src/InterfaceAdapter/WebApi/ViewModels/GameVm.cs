using Wsa.Gaas.Werewolf.Domain.Entities;

namespace Wsa.Gaas.Werewolf.WebApi.ViewModels
{
    public class GameVm
    {
        public string Id { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;

        public static GameVm FromDomain(Game game)
        {
            return new GameVm
            {
                Id = game.RoomId.ToString(),
                Status = game.Status.ToString(),
            };
        }
    }
}
