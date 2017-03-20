using Discord;
using Discord.Commands;
using DiscordBot.Game;
using DiscordBot.Roles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBot.Commands
{
    class StartGameCommand
    {
        private static async void StartGame(CommandEventArgs e, GamePlayerList g)
        {
            Channel channel =  await e.Server.CreateChannel("Mafia-Game-Room", ChannelType.Text);
            await channel.AddPermissionsRule(e.Server.EveryoneRole, new ChannelPermissionOverrides(readMessages: PermValue.Deny));

            foreach (var item in g.Objects)
            {
                await channel.AddPermissionsRule(item.User, new ChannelPermissionOverrides(readMessages: PermValue.Allow, sendMessages: PermValue.Deny));
            }
            
            await channel.SendMessage("@everyone the game is starting up...");
            await Task.Delay(500);
            Message m1 = await channel.SendMessage("Distributing roles now...");
            await channel.SendIsTyping();
            distributeRoles(channel,g);
            await m1.Edit("Distributing roles now... :white_check_mark:");
            await Task.Delay(500);

            Message m2 = await channel.SendMessage("Sending role pm's...");
            foreach (var item in g.Objects)
            {
                item.Role.sendRolePM(item.User);
            }
            await m2.Edit("Sending role pm's... :white_check_mark:");
            await Task.Delay(500);

            Message m3 = await channel.SendMessage("Creating Mafia Chat...");
            Channel mafiaChat = await e.Server.CreateChannel("Mafia-Night-Chat", ChannelType.Text);
            await mafiaChat.AddPermissionsRule(e.Server.EveryoneRole, new ChannelPermissionOverrides(readMessages: PermValue.Deny));
            foreach (var item in g.Objects)
            {
                if(item.Role.Allignment == Roles.RoleUtil.Allignment.Mafia)
                await mafiaChat.AddPermissionsRule(item.User, new ChannelPermissionOverrides(readMessages: PermValue.Allow, sendMessages: PermValue.Allow));
            }
            await mafiaChat.SendMessage($"Welcome scummy friends :smiling_imp:, You can freely discuss in this chat during both day & night phases.\nOnce night hits I will make an announcement for you to post your Night Kill Target.");
            await m3.Edit("Creating Mafia Chat... :white_check_mark:");
            await Task.Delay(500);

            await channel.SendMessage(":sunny: @everyone **It is now day 1.** Day 1 will end in 15 minutes. :sunny:");
            await Task.Delay(100);
            foreach (var item in g.Objects)
            {
                await channel.AddPermissionsRule(item.User, new ChannelPermissionOverrides(readMessages: PermValue.Allow, sendMessages: PermValue.Allow));
            }

            g.setChats(channel, mafiaChat);

        }

        private static async void distributeRoles(Channel c,GamePlayerList g)
        {
            int playerCount = g.Objects.Count;
            int townPlayers = ((int) Math.Ceiling(playerCount * 0.75)), mafiaPlayers = ((int)Math.Floor(playerCount * 0.25));
            if(mafiaPlayers == 0)
            {
                townPlayers--;
                mafiaPlayers++;
            }
            int town = 0, mafia = 0;
            Random random = new Random();

            foreach (var item in g.Objects)
            {
                if(town < townPlayers && mafia < mafiaPlayers)
                {
                    switch(random.Next(1, 3))
                    {
                        case 1:
                            item.AssignRole(new Vanilla(Roles.RoleUtil.Allignment.Town, item.User.Name));
                            town++;
                            break;
                        case 2:
                            item.AssignRole(new Vanilla(Roles.RoleUtil.Allignment.Mafia, item.User.Name));
                            mafia++;
                            break;
                    }
                }
                else if(town < townPlayers)
                {
                    item.AssignRole(new Vanilla(Roles.RoleUtil.Allignment.Town, item.User.Name));
                    town++;
                } else
                {
                    item.AssignRole(new Vanilla(Roles.RoleUtil.Allignment.Mafia, item.User.Name));
                    mafia++;
                }
            }

            await c.SendMessage($":exclamation: Debug! :exclamation:\nAmount of town = {townPlayers}, amount of mafia = {mafiaPlayers}\nAmount of assigned town = {town}, Amount of assigned mafia = {mafia}");
        }

        public static void createCommand(DiscordClient _client)
        {
            _client.GetService<CommandService>().CreateCommand("ready")
                .Alias("startGame")
                .Parameter("...", ParameterType.Multiple)
                .Description("Declare that you're ready for the game, once everyone is ready the game will start.\n**Admin Parameters:**\n `--force` = starts the game even if not everyone is ready.")
                .Do(async e =>
                {
                    var game = Program.servers[e.Server];

                    if (!game.gameRunning)
                    {
                        if (e.Args.Contains<string>("--force") && e.User.ServerPermissions.Administrator && game.Objects.Count > 2)
                        {
                            await e.Channel.SendMessage("@everyone The game has been started by a moderator! Preparing to launch...");
                            game.gameRunning = true;
                            await Task.Delay(2000);
                            StartGame(e, game);
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
                                        await Task.Delay(2000);
                                        StartGame(e, game);
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
