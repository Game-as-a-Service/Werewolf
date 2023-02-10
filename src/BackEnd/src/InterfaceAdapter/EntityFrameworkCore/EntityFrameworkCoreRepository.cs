using Microsoft.EntityFrameworkCore;
using Wsa.Gaas.Werewolf.Application.Common;
using Wsa.Gaas.Werewolf.Domain.Objects;

namespace Wsa.Gaas.Werewolf.EntityFrameworkCore
{
    public class InMemoryRepository : IRepository
    {
        private readonly Dictionary<ulong, Game> _discordIdMemory = new();
        private readonly Dictionary<Guid, Game> _idMemory = new();

        public InMemoryRepository() { }

        public IQueryable<Game> FindAll()
        {
            return _idMemory.Values.AsQueryable();
        }

        public Task<Game?> FindByDiscordChannelIdAsync(ulong discordChannelId)
        {
            _discordIdMemory.TryGetValue(discordChannelId, out var game);

            return Task.FromResult(game);
        }

        public Task<Game?> FindByIdAsync(Guid id)
        {
            _idMemory.TryGetValue(id, out var game);

            return Task.FromResult(game);
        }

        public void Save(Game game)
        {
            if (game.Id == Guid.Empty)
            {
                game.GetType().GetProperty(nameof(game.Id))!.SetValue(game, Guid.NewGuid());
            }
            _idMemory[game.Id] = game;
            _discordIdMemory[game.DiscordVoiceChannelId] = game;
        }

        public Task SaveAsync(Game game)
        {
            Save(game);
            return Task.CompletedTask;
        }
    }
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
