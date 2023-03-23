using Bogus;
using Wsa.Gaas.Werewolf.Domain.Objects;

namespace Wsa.Gaas.Werewolf.WebApiTests.Common
{
    internal class GameFaker : Faker<Game>
    {
        public GameFaker() 
        {
            RuleSet(nameof(CreateGame), ruleSet =>
            {
                ruleSet.RuleFor(e => e.DiscordVoiceChannelId,
                    faker => faker.Random.ULong(1));
            });

            RuleSet(nameof(StartGame), ruleSet =>
            {
                ruleSet.RuleFor(e => e.DiscordVoiceChannelId,
                    faker => faker.Random.ULong(1));

                ruleSet.RuleFor(e => e.Status,
                    GameStatus.Started);
            });

        }

        public Game CreateGame()
        {
            return Generate(nameof(CreateGame));
        }

        public Game StartGame()
        {
            return Generate(nameof(StartGame));
        }

    }
}
