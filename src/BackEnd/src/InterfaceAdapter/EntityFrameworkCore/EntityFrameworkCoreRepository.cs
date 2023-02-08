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

        public async Task<Game?> FindByIdAsync(Guid id)
        {
            return await FindAsync<Game>(id);
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
            if (Entry(game).State == EntityState.Detached)
            {
                Add(game);
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
