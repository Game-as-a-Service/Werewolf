﻿using System.Collections.Immutable;
using Wsa.Gaas.Werewolf.Domain.Exceptions;

namespace Wsa.Gaas.Werewolf.Domain.Objects
{
    public class Game
    {
        public Guid Id { get; internal set; }
        public ulong DiscordVoiceChannelId { get; internal set; }
        public GameStatus Status { get; internal set; }

        private List<Player> _players = new();
        public ImmutableList<Player> Players { get => _players.ToImmutableList(); }

        internal Game()
        {
        }

        public Game(ulong discordVoiceChannelId)
        {
            DiscordVoiceChannelId = discordVoiceChannelId;
            Status = GameStatus.Created;
        }

        public void StartGame(ulong[] players)
        {
            if (Status != GameStatus.Created)
            {
                throw new GameAlreadyStartedException();
            }

            AddPlayers(players);
            AssignRolesToPlayers();

            Status = GameStatus.Started;
        }

        public void StartPlayerRoleConfirmation()
        {
            Status = GameStatus.PlayerRoleConfirmationStarted;
        }

        internal void AddPlayers(ulong[] players)
        {
            var uniquePlayers = players.Distinct().ToList();

            if (uniquePlayers.Count != players.Length)
            {
                throw new PlayersDuplicatedException();
            }

            if (uniquePlayers.Count < 9 || uniquePlayers.Count > 12)
            {
                throw new PlayersNumberNotSupportedException();
            }

            _players.AddRange(players.Select(x => new Player(x)));
        }

        internal void AssignRolesToPlayers()
        {
            // Shuffle
            _players = _players.OrderBy(_ => Guid.NewGuid()).ToList();

            var roles = GetRoles(_players.Count).OrderBy(_ => Guid.NewGuid()).ToList();

            // Assign Roles
            for (var i = 0; i < roles.Count; i++)
            {
                var role = roles[i];
                var player = _players[i];

                player.SetRole(role, i + 1);
            }
        }

        private List<Role> GetRoles(int n)
        {
            var roles = new List<Role>()
            {
                new Villager(), new Villager(), new Villager(),
                new Werewolf(), new Werewolf(), new Werewolf(),
                new Witch(), new Seer(), new Hunter(),
            };

            if (n >= 10)
            {
                roles.Add(new Villager());
            }

            if (n >= 11)
            {
                roles.Add(new AlphaWerewolf());
            }

            if (n >= 12)
            {
                roles.Add(new Guardian());
            }

            return roles;
        }
    }
}