using Wsa.Gaas.Werewolf.Domain.Objects;

namespace Wsa.Gaas.Werewolf.Application.Common
{
    public interface IRepository
    {
        Task<Game?> FindByIdAsync(Guid id);

        Task<Game?> FindByDiscordChannelIdAsync(ulong discordChannelId);

        IQueryable<Game> FindAll();

        Task SaveAsync(Game game);

        void Save(Game game);
    }
}
