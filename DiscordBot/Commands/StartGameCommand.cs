using Discord;
using Discord.Commands;
using DiscordBot.Core;
using DiscordBot.Game;
using DiscordBot.Roles;
using DiscordBot.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBot.Commands
{
    partial class StartGameCommand
    {
        public static void createCommand(DiscordClient _client)
        {
            _client.GetService<CommandService>().CreateCommand("ready")
                .Alias("startGame")
                .Parameter("...", ParameterType.Multiple)
                .Description("Declare that you're ready for the game, once everyone is ready the game will start.\n\n**Admin Parameters:**\n `--force` = starts the game even if not everyone is ready.")
                .Do(async e =>
                {
                    var game = Program.servers[e.Server];

                    if (!game.gameRunning)
                    {
                        if (e.Args.Contains<string>("--force") && e.User.ServerPermissions.Administrator && game.Objects.Count > 2)
                        {
                            await e.Channel.SendMessage("@everyone The game has been started by a moderator! Preparing to launch...");
                            game.gameRunning = true;
                            await Task.Delay(TimeConverter.SecToMS(2));
                            StartGame.startGame(e, game);
                        }
                        else if (e.Args.Contains<string>("--force") && e.User.ServerPermissions.Administrator && game.Objects.Count <= 2)
                        {
                            await e.User.CreatePMChannel();
                            await e.User.SendMessage(":no_entry_sign: **The game has less than 3 users. Game cannot be force-started** :no_entry_sign:");
                        }
                        else
                        {
                            if (game.inGame(e.User))
                            {
                                var player = game.Find(e.User);
                                if (!player.Ready)
                                {
                                    bool everyoneReady = player.readyUp(game);
                                    await e.Channel.SendMessage(e.User.Mention + " is ready! :white_check_mark:");

                                    if (everyoneReady && game.Objects.Count > 2)
                                    {
                                        await e.Channel.SendMessage("@everyone Everyone is ready! Starting game now...");
                                        game.gameRunning = true;
                                        await Task.Delay(TimeConverter.SecToMS(2));
                                        StartGame.startGame(e, game);
                                    } else if(everyoneReady)
                                    {
                                        await e.Channel.SendMessage(":warning: Atleast 3 people are required to start the game! :warning:");
                                    }
                                }
                                else
                                {
                                    player.Ready = false;
                                    await e.Channel.SendMessage(e.User.Mention + " is no longer ready. :x:");
                                }
                            }
                            else
                            {
                                await e.Channel.SendMessage(":no_entry_sign: " + e.User.Mention + " You're not in the game! Please join first by typing !join :no_entry_sign:");
                            }
                        }
                    } 
                });
        }
    }
}
