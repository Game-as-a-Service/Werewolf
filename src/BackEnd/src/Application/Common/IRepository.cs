using Wsa.Gaas.Werewolf.Domain.Objects;

namespace Wsa.Gaas.Werewolf.Application.Common
{
    public interface IRepository
    {
        Task<Game?> FindByIdAsync(Guid id);

        Task<Game?> FindByRoomIdAsync(long roomId);

        IQueryable<Game> FindAll();

        Task SaveAsync(Game game);

        void Save(Game game);
    }
}
