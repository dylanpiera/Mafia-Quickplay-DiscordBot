using Discord;
using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBot.Commands {
    static class CookieCommand {
        public static void createCommand(DiscordClient _client) {
            _client.GetService<CommandService>().CreateCommand("cookie")
                .Description("Bot answers with :cookie:")
                .Do(async e => {
                    Message message = await e.Channel.SendMessage(e.User.Mention + " :cookie::cookie:");
                });
        }
    }
}