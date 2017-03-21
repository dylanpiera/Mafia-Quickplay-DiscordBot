using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DiscordBot.Commands;
using DiscordBot.Game;
using Discord;
using Discord.Commands;
using System.ComponentModel;
using System.Timers;
using DiscordBot.Core;
using DiscordBot.Util;

namespace DiscordBot
{
    class Program
    {
        static void Main(string[] args) => new Program().Start();
        public static Dictionary<Server, GamePlayerList> servers = new Dictionary<Server, GamePlayerList>();

        private DiscordClient _client;

        public void Start()
        {
            _client = new DiscordClient();

            //Setup our bots to listen for commands that start with ! or @mention the bot
            _client.UsingCommands(x =>
            {
                x.PrefixChar = '*';
                x.IsSelfBot = false;
                x.AllowMentionPrefix = true;
                x.HelpMode = HelpMode.Private;
            });

            //For each server create a new Game, this way multiple games can be ran at once
            _client.ServerAvailable += (s, e) =>
            {
                foreach (var server in _client.Servers)
                {
                    Console.WriteLine("Server " + server.Name + " found.");
                    servers[server] = new GamePlayerList();
                }
            };

            CommandInitializer.init(_client);

            _client.ExecuteAndWait(async () => {
                await _client.Connect(Sneaky.BotToken, TokenType.Bot);

                _client.SetGame(new Discord.Game("Mafia!", GameType.Default, ""));

                _client.Log.Message += (s, e) => Console.WriteLine(e.Severity + " " + e.Source + " " + e.Message);
            });

            
        }
    }
}
