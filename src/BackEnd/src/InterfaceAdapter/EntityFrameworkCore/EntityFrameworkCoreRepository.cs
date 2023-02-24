using Microsoft.EntityFrameworkCore;
using System.Numerics;
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

        public Task<Game?> FindByDiscordChannelIdAsync(ulong discordVoiceChannelId)
        {
            return FindAll().FirstOrDefaultAsync(x => x.DiscordVoiceChannelId == discordVoiceChannelId && x.Status != GameStatus.Ended);
        }

        public Task SaveAsync(Game game)
        {
            Save(game);

            return Task.CompletedTask;
        }

        public void Save(Game game)
        {
            var gameEntry = Entry(game);

            if (game.Id == Guid.Empty)
            {
                Add(game);
            }
            else if(gameEntry.State == EntityState.Detached)
            {
                Attach(game);
            }

            foreach (var player in game.Players)
            {
                var playerEntry = Entry(player);

                var role = player.Role;

                if (role != null)
                {
                    Attach(role);

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

        public void Initialize()
        {
            if (Database.IsInMemory())
            {
                Database.EnsureCreated();
            }
            else if(Database.IsSqlServer())
            {
                Database.Migrate();
            }
        }
    }
}
