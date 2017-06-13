using Discord;
using Discord.Commands;
using DiscordBot.Game;
using DiscordBot.Resources;
using DiscordBot.Roles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBot.Core {
    static class StartGame {
        public static async void startGame(CommandEventArgs e, GamePlayerList g) {
            //Create a chatroom in the server with all players in it.
            Channel channel = await e.Server.CreateChannel("Mafia-Game-Room", ChannelType.Text);

            await channel.AddPermissionsRule(e.Server.CurrentUser, new ChannelPermissionOverrides(readMessages: PermValue.Allow));
            await channel.AddPermissionsRule(e.Server.EveryoneRole, new ChannelPermissionOverrides(readMessages: PermValue.Deny));

            foreach(var item in g.Objects) {
                await channel.AddPermissionsRule(item.User, new ChannelPermissionOverrides(readMessages: PermValue.Allow, sendMessages: PermValue.Deny));
            }
            await channel.SendMessage("@everyone the game is starting up...");
            await Task.Delay(500);

            //Distribute Roles
            Message m1 = await channel.SendMessage("Distributing roles now...");
            await channel.SendIsTyping();
            distributeRoles(g);
            await m1.Edit("Distributed roles! :white_check_mark:");
            await Task.Delay(500);

            //Send role PM's
            Message m2 = await channel.SendMessage("Sending role pm's...");
            foreach(var item in g.Objects) {
                item.Role.sendRolePM(item.User);
            }
            await m2.Edit("Sent role pm's! :white_check_mark:");
            await Task.Delay(500);

            //Create a chatroom with just the mafia members in it.
            Message m3 = await channel.SendMessage("Creating Mafia Chat...");
            Channel mafiaChat = await e.Server.CreateChannel("Mafia-Night-Chat", ChannelType.Text);
            await mafiaChat.AddPermissionsRule(e.Server.CurrentUser, new ChannelPermissionOverrides(readMessages: PermValue.Allow));
            await mafiaChat.AddPermissionsRule(e.Server.EveryoneRole, new ChannelPermissionOverrides(readMessages: PermValue.Deny));
            foreach(var item in g.Objects) {
                if(item.Role.Allignment == Roles.RoleUtil.Allignment.Mafia)
                    await mafiaChat.AddPermissionsRule(item.User, new ChannelPermissionOverrides(readMessages: PermValue.Allow, sendMessages: PermValue.Allow));
            }
            await mafiaChat.SendMessage($"Welcome scummy friends :smiling_imp:, You can freely discuss in this chat during both day & night phases.\nOnce night hits I will make an announcement for you to post your Night Kill Target.");
            await m3.Edit("Created Mafia Chat! :white_check_mark:");
            await Task.Delay(500);

            //Start day 1, and allow everyone to speak.
            await channel.SendMessage($":sunny: @everyone **It is now day 1.** Day 1 will end in {g.PhaseLengthInMin} minutes. :sunny:");
            await Task.Delay(100);
            foreach(var item in g.Objects) {
                await channel.AddPermissionsRule(item.User, new ChannelPermissionOverrides(readMessages: PermValue.Allow, sendMessages: PermValue.Allow));
            }

            //sets the channel in the game for refference from different objects
            g.setChats(channel, mafiaChat);
            g.Phase = Util.Phases.Day;
            g.PhaseCounter = 1;
            //run the GameManager for continuation of the game.
            GameManager.runGame(g);
        }

        //Randomly distribute the roles.
        private static void distributeRoles(GamePlayerList g) {
            //75% of the players are town, 25% are mafia (rounded up and down respectively)
            int playerCount = g.Objects.Count;

            g.TownPlayers = ((int)Math.Floor(playerCount * 0.75));
            g.MafiaPlayers = ((int)Math.Ceiling(playerCount * 0.25));
            //In the case of a 5 player game (where there'd be 2 mafia) add 1 town and remove 1 mafia.
            if(playerCount == 5 && g.MafiaPlayers == 2)
            {
                g.TownPlayers++;
                g.MafiaPlayers--;
            }
            int i = g.TownPlayers;
            while(i >= 4)
            {
                g.TownPlayers--;
                g.Cops++;
                i -= 4;
            }

            g.TownAlive = 0;
            g.MafiaAlive = 0;

            Player[] players = ListHelper.ShuffleList<Player>(g.Objects).ToArray();
            i = 0;
            do
            {
                players[i].AssignRole(new Vanilla(Roles.RoleUtil.Allignment.Town, players[i].User.Name));
                g.TownAlive++;
                i++;
            } while (g.TownAlive < g.TownPlayers);
            do
            {

                if(g.MafiaPlayers > 1 && g.Godfathers == 0)
                {
                    players[i].AssignRole(new Vanilla("Godfather", "You are the leader of the mafia, and will show up as town when scanned by an investigative role like Cop.\nYou also have the power to vote in the Mafia Chat every night on whom to kill.", $"Dear **{players[i].User.Name}**,\n\nYou are the most basic of roles in existence,\nYou are the **Godfather**.\n\nYou are the leader of the mafia, and will show up as town when scanned by an investigative role like Cop.\nYou also have the power to vote in the Mafia Chat every night on whom to kill.\n\nYou win with the **Mafia** whose goal is to outnumber all members of the Town"));
                    g.MafiaAlive++;
                    g.Godfathers++;
                    i++;
                }
                players[i].AssignRole(new Vanilla(Roles.RoleUtil.Allignment.Mafia, players[i].User.Name));
                g.MafiaAlive++;
                i++;
            } while (g.MafiaAlive < g.MafiaPlayers);
            while (g.Cops > 0)
            {
                players[i].AssignRole(new Cop(players[i].User.Name));
                g.Cops--;
                g.TownAlive++;
                i++;
            }

            g.Objects = players.ToList();

            //Old randomize method, above method should however be smoother and easier to edit. But needs testing
            /*foreach (var item in g.Objects)
            {
                if (g.TownAlive < g.TownPlayers && g.MafiaAlive < g.MafiaPlayers && g.Cops > 0)
                {
                    switch (random.Next(1, 4))
                    {

                        case 1:
                            item.AssignRole(new Vanilla(Roles.RoleUtil.Allignment.Town, item.User.Name));
                            g.TownAlive++;
                            break;
                        case 2:
                            item.AssignRole(new Vanilla(Roles.RoleUtil.Allignment.Mafia, item.User.Name));
                            g.MafiaAlive++;
                            break;
                        case 3:
                            item.AssignRole(new Cop(item.User.Name));
                            g.TownAlive++;
                            g.Cops--;
                            break;
                    }
                } else if (g.Cops > 0)
                {
                    item.AssignRole(new Cop(item.User.Name));
                    g.TownAlive++;
                    g.Cops--;
                }
                else if (g.TownAlive < g.TownPlayers)
                {

                    item.AssignRole(new Vanilla(Roles.RoleUtil.Allignment.Town, item.User.Name));
                    g.TownAlive++;
                } else {
                    item.AssignRole(new Vanilla(Roles.RoleUtil.Allignment.Mafia, item.User.Name));
                    g.MafiaAlive++;
                }
            }*/
        }
    }
}