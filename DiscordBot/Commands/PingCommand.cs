using Discord;
using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Discord.API.Client.Message;

namespace DiscordBot.Commands
{
    static class PingCommand
    {
        public static void createCommand(DiscordClient _client)
        {
            _client.GetService<CommandService>().CreateCommand("ping")
                .Description("Bot answers with Pong!")
                .Do(async e =>
                {
                    Message message = await e.Channel.SendMessage(e.User.Mention + " Pong!");                    
                });
        }
        public static void createCommand(CommandGroupBuilder builder)
        {
            builder.CreateCommand("ping")
                .Description("Bot answers with Pong!")
                .Do(async e =>
                {
                    Message message = await e.Channel.SendMessage(e.User.Mention + " Pong!");
                });
        }
    }
}
