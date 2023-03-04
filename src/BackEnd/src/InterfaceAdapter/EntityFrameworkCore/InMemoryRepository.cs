using Wsa.Gaas.Werewolf.Application.Common;
using Wsa.Gaas.Werewolf.Domain.Objects;

namespace Wsa.Gaas.Werewolf.EntityFrameworkCore;

public class InMemoryRepository : IRepository
{
    private readonly Dictionary<long, Game> _roomIdMemory = new();
    private readonly Dictionary<long, Game> _idMemory = new();
    private readonly IIdGenerator _idGenerator;

    public InMemoryRepository(IIdGenerator idGenerator)
    {
        _idGenerator = idGenerator;
    }

    public IQueryable<Game> FindAll()
    {
        return _idMemory.Values.AsQueryable();
    }

    public Task<Game?> FindByRoomIdAsync(long roomId)
    {
        _roomIdMemory.TryGetValue(roomId, out var game);

        return Task.FromResult(game);
    }

    public Task<Game?> FindByIdAsync(long id)
    {
        _idMemory.TryGetValue(id, out var game);

        return Task.FromResult(game);
    }

    public void Save(Game game)
    {
        if (game.Id == default)
        {
            game.GetType().GetProperty(nameof(game.Id))!.SetValue(game, _idGenerator.GenerateId());
        }
        _idMemory[game.Id] = game;
        _roomIdMemory[game.RoomId] = game;
    }

    public Task SaveAsync(Game game)
    {
        Save(game);
        return Task.CompletedTask;
    }
}