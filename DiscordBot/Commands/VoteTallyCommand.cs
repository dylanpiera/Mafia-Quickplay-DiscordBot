﻿using Discord;
using Discord.Commands;
using DiscordBot.Game;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DiscordBot.Commands
{
    static class VoteTallyCommand
    {
        public static void createCommand(DiscordClient _client)
        {
            _client.GetService<CommandService>().CreateCommand("votecount")
                .Alias(new string[] { "tally" })
                .Description("Shows the total votes on all players.")
                .Do(async e =>
                {
                    if (Program.servers[e.Server].gameRunning)
                    {
                        foreach (var item in Program.servers[e.Server].Objects)
                        {
                            item.VotesOn = 0;
                        }
                        foreach (var item in Program.servers[e.Server].Objects)
                        {
                            try
                            {
                                item.LynchTarget.VotesOn++;
                            }
                            catch (Exception)
                            {

                            }
                        }
                        int i = 0;
                        string playerList = "";
                        List<Player> SortedList = Program.servers[e.Server].Objects.OrderByDescending(o => o.VotesOn).ToList();
                        foreach (var item in SortedList)
                        {
                            i++;
                            try
                            {
                                playerList += $"{i}. {item.User.Name} " + item.VotesOn + ": " + votedFor(SortedList, item) + "\n";
                            }
                            catch (Exception) { }
                        }

                        await e.Channel.SendMessage(e.User.Mention + "\n\n**Current Vote Count::**\n```" + playerList + "```");
                    }
                });
        }

        public static string votedFor(List<Player> list,Player lynchee)
        {
            string s = "";

            foreach (var item in list)
            {
                if(item.LynchTarget == lynchee)
                {
                    s += item.User.Name + " ";
                }
            }

            return s;
        }
    }
}
