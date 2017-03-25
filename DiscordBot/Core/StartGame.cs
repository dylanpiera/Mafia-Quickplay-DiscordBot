using Discord;
using Discord.Commands;
using DiscordBot.Game;
using DiscordBot.Roles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBot.Core
{
    static class StartGame
    {
        public static async void startGame(CommandEventArgs e, GamePlayerList g)
        {
            //Create a chatroom in the server with all players in it.
            Channel channel = await e.Server.CreateChannel("Mafia-Game-Room", ChannelType.Text);

            await channel.AddPermissionsRule(e.Server.CurrentUser, new ChannelPermissionOverrides(readMessages: PermValue.Allow));
            await channel.AddPermissionsRule(e.Server.EveryoneRole, new ChannelPermissionOverrides(readMessages: PermValue.Deny));

            foreach (var item in g.Objects)
            {
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
            foreach (var item in g.Objects)
            {
                item.Role.sendRolePM(item.User);
            }
            await m2.Edit("Sent role pm's! :white_check_mark:");
            await Task.Delay(500);

            //Create a chatroom with just the mafia members in it.
            Message m3 = await channel.SendMessage("Creating Mafia Chat...");
            Channel mafiaChat = await e.Server.CreateChannel("Mafia-Night-Chat", ChannelType.Text);
            await mafiaChat.AddPermissionsRule(e.Server.CurrentUser, new ChannelPermissionOverrides(readMessages: PermValue.Allow));
            await mafiaChat.AddPermissionsRule(e.Server.EveryoneRole, new ChannelPermissionOverrides(readMessages: PermValue.Deny));
            foreach (var item in g.Objects)
            {
                if (item.Role.Allignment == Roles.RoleUtil.Allignment.Mafia)
                    await mafiaChat.AddPermissionsRule(item.User, new ChannelPermissionOverrides(readMessages: PermValue.Allow, sendMessages: PermValue.Allow));
            }
            await mafiaChat.SendMessage($"Welcome scummy friends :smiling_imp:, You can freely discuss in this chat during both day & night phases.\nOnce night hits I will make an announcement for you to post your Night Kill Target.");
            await m3.Edit("Created Mafia Chat! :white_check_mark:");
            await Task.Delay(500);

            //Start day 1, and allow everyone to speak.
            await channel.SendMessage($":sunny: @everyone **It is now day 1.** Day 1 will end in {g.PhaseLengthInMin} minutes. :sunny:");
            await Task.Delay(100);
            foreach (var item in g.Objects)
            {
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
        private static void distributeRoles(GamePlayerList g)
        {
            //75% of the players are town, 25% are mafia (rounded up and down respectively)
            int playerCount = g.Objects.Count;
            int townPlayers = ((int)Math.Ceiling(playerCount * 0.75)), mafiaPlayers = ((int)Math.Floor(playerCount * 0.25));
            //In the case of a 3 player game (where there'd be no mafia) remove 1 town add 1 mafia.
            if (mafiaPlayers == 0)
            {
                townPlayers--;
                mafiaPlayers++;
            }
            int town = 0, mafia = 0;
            Random random = new Random();

            foreach (var item in g.Objects)
            {
                if (town < townPlayers && mafia < mafiaPlayers)
                {
                    switch (random.Next(1, 3))
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
                else if (town < townPlayers)
                {
                    item.AssignRole(new Vanilla(Roles.RoleUtil.Allignment.Town, item.User.Name));
                    town++;
                }
                else
                {
                    item.AssignRole(new Vanilla(Roles.RoleUtil.Allignment.Mafia, item.User.Name));
                    mafia++;
                }
            }
        }
    }
}
