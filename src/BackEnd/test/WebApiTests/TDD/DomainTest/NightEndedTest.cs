using Bogus.DataSets;
using Wsa.Gaas.Werewolf.Domain.Events;
using Wsa.Gaas.Werewolf.Domain.Objects;

namespace Wsa.Gaas.Werewolf.WebApiTests.TDD.DomainTest;
public class NightEndedTest
{
    [Test]
    [Description("""
        Given:
        * 9 位玩家存活
        * 狼人們沒投票
        * 女巫沒用毒
        
        When:
        * 夜晚結束

        Then:
        * 沒有人死亡，9 位玩家依然存活

        """)]
    public void Given沒狼殺沒用毒_Then平安夜()
    {
        // Arrange
        var game = new Game
        {
            DiscordVoiceChannelId = 1,
            Status = GameStatus.NightEnded,
        };
        game.AddPlayers(new ulong[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 });

        // Act: return 平安夜 events
        var events = game.AnnounceNightResult();

        // Assert
        foreach (var player in game.Players)
        {
            // 每個人必須活著
            player.IsDead.Should().BeFalse();
            player.BuffStatus.Should().Be(BuffStatus.None);
        }

        events.Should().HaveCount(1);
        events.First().Should().BeOfType<SafetyEveGameEvent>();

    }

    [Test]
    [Description("""
        Given:
        * 9 位玩家存活 (1,2,3,4,5,6,7,8,9)
        * 狼人們投了 3 號玩家
        * 女巫用了解藥
        
        When:
        * 夜晚結束

        Then:
        * 沒有人死亡，9 位玩家依然存活

        """)]
    public void Given狼殺解藥_Then平安夜()
    {
        var game = new Game
        {
            DiscordVoiceChannelId = 1,
            Status = GameStatus.NightEnded,
        };
        game.AddPlayers(new ulong[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 });

        game.Players[3].BuffStatus |= BuffStatus.KilledByWerewolf;
        game.Players[3].BuffStatus |= BuffStatus.SavedByWitch;

        // Act: return 平安夜 events
        var events = game.AnnounceNightResult();

        // Assert
        foreach (var player in game.Players)
        {
            // 每個人必須活著
            player.IsDead.Should().BeFalse();
            player.BuffStatus.Should().Be(BuffStatus.None);
        }

        events.Should().HaveCount(1);
        events.First().Should().BeOfType<SafetyEveGameEvent>();
    }

    [Test]
    [Description("""
        Given:
        * 9 位玩家存活 (1,2,3,4,5,6,7,8,9)
        * 狼人們投了 5 號玩家
        
        When:
        * 夜晚結束

        Then:
        * 5 號玩家死亡，剩 8 位玩家存活 (1,2,3,4,6,7,8,9)

        """)]
    public void Given只有狼殺_Then一人死亡()
    {
        var game = new Game
        {
            DiscordVoiceChannelId = 1,
            Status = GameStatus.NightEnded,
        };
        game.AddPlayers(new ulong[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 });

        var targetPlayer = game.Players[5];
        targetPlayer.BuffStatus |= BuffStatus.KilledByWerewolf;

        // Act: return 玩家死亡
        var events = game.AnnounceNightResult();

        // Assert
        foreach (var player in game.Players)
        {
            if (player.UserId == targetPlayer.UserId)
            {
                player.IsDead.Should().BeTrue();
            }
            else
            {
                player.IsDead.Should().BeFalse();
            }

            player.BuffStatus.Should().Be(BuffStatus.None);
        }

        events.Should().HaveCount(1);
        events.First().Should().BeOfType<PlayerDiedGameEvent>();
    }

    [Test]
    [Description("""
        Given:
        * 9 位玩家存活 (1,2,3,4,5,6,7,8,9)
        * 狼人們沒投票
        * 女巫毒了 5 號玩家
        
        When:
        * 夜晚結束

        Then:
        * 5 號玩家死亡，剩 8 位玩家存活 (1,2,3,4,6,7,8,9)

        """)]
    public void Given只有用毒_Then一人死亡()
    {
        var game = new Game
        {
            DiscordVoiceChannelId = 1,
            Status = GameStatus.NightEnded,
        };
        game.AddPlayers(new ulong[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 });

        var targetPlayer = game.Players[5];
        targetPlayer.BuffStatus |= BuffStatus.KilledByWitch;

        // Act: return 玩家死亡
        var events = game.AnnounceNightResult();

        // Assert
        foreach (var player in game.Players)
        {
            if (player.UserId == targetPlayer.UserId)
            {
                player.IsDead.Should().BeTrue();
            }
            else
            {
                player.IsDead.Should().BeFalse();
            }

            player.BuffStatus.Should().Be(BuffStatus.None);
        }

        events.Should().HaveCount(1);
        events.First().Should().BeOfType<PlayerDiedGameEvent>();
    }

    [Test]
    [Description("""
        Given:
        * 9 位玩家存活 (1,2,3,4,5,6,7,8,9)
        * 狼人們投了 5 號玩家
        * 女巫毒了 5 號玩家
        
        When:
        * 夜晚結束

        Then:
        * 5 號玩家死亡，剩 8 位玩家存活 (1,2,3,4,6,7,8,9)

        """)]
    public void Given用毒狼殺同一人_Then一人死亡()
    {
        var game = new Game
        {
            DiscordVoiceChannelId = 1,
            Status = GameStatus.NightEnded,
        };
        game.AddPlayers(new ulong[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 });

        var targetPlayer = game.Players[5];
        targetPlayer.BuffStatus = BuffStatus.KilledByWitch | BuffStatus.KilledByWerewolf;

        // Act: return 玩家死亡
        var events = game.AnnounceNightResult();

        // Assert
        foreach (var player in game.Players)
        {
            if (player.UserId == targetPlayer.UserId)
            {
                player.IsDead.Should().BeTrue();
            }
            else
            {
                player.IsDead.Should().BeFalse();
            }

            player.BuffStatus.Should().Be(BuffStatus.None);
        }

        events.Should().HaveCount(1);
        events.First().Should().BeOfType<PlayerDiedGameEvent>();
    }

    [Test]
    [Description("""
        Given:
        * 9 位玩家存活 (1,2,3,4,5,6,7,8,9)
        * 狼人們投了 7 號玩家
        * 女巫毒了 5 號玩家
        
        When:
        * 夜晚結束

        Then:
        * 5 and 7 號玩家死亡，剩 7 位玩家存活 (1,2,3,4,6,8,9)

        """)]
    public void Given用毒狼殺不同人_Then兩人死亡()
    {
        // 3 AAA

        // Arrange
        var game = new Game
        {
            DiscordVoiceChannelId = 1,
            Status = GameStatus.NightEnded,
        };
        // * 9 位玩家存活 (1,2,3,4,5,6,7,8,9)
        game.AddPlayers(new ulong[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 });

        var player5 = game.Players[4];
        var player7 = game.Players[6];

        // * 女巫毒了 5 號玩家
        player5.BuffStatus = BuffStatus.KilledByWitch;

        // * 狼人們投了 7 號玩家
        player7.BuffStatus = BuffStatus.KilledByWerewolf;

        // Act
        var events = game.AnnounceNightResult();

        // Assert
        foreach (var player in game.Players)
        {
            if (player.UserId == player5.UserId || player.UserId == player7.UserId)
            {
                // * 5 and 7 號玩家死亡
                player.IsDead.Should().BeTrue();
            }
            else
            {
                // 剩 7 位玩家存活 (1,2,3,4,6,8,9)
                player.IsDead.Should().BeFalse();
            }

            // 驗證狀態要清空
            player.BuffStatus.Should().Be(BuffStatus.None);
        }

        // 如果有兩個人死掉，會有幾個 domain event 發出來?
        // 1. 兩個 events

        // TODO
        // 問題: 玩家死後，有一位要觸發技能...

        // 發送的 domain event
        events.Should().HaveCount(2);
        events.First().Should().BeOfType<PlayerDiedGameEvent>();
    }

    [Test]
    [Description("""
        Given:
        * 9 位玩家存活 (1,2,3,4,5,6,7,8,9)
        * 狼人們投了獵人玩家
        
        When:
        * 夜晚結束

        Then:
        * 獵人玩家死亡並觸發技能，剩 8 位玩家存活

        """)]
    public void Given狼殺獵人_Then獵人死亡並觸發腳色技能()
    {
        // 3 AAA

        // Arrange
        var game = new Game
        {
            DiscordVoiceChannelId = 1,
            Status = GameStatus.NightEnded,
        };
        // * 9 位玩家存活 (1,2,3,4,5,6,7,8,9)
        game.AddPlayers(new ulong[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 });

        // 找獵人
        var hunter = game.Players.Single(p => p.Role is Domain.Objects.Roles.Hunter);

        // * 狼人們投了獵人玩家
        hunter.BuffStatus = BuffStatus.KilledByWerewolf;

        // Act
        var events = game.AnnounceNightResult();

        // Assert
        foreach (var player in game.Players)
        {
            if (player.UserId == hunter.UserId)
            {
                // * 獵人玩家死亡
                player.IsDead.Should().BeTrue();
            }
            else
            {
                // 剩 8 位玩家存活 (1,2,3,4,6,8,9)
                player.IsDead.Should().BeFalse();
            }

            // 驗證狀態要清空
            player.BuffStatus.Should().Be(BuffStatus.None);
        }

        // 如果有兩個人死掉，會有幾個 domain event 發出來?
        // 1. 兩個 events

        // TODO
        // 問題: 玩家死後，有一位要觸發技能...

        // 發送的 domain event
        events.Should().HaveCount(1);
        var diedEvent = events.First();
        diedEvent.Should().BeOfType<PlayerDiedGameEvent>();

        ((PlayerDiedGameEvent)diedEvent).Skill.Should().Be(SkillTrigger.Shot);

    }

    [Test]
    [Description("""
        Given:
        * 12 位玩家存活 (1,2,3,4,5,6,7,8,9,10,11,12)
        * 狼人們投了狼王玩家 (騙銀水)
        
        When:
        * 夜晚結束

        Then:
        * 狼王玩家死亡並觸發技能，剩 11 位玩家存活

        """)]
    public void Given狼殺狼王_Then狼王死亡並觸發腳色技能()
    {
        // 3 AAA

        // Arrange
        var game = new Game
        {
            DiscordVoiceChannelId = 1,
            Status = GameStatus.NightEnded,
        };
        // * 12 位玩家存活 (1,2,3,4,5,6,7,8,9,10,11,12)
        game.AddPlayers(new ulong[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12 });

        // 找狼王
        var alphaWerewolf = game.Players.Single(p => p.Role is Domain.Objects.Roles.AlphaWerewolf);

        // * 狼人們投了狼王玩家
        alphaWerewolf.BuffStatus = BuffStatus.KilledByWerewolf;

        // Act
        var events = game.AnnounceNightResult();

        // Assert
        foreach (var player in game.Players)
        {
            if (player.UserId == alphaWerewolf.UserId)
            {
                // * 狼人玩家死亡
                player.IsDead.Should().BeTrue();
            }
            else
            {
                // 剩 8 位玩家存活 (1,2,3,4,6,8,9)
                player.IsDead.Should().BeFalse();
            }

            // 驗證狀態要清空
            player.BuffStatus.Should().Be(BuffStatus.None);
        }

        // 發送的 domain event
        events.Should().HaveCount(1);
        var diedEvent = events.First();
        diedEvent.Should().BeOfType<PlayerDiedGameEvent>();

        ((PlayerDiedGameEvent)diedEvent).Skill.Should().Be(SkillTrigger.Shot);

    }
}
