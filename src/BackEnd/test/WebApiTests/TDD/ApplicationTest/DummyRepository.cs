using Wsa.Gaas.Werewolf.Application.Common;
using Wsa.Gaas.Werewolf.Domain.Objects;

namespace Wsa.Gaas.Werewolf.WebApiTests.TDD.ApplicationTest
{
    public class DummyRepository : IRepository
    {
        public IQueryable<Game> FindAll()
        {
            throw new NotImplementedException();
        }

        public Task<Game?> FindByDiscordChannelIdAsync(ulong discordChannelId)
        {
            throw new NotImplementedException();
        }

        public Task<Game?> FindByIdAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public void Initialize()
        {
            throw new NotImplementedException();
        }

        public void Save(Game game)
        {
            throw new NotImplementedException();
        }

        public Task SaveAsync(Game game)
        {
            throw new NotImplementedException();
        }
    }
}
