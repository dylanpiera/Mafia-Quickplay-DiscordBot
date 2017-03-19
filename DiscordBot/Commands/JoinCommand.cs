using Discord;
using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBot.Commands
{
    static class JoinCommand
    {
        public static void createCommand(DiscordClient _client)
        {
            _client.GetService<CommandService>().CreateCommand("join")
                .Parameter("username",ParameterType.Multiple)
                .Description("Join the mafia game in the current server if it hasn't started yet.\n\n**Admin Only:** make a user with the name `username` join the game.\n_Optional Admin Parameters:_ `--force`")
                .Do(async e =>
                {
                    //If the game isn't running...
                    if (!Program.servers[e.Server].gameRunning)
                    {
                        //and if a parameter was passed along by an Admin using this command...
                        if (e.GetArg("username") != "" && e.User.ServerPermissions.Administrator)
                        {
                            //Then loop trough each mentioned user...
                            foreach (var player in e.Message.MentionedUsers)
                            {
                                //and check if they are not a bot (or if the command was forced)...
                                if (!player.IsBot || e.Args.Contains<string>("--force"))
                                {
                                    //and see if they are not already in the game
                                    if (!Program.servers[e.Server].inGame(player))
                                    {
                                        //add them to the game
                                        Program.servers[e.Server].Add(player);
                                        await e.Channel.SendMessage(e.User.Mention + " added: " + player.Mention + " to the queue!");
                                        return;
                                    }
                                    else
                                    {
                                        await e.Channel.SendMessage(e.User.Mention + " attempted to add : " + player.Mention + " to the queue, but they already were in!");
                                        return;
                                    }
                                }
                                else
                                {
                                    await e.Channel.SendMessage("I'm sorry, But bots aren't allowed to join the game. They'd be too good.");
                                    return;
                                }
                            }
                        }
                        else
                        {
                            if (!Program.servers[e.Server].inGame(e.User))
                            {
                                Program.servers[e.Server].Add(e.User);
                                await e.Channel.SendMessage(e.User.Mention + " has joined the game!");
                                return;
                            }

                            await e.Channel.SendMessage(e.User.Mention + " you're already in the game!");
                        }
                    } else if (e.Args.Contains<string>("--force"))
                    {
                        await e.User.CreatePMChannel();
                        await e.User.PrivateChannel.SendMessage("The `--force` parameter hasn't been implemented yet to add people to already started games. The current functionality only lets you add bots to the queue.");
                        return;

                    } else
                        await e.Channel.SendMessage(e.User.Mention + " I'm sorry, but the game has already started!");
                });
        }
    }
}
