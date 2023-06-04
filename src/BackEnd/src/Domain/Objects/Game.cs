using System.Collections.Immutable;
using Wsa.Gaas.Werewolf.Domain.Events;
using Wsa.Gaas.Werewolf.Domain.Exceptions;

namespace Wsa.Gaas.Werewolf.Domain.Objects
{
    public class Game
    {
        // private set => 自己才可以改
        // protected set => 繼承的 class 才可以改
        // internal set => 同一個 assembly 才可以改
        // public set => 所有人都可以改

        // assemlby => .Net Project 編譯出來的 dll 或 exe

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

        public GameStartedEvent StartGame(ulong[] playerIds)
        {
            if (Status != GameStatus.Created)
            {
                throw new GameAlreadyStartedException();
            }

            AddPlayers(playerIds);

            Status = GameStatus.Started;

            return new GameStartedEvent(this);
        }

        public PlayerRoleConfirmationStartedEvent StartPlayerRoleConfirmation()
        {
            if (Status != GameStatus.Started)
            {
                throw new GameStatusException(GameStatus.Started, Status);
            }

            Status = GameStatus.PlayerRoleConfirmationStarted;

            return new PlayerRoleConfirmationStartedEvent(this);
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

            for(var i = 0; i < n; i++)
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

        public void StartPlayerSpeaking()
        {
            Status = GameStatus.PlayerSpeaking;

            CurrentSpeakingPlayer = Players.OrderBy(_ => Guid.NewGuid())
                                           .First();
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

        public SeerDiscoveredEvent DiscoverPlayerRole(ulong playerId, Player discoverPlayer)
        {
            var gameEvent = new SeerDiscoveredEvent(this)
            {
                PlayerId = playerId,
                DiscoveredPlayerNumber = discoverPlayer.PlayerNumber,
                DiscoveredRoleFaction = discoverPlayer.Role!.Faction
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
    }
}