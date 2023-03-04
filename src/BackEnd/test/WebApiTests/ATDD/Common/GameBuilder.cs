using Wsa.Gaas.Werewolf.Application.Common;
using Wsa.Gaas.Werewolf.Domain.Objects;

namespace Wsa.Gaas.Werewolf.WebApiTests.ATDD.Common
{
    internal class GameBuilder
    {
        private readonly Game _game;
        private readonly IRepository _repository;

        internal GameBuilder(IRepository repository)
        {
            _game = new Game();
            _repository = repository;
        }

        public GameBuilder WithRandomDiscordVoiceChannel()
        {
            return WithDiscordVoiceChannel(new Random().Next());
        }

        public GameBuilder WithDiscordVoiceChannel(long channelId)
        {
            _game.DiscordVoiceChannelId = channelId;

            return this;
        }

        public GameBuilder WithGameStatus(GameStatus status)
        {
            _game.Status = status;

            return this;
        }

        public GameBuilder WithPlayers(long[] players)
        {
            _game.AddPlayers(players);

            return this;
        }

        public GameBuilder WithRandomPlayers(int count)
        {
            _game.AddPlayers(Enumerable.Range(1, count)
                                       .Select(o => (long) o)
                                       .ToArray());

            return this;
        }

        public Game Build()
        {
            _repository.Save(_game);

            return _game;
        }
    }
}