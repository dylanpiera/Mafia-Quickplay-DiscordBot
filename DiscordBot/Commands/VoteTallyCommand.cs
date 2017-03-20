using Discord;
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
                .Do(async e =>
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

                    string playerList = "";
                    List<Player> SortedList = Program.servers[e.Server].Objects.OrderByDescending(o => o.VotesOn).ToList();
                    foreach (var item in SortedList)
                    {
                        try
                        {
                            playerList += $"{item.User.Name} " + item.VotesOn + ": " + votedFor(SortedList, item) + "\n";
                        } catch(Exception) { }
                    }

                    await e.Channel.SendMessage(e.User.Mention + "\n\n**Players in current game:**\n```" + playerList + "```");
                    await Task.Delay(0);
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
