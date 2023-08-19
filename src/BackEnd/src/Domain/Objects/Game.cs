using System.Collections.Immutable;
using Wsa.Gaas.Werewolf.Domain.Common;
using Wsa.Gaas.Werewolf.Domain.Events;
using Wsa.Gaas.Werewolf.Domain.Exceptions;
using Wsa.Gaas.Werewolf.Domain.Objects.Roles;

namespace Wsa.Gaas.Werewolf.Domain.Objects;

public class Game
{
    public Guid Id { get; internal set; }
    public ulong DiscordVoiceChannelId { get; internal set; }
    public GameStatus Status { get; internal set; }
    public Guid? CurrentSpeakingPlayerId { get; internal set; }

    private readonly List<Player> _players = new();
    internal readonly VoteManager VoteManager = new();
    public ImmutableList<Player> Players => _players.ToImmutableList();
    public Player? CurrentSpeakingPlayer { get; internal set; }

    internal Game() { }

    public Game(ulong discordVoiceChannelId)
    {
        DiscordVoiceChannelId = discordVoiceChannelId;
        Status = GameStatus.Created;
    }

    public virtual IEnumerable<GameEvent> StartGame(ulong[] playerIds)
    {
        if (Status != GameStatus.Created)
        {
            throw new GameAlreadyStartedException();
        }

        AddPlayers(playerIds);

        Status = GameStatus.PlayerRoleConfirmationStarted;

        return new GameEvent[] {
            new GameStartedEvent(this),
            new PlayerRoleConfirmationStartedEvent(this),
        };
    }

    public PlayerRoleConfirmedEvent ConfirmPlayerRole(ulong playerId)
    {
        var player = Players.FirstOrDefault(x => x.UserId == playerId)
            ?? throw new PlayerNotFoundException(DiscordVoiceChannelId, playerId);

        if (player.Role == null)
        {
            throw new PlayerRoleNotAssignedException(playerId);
        }

        var gameEvent = new PlayerRoleConfirmedEvent(this)
        {
            PlayerId = playerId,
            Role = player.Role.Name,
        };

        return gameEvent;
    }

    public void StartPlayerSpeaking()
    {
        Status = GameStatus.PlayerSpeaking;

        CurrentSpeakingPlayer = Players.OrderBy(_ => Guid.NewGuid())
                                       .First();
    }

    public SeerDiscoveredEvent DiscoverPlayerRole(ulong userId, int playerNumber)
    {
        var player = Players.ElementAt(playerNumber - 1);

        if (player.IsDead)
        {
            throw new PlayerNotSurvivedException(playerNumber);
        }

        var gameEvent = new SeerDiscoveredEvent(this)
        {
            PlayerId = userId,
            DiscoveredPlayerNumber = playerNumber,
            DiscoveredRoleFaction = player.Role.Faction
        };

        return gameEvent;
    }

    public void EndGame()
    {
        Status = GameStatus.Ended;
    }

    public virtual WerewolfVotedEvent WerewolfVote(ulong callerId, ulong targetId)
    {
        // caller 真的在這場遊戲嗎?
        var caller = Players.FirstOrDefault(x => x.UserId == callerId);

        if (caller == null)
        {
            throw new PlayerNotFoundException(DiscordVoiceChannelId, callerId);
        }

        // caller 真的是狼人嗎?
        var isWerewolf = caller.IsWerewolf();

        if (isWerewolf == false)
        {
            throw new PlayerNotWerewolfException("Caller is not a werewolf");
        }

        // 真的投票
        VoteManager.Vote(callerId, targetId);

        return new WerewolfVotedEvent(this);

    }

    internal void AddPlayers(ulong[] playerIds)
    {
        var uniquePlayerIds = playerIds.Distinct().ToList();

        if (uniquePlayerIds.Count != playerIds.Length)
        {
            throw new PlayersDuplicatedException();
        }

        if (uniquePlayerIds.Count < 9 || uniquePlayerIds.Count > 12)
        {
            throw new PlayersNumberNotSupportedException();
        }

        int n = playerIds.Length;

        var randomPlayerIds = playerIds.OrderBy(_ => Guid.NewGuid()).ToList();
        var randomRoles = GetRoles(n).OrderBy(_ => Guid.NewGuid()).ToList();

        for (var i = 0; i < n; i++)
        {
            _players.Add(new Player(
                randomPlayerIds[i],
                i + 1,
                randomRoles[i]
            ));
        }
    }

    internal static List<Role> GetRoles(int n)
    {
        var roles = new List<Role>()
        {
            Role.VILLAGER, Role.VILLAGER, Role.VILLAGER,
            Role.WEREWOLF, Role.WEREWOLF, Role.WEREWOLF,
            Role.WITCH, Role.SEER, Role.HUNTER,
        };

        if (n >= 10)
        {
            roles.Add(Role.VILLAGER);
        }

        // TODO, 10 人 = 4 平民 + 3 神職 + 2 小狼 + 狼王
        if (n >= 11)
        {
            roles.Add(Role.ALPHAWEREWOLF);
        }

        if (n >= 12)
        {
            roles.Add(Role.GUARDIAN);
        }

        return roles;
    }

    public WitchUseAntidoteEvent WitchUseAntidote(ulong witchUserId)
    {
        var witch = Players.FirstOrDefault(x => x.UserId == witchUserId);

        if (witch is null)
        {
            throw new PlayerNotFoundException(DiscordVoiceChannelId, witchUserId);
        }

        if (witch.Role is not Witch)
        {
            throw new PlayerNotWitchException("Player not witch");
        }

        if (witch.IsAntidoteUsed)
        {
            throw new GameException("Witch antidote is used");
        }

        // 找出被狼殺的玩家
        var playerKilledByWerewolf = Players.FirstOrDefault(x =>
            (x.BuffStatus & BuffStatus.KilledByWerewolf) == BuffStatus.KilledByWerewolf
        );

        if (playerKilledByWerewolf == null)
        {
            throw new GameException("No one was killed by werewolf");
        }

        // 標記被女巫救
        playerKilledByWerewolf.BuffStatus |= BuffStatus.SavedByWitch;

        // 標記解藥已使用
        witch.IsAntidoteUsed = true;

        return new WitchUseAntidoteEvent(this);
    }

    internal List<GameEvent> AnnounceNightResult()
    {
        var events = new List<GameEvent>();

        // 檢查狼殺
        var playerKilledByWerewolf = Players.SingleOrDefault(p => p.BuffStatus == BuffStatus.KilledByWerewolf);
        if (playerKilledByWerewolf != null) 
        { 
            playerKilledByWerewolf.IsDead = true;

            var @event = new PlayerDiedGameEvent(this);

            var shouldTriggerShot = playerKilledByWerewolf.Role is Hunter or AlphaWerewolf;

            if (shouldTriggerShot)
            {
                @event.Skill = SkillTrigger.Shot;
            }

            events.Add(@event);
        }

        // 檢查女巫毒藥
        var playerKilledByWitch = Players.SingleOrDefault(p => p.BuffStatus == BuffStatus.KilledByWitch);
        if (playerKilledByWitch != null)
        {
            playerKilledByWitch.IsDead = true;
            events.Add(new PlayerDiedGameEvent(this));
        }

        // 檢查同時被狼殺跟女巫毒
        var playerKilledByWitchandWerewolf = Players.SingleOrDefault(p => p.BuffStatus == (BuffStatus.KilledByWitch | BuffStatus.KilledByWerewolf));
        if (playerKilledByWitchandWerewolf != null)
        {
            playerKilledByWitchandWerewolf.IsDead = true;
            events.Add(new PlayerDiedGameEvent(this));
        }



        // 清除狀態
        Players.ForEach(p => p.BuffStatus = BuffStatus.None);

        if (events.Count == 0)
        {
            events.Add(new SafetyEveGameEvent(this));
        }

        Status = GameStatus.LastNightResultAnnounced;

        return events;
    }

    public WitchUsePoisonEvent WitchUsePoison(ulong witchUserId, ulong targetPlayerId)
    {
        var witch = Players.FirstOrDefault(x => x.UserId == witchUserId);

        if (witch is null)
        {
            throw new PlayerNotFoundException(DiscordVoiceChannelId, witchUserId);
        }

        if (witch.Role is not Witch)
        {
            throw new PlayerNotWitchException("Player not witch");
        }

        if (witch.IsPoisonUsed)
        {
            throw new GameException("Witch poison is used");
        }

        // 找出被狼殺的玩家
        var targetPlayer = Players.FirstOrDefault(x =>
            x.UserId == targetPlayerId
        );

        if (targetPlayer == null)
        {
            throw new GameException("Target player not found");
        }

        if (targetPlayer.IsDead)
        {
            throw new GameException("Target player is dead");
        }

        // 標記被女巫毒
        targetPlayer.BuffStatus |= BuffStatus.KilledByWitch;

        // 標記毒藥已使用
        witch.IsPoisonUsed = true;

        return new WitchUsePoisonEvent(this);
    }
}