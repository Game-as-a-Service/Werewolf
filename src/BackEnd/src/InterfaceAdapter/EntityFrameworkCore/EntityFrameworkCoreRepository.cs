using Microsoft.EntityFrameworkCore;
using Wsa.Gaas.Werewolf.Application.Common;
using Wsa.Gaas.Werewolf.Domain.Objects;

namespace Wsa.Gaas.Werewolf.EntityFrameworkCore
{
    public class EntityFrameworkCoreRepository : DbContext, IRepository
    {
        public EntityFrameworkCoreRepository(DbContextOptions opt) : base(opt)
        {
        }

        public IQueryable<Game> FindAll()
        {
            return Set<Game>();
        }

        public async Task<Game?> FindByIdAsync(long id)
        {
            return await FindAsync<Game>(id);
        }

        public Task<Game?> FindByRoomIdAsync(long roomId)
        {
            return FindAll().FirstOrDefaultAsync(x => x.RoomId == roomId && x.Status != GameStatus.Ended);
        }

        public async Task SaveAsync(Game game)
        {
            if (Entry(game).State == EntityState.Detached)
            {
                await AddAsync(game);
            }
            await SaveChangesAsync();
        }

        public void Save(Game game)
        {
            var gameEntry = Entry(game);
            if (gameEntry.State == EntityState.Detached)
            {
                Add(game);
            }

            foreach (var role in game.Players.Select(x => x.Role))
            {
                if (role != null)
                {
                    var entry = Entry(role);
                }
            }
            SaveChanges();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfigurationsFromAssembly(GetType().Assembly);
        }


    }
}
