using Discord;
using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBot.Commands
{
    static class LeaveCommand
    {
        public static void createCommand(DiscordClient _client)
        {
            _client.GetService<CommandService>().CreateCommand("leave")
                .Parameter("username",ParameterType.Multiple)
                .Description("Leave the mafia game in the current server.\n\n**Admin only:** Kick a user with the name `username` from the game.\n_Optional Admin Parameters:_ `--silent`, `--force`")
                .Do(async e =>
                {
                    //As long as there is an argument and the caller of this command has Administrator Permission
                    if (e.Args.Length != 0 && e.User.ServerPermissions.Administrator)
                    {
                        //Loop trough all mentioned players
                        foreach (var player in e.Message.MentionedUsers)
                        {
                            //And if the player isn't a bot
                            if ((!player.IsBot && !Program.servers[e.Server].gameRunning) || e.Args.Contains<string>("--force"))
                            {
                                //And is in game
                                if (Program.servers[e.Server].inGame(player))
                                {
                                    //Remove them and broadcast they're removed, unless parameter "--silent" is passed along
                                    Program.servers[e.Server].Remove(player);

                                    if (!e.Args.Contains<string>("--silent"))
                                    {
                                        await e.Channel.SendMessage($":warning: {e.User.Mention} Removed: {player.Mention} from the game! :warning:");
                                    }
                                }
                                //else if(!e.Args.Contains<string>("--silent")) await e.Channel.SendMessage(e.User.Mention + " attempted to remove: " + player.Name + " but they are not in the current game!");
                            }
                            else
                            {
                                await e.User.CreatePMChannel();
                                await e.User.PrivateChannel.SendMessage(":warning: You can not remove users from an ongoing game or bots without the `--force` parameter! :warning:");
                                return;
                            }
                        }

                        if (!e.Args.Contains("@") && !e.Args.Contains("--silent"))
                        {
                            await e.User.CreatePMChannel();
                            await e.User.SendMessage(":warning: Please mention the user! :warning:");
                        }
                        return;
                    }
                    else
                    {
                        if (!Program.servers[e.Server].gameRunning)
                        {
                            if (Program.servers[e.Server].inGame(e.User))
                            {
                                Program.servers[e.Server].Remove(e.User);
                                await e.Channel.SendMessage(e.User.Mention + " has left the game! :heavy_check_mark:");
                                return;
                            }

                            await e.Channel.SendMessage(e.User.Mention + " you ain't in the game! :x:");
                        }
                        else
                        {
                            await e.User.CreatePMChannel();
                            await e.User.PrivateChannel.SendMessage(":no_entry_sign: You can not leave a game in progress. :no_entry_sign:");
                        }
                    }
                });
        }
    }
}
