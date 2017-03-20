using Discord.Commands;
using Discord;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBot.Commands
{
    class PlayerListCommand
    {
        public static void createCommand(DiscordClient _client)
        {
            _client.GetService<CommandService>().CreateCommand("playerList")
                .Alias(new String[] {"list","players"})
                .Description("Get a list of people currently in the mafia game on the current server")
                .Do(async e =>
                {
                    //If the game hasn't started yet, show the signup list
                    if (!Program.servers[e.Server].gameRunning)
                    {
                        if (Program.servers[e.Server].Objects.Count > 0)
                        {
                            string playerList = "";
                            int i = 0;
                            foreach (var item in Program.servers[e.Server].Objects)
                            {
                                i++;
                                playerList += i + ". " + item.User.Name + "\n";
                            }

                            await e.Channel.SendMessage(e.User.Mention + "\n\n**Players in current game:**```" + playerList + "```");
                            return;
                        }
                        else await e.Channel.SendMessage(e.User.Mention + " There is nobody currently in the game!");
                    }
                    else
                    {
                        string playerList = "";
                        int i = 0;
                        foreach (var item in Program.servers[e.Server].Objects)
                        {
                            i++;
                            try
                            {
                                playerList += i + ". " + item.User.Name + " Votes: " + item.LynchTarget.User.Name + "\n";
                            }
                            catch (Exception)
                            {
                                playerList += i + ". " + item.User.Name + " Votes: -\n";
                            }
                        }
                        await e.Channel.SendMessage(e.User.Mention + "\n\n**Players in current game:**```" + playerList + "```");
                        return;
                    }
                });
        }
    }
}
