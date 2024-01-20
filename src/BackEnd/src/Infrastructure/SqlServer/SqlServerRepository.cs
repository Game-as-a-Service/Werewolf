using Microsoft.EntityFrameworkCore;
using Wsa.Gaas.Werewolf.Application.Common;
using Wsa.Gaas.Werewolf.Domain.Objects;

namespace Wsa.Gaas.Werewolf.SqlServer;
public class SqlServerRepository : DbContext, IRepository
{
    public SqlServerRepository(DbContextOptions opt) : base(opt)
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
        return Task.FromResult(FindByDiscordChannelId(discordVoiceChannelId));
    }

    public Game? FindByDiscordChannelId(ulong discordVoiceChannelId)
    {
        return FindAll().FirstOrDefault(x => x.DiscordVoiceChannelId == discordVoiceChannelId && x.Status != GameStatus.Ended);
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
        else if (gameEntry.State == EntityState.Detached)
        {
            Attach(game);
        }

        foreach (var player in game.Players)
        {
            var role = player.Role;

            if (role != null)
            {
                Attach(role);
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
