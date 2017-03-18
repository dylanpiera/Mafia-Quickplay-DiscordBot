using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;

namespace DiscordBot
{
    class Program
    {
        static void Main(string[] args) => new Program().Start();

        private DiscordClient _client;

        public void Start()
        {
            _client = new DiscordClient();
            
            //Setup our bots to listen for commands that start with ! or @mention the bot
            _client.UsingCommands(x =>
            {
                x.PrefixChar = '!';
                x.IsSelfBot = false;
                x.AllowMentionPrefix = true;
                x.HelpMode = HelpMode.Public;
            });

            _client.GetService<CommandService>().CreateCommand("ping")
                .Description("Bot answers with Pong!")
                .Do(async e =>
                {
                    await e.Channel.SendMessage(e.User.Mention + " Pong!");
                });

            _client.ExecuteAndWait(async () => {
                await _client.Connect(Sneaky.BotToken, TokenType.Bot);
            });
        }
    }
}
