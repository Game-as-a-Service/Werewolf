﻿using Wsa.Gaas.Werewolf.Application.Common;
using Wsa.Gaas.Werewolf.Domain.Objects;

namespace Wsa.Gaas.Werewolf.InMemory;
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
        return Task.FromResult(FindByDiscordChannelId(discordChannelId));
    }

    public Game? FindByDiscordChannelId(ulong discordChannelId)
    {
        _discordIdMemory.TryGetValue(discordChannelId, out var game);

        return game?.Status == GameStatus.Ended
            ? null
            : game
            ;
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
