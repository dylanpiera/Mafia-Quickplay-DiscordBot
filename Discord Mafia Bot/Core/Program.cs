using System;
using Discord_Mafia_Bot.Commands;
using System.Linq;
using System.Threading.Tasks;
using System.Reflection;
using Discord;
using Discord.WebSocket;
using Discord.Commands;
using Microsoft.Extensions.DependencyInjection;
using Discord_Mafia_Bot.Util;
using DiscordBot.Game;
using System.Collections.Generic;

namespace Discord_Mafia_Bot
{

    public class Program
    {
        public static void Main(string[] args) => new Program().MainAsync().GetAwaiter().GetResult();
        private static Dictionary<IGuild, GamePlayerList> servers = new Dictionary<IGuild, GamePlayerList>();

        private DiscordSocketClient _client;
        private CommandService commands;
        private IServiceProvider services;
        
        internal static Dictionary<IGuild, GamePlayerList> Servers
        {
            get
            {
                return servers;
            }

            set
            {
                servers = value;
            }
        }

        public async Task MainAsync()
        {
            _client = new DiscordSocketClient(new DiscordSocketConfig {LogLevel = LogSeverity.Info, MessageCacheSize = 100});
            commands = new CommandService(new CommandServiceConfig {CaseSensitiveCommands = false, LogLevel = LogSeverity.Info});
            
            _client.Log += Log;
            _client.Ready += Ready;

            services = new ServiceCollection().BuildServiceProvider();

            await InstallCommands();

            await _client.LoginAsync(TokenType.Bot, Sneaky.BotToken);
            await _client.StartAsync();

            await Task.Delay(-1);
        }

        private Task Ready()
        {
            foreach (SocketGuild guild in _client.Guilds)
            {
                servers[guild] = new GamePlayerList();
            }
            return Task.CompletedTask;
        }

        public async Task InstallCommands()
        {
            _client.MessageReceived += HandleCommand;

            await commands.AddModulesAsync(Assembly.GetEntryAssembly());

            Console.WriteLine("List of Commands on boot-up:");
            foreach (CommandInfo command in commands.Commands)
            {
                if(command.Attributes.All(a => !(a is HiddenAttribute)))
                Console.WriteLine("!" + command.Name);
            }
            Console.WriteLine("---");

        }

        public async Task HandleCommand(SocketMessage arg)
        {
            var message = arg as SocketUserMessage;
            if (message == null) return;

            int argPos = 0;

            if (!(message.HasCharPrefix('!', ref argPos) || message.HasMentionPrefix(_client.CurrentUser, ref argPos))) return;

            var context = new CommandContext(_client, message);

            var result = await commands.ExecuteAsync(context, argPos, services);
            if (!result.IsSuccess) Console.WriteLine(result.ErrorReason);
        }

        private Task Log(LogMessage arg) // if I want to log further change this up.
        {
            Console.WriteLine(arg.ToString());
            return Task.CompletedTask;
        }
    }
}
