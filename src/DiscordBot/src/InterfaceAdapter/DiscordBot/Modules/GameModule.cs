using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using System.Threading.Channels;
using Wsa.Gaas.Werewolf.DiscordBot.Application.UseCases;
using Wsa.Gaas.Werewolf.DiscordBot.DiscordClients;
using Wsa.Gaas.Werewolf.DiscordBot.Dtos;

namespace Wsa.Gaas.Werewolf.DiscordBot.Modules
{
    [Group("game", "遊戲")]
    public class GameModule : InteractionModuleBase<SocketInteractionContext>
    {
        private readonly BackendApi _backendApi;
        private readonly Dictionary<ulong, List<ulong>> _allJoinedPlayers = new();
        private readonly Random _random = new();

        public GameModule(
            BackendApi backendApi
        )
        {
            _backendApi = backendApi;
        }

        // /game status
        [SlashCommand("status", "遊戲狀態")]
        public async Task Status()
        {

            var channel = Context.Channel;
            if (channel is SocketVoiceChannel voiceChannel)
            {
                // 呼叫 後端 Creat Game API 
                try
                {
                    var gameDto = await _backendApi.GetGame(voiceChannel.Id);

                    if (gameDto == null)
                    {
                        
                        await RespondAsync(
                            "沒有遊戲正在進行中。",
                            components: BuildButtons(voiceChannel, gameDto)
                        );
                    }
                    else
                    {
                        await RespondAsync(
                            embed: BuildGameEmbed(voiceChannel, gameDto),
                            components: BuildButtons(voiceChannel, gameDto)
                        );
                    }

                }
                catch (Exception ex)
                {
                    await RespondAsync(
                        $"""
                        ```
                        {ex.Message}
                        ```
                        """
                    );
                }
            }
            else
            {
                var text = $"{channel.Name} 是 ${channel.GetChannelType()} 頻道 , 請在語音頻道使用此指令";

                await RespondAsync(
                    text,
                    ephemeral: true
                );
            }

        }

        [SlashCommand("version", "遊戲版本")]
        public async Task Version()
        {
            var version = await new GameVersionGetUseCase()
                .ExecuteAsync(new GameVersionGetRequest());

            await RespondAsync(
                embeds: new[]
                {
                    BuildGameVersionEmbed(version),
                }
            );
        }

        [SlashCommand("invite", "遊戲邀請")]
        public async Task Invite(int numberOfPlayers)
        {
            if (Context.Channel is SocketVoiceChannel channel)
            {
                var users = channel.Guild.Users; 

                RandomGame(channel.Id, numberOfPlayers, users);

                var gameDto = new GameDto { Id = channel.Id };

                await RespondAsync(
                    embeds: new[]
                    {
                        BuildGameEmbed(channel, gameDto)
                    },
                    components: BuildButtons(channel, gameDto)
                );
            }
        }

        [SlashCommand("create", "新增遊戲")]
        public async Task Create()
        {
            var channel = Context.Channel;
            var user = Context.User;

            var gameDto = await _backendApi.CreateGame(channel.Id)
                ?? await _backendApi.GetGame(channel.Id);

            if (_allJoinedPlayers.ContainsKey(gameDto!.Id) == false)
            {
                _allJoinedPlayers[gameDto.Id] = new List<ulong>
                {
                    user.Id,
                };
            }

            await RespondAsync(
                embeds: new[]
                {
                    BuildGameEmbed(channel, gameDto),
                },
                components: BuildButtons(channel, gameDto)
            );
        }

        [ComponentInteraction("btn-create-game", true)]
        public async Task CreateGame()
        {
            await DeferAsync();

            var channel = Context.Channel;
            var user = Context.User;

            // Data Transfer Object
            var gameDto = await _backendApi.CreateGame(channel.Id)
                ?? await _backendApi.GetGame(channel.Id);

            if (_allJoinedPlayers.ContainsKey(gameDto!.Id) == false)
            {
                _allJoinedPlayers[gameDto.Id] = new List<ulong>
                {
                    user.Id,
                };
            }

            gameDto = await _backendApi.GetGame(channel.Id) ?? new GameDto();

            await Context.Interaction.ModifyOriginalResponseAsync(prop =>
                {
                    prop.Content = "# 遊戲已新增";
                    prop.Embeds = new[]
                    {
                        BuildGameEmbed(channel, gameDto),
                    };

                prop.Components = BuildButtons(channel, gameDto);
            });

            //return gameDto;
        }

        [ComponentInteraction("btn-join-game", true)]
        public async Task JoinGame()
        {
            var channelId = Context.Channel.Id;
            var userId = Context.User.Id;
            var join = true;

            if (_allJoinedPlayers.ContainsKey(channelId) == false)
            {
                _allJoinedPlayers[channelId] = new();
            }

            var joinedPlayers = _allJoinedPlayers[channelId];

            if (join && joinedPlayers.Contains(userId) == false && joinedPlayers.Count < 12)
            {
                joinedPlayers.Add(userId);
            }
            else if (join == false)
            {
                joinedPlayers.Remove(userId);
            }
        }

        // TODO: call join game api
        private void RandomGame(ulong channelId, int n, IEnumerable<IUser> users)
        {
            if (_allJoinedPlayers.ContainsKey(channelId) == false)
            {
                _allJoinedPlayers[channelId] = new();
            }

            var joinedPlayers = _allJoinedPlayers[channelId];

            joinedPlayers.AddRange(
                users
                    .Where(x => x.IsBot == false)
                    .Where(x => joinedPlayers.Contains(x.Id) == false)
                    .Take(n - joinedPlayers.Count)
                    .Select(x => x.Id)
            );
        }

        internal MessageComponent BuildButtons(IChannel channel, GameDto? gameDto)
        {
            var builder = new ComponentBuilder();

            if (gameDto == null)
            {
                builder = builder.WithButton(
                    "開新遊戲",
                    "btn-create-game",
                    ButtonStyle.Primary
                );
            }
            else if (gameDto.Status == GameStatus.PlayerRoleConfirmationStarted)
            {
                builder = builder.WithButton(
                    "確認我的角色身分",
                    "btn-confirm-player-role",
                    ButtonStyle.Primary
                );
            }
            else
            {
                var canStart = _allJoinedPlayers[channel.Id].Count >= 9;
                builder = builder.WithButton(
                        "加入遊戲",
                        "btn-join-game",
                        ButtonStyle.Primary
                    )
                    .WithButton(
                        "離開遊戲",
                        "btn-leave-game",
                        ButtonStyle.Danger
                    )
                    .WithButton(
                        "開始遊戲",
                        "btn-start-game",
                        canStart ? ButtonStyle.Success : ButtonStyle.Secondary,
                        disabled: !canStart
                    );
            }

            return builder.Build();
        }

        internal Embed BuildGameEmbed(IChannel channel, GameDto gameDto)
        {
            var joinedPlayers = gameDto.Status == GameStatus.PlayerRoleConfirmationStarted
            ? gameDto.Players.Select(x => x.UserId)
            : _allJoinedPlayers[channel.Id];
            
            var players = "";
            for (var i = 0; i < 12; i++)
            {
                var user = Context.Guild.Users.FirstOrDefault(x => x.Id == joinedPlayers.ElementAtOrDefault(i));
                players += $"[{user?.Mention ?? " "}]\n";
            }

            return new EmbedBuilder()
                .WithColor(RandomColor())
                .WithFields(
                    new EmbedFieldBuilder()
                        .WithName("Slot")
                        .WithValue(string.Join("\n", Enumerable.Range(1, 12).Select(x => $"[{x}]")))
                        .WithIsInline(true)
                        ,

                    new EmbedFieldBuilder()
                        .WithName("Players")
                        .WithValue(players)
                        .WithIsInline(true)
                )
                .Build();
        }

        internal static Embed BuildGameVersionEmbed(string version)
        {
            var builder = new EmbedBuilder()
                .WithColor(Color.Blue)
                .WithCurrentTimestamp()
                .WithTitle("狼人殺版本")
                .WithDescription(version)
                ;

            return builder.Build();
        }

        private Color RandomColor()
        {
            return new Color(_random.Next(256), _random.Next(256), _random.Next(256));
        }
    }
}
