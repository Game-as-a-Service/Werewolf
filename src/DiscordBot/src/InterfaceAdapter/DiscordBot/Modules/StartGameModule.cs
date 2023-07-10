using Discord.Interactions;

namespace Wsa.Gaas.Werewolf.DiscordBot.Modules
{
    public class StartGameModule : InteractionModuleBase<SocketInteractionContext>
    {
        [SlashCommand("update_all_user_data", "(Owner Only) 更新所有成員資料")]
        public async Task StartGame()
        {

            await ModifyOriginalResponseAsync(x => x.Content = "成員資料更新成功!");
        }
    }
}
