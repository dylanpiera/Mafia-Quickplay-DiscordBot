using Discord;
using Discord.Commands;
using System.Threading.Tasks;

namespace DiscordBot.Commands {
    static class CommandTemplate {
        public static void createCommand(DiscordClient _client) {
            _client.GetService<CommandService>().CreateCommand("command_name")
                .Do(async e => {
                    await Task.Delay(0);
                });
        }
    }
}