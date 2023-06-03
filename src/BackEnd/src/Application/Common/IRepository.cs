using Wsa.Gaas.Werewolf.Domain.Objects;

namespace Wsa.Gaas.Werewolf.Application.Common
{
    public interface IRepository
    {
        // 讀
        Task<Game?> FindByIdAsync(Guid id);
        Task<Game?> FindByDiscordChannelIdAsync(ulong discordChannelId);
        IQueryable<Game> FindAll();

        // 存
        Task SaveAsync(Game game);
        void Save(Game game);


        void Initialize();
    }
}
