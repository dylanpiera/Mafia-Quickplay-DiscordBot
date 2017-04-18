using Discord;
using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBot.Commands {
    class SpectatorCommand {
        public static void createCommand(DiscordClient _client) {
            _client.GetService<CommandService>().CreateCommand("spectate")
                .Parameter("username", ParameterType.Multiple)
                .Description("Spectate the mafia game in the current server.\n\n**Admin Only:** make a user with the name `username` spectate in the game.\n_Optional Admin Parameters:_ `--force`")
                .Do(async e => {
                    //and if a parameter was passed along by an Admin using this command...
                    if(e.Args.Length != 0 && e.User.ServerPermissions.Administrator) {
                        //Then loop trough each mentioned user...
                        foreach(var spectator in e.Message.MentionedUsers) {
                            //and check if they are not a bot (or if the command was forced)...
                            if(!spectator.IsBot || (e.Args.Contains<string>("--force") && spectator.Id != _client.CurrentUser.Id)) {
                                //and see if they are not already in the game
                                if(!Program.servers[e.Server].isSpectating(spectator)) {
                                    //add them to the game
                                    Program.servers[e.Server].AddSpectate(spectator);
                                    await e.Channel.SendMessage(e.User.Mention + " made " + spectator.Mention + " a spectator :white_check_mark: ");
                                } else {
                                    //await e.Channel.SendMessage(e.User.Mention + " attempted to add : " + player.Mention + " to the queue, but they already were in!");
                                }
                            } else {
                                if(spectator.Id == _client.CurrentUser.Id) {
                                    await e.Channel.SendMessage("ME? Spectating? I'm sorry, but I just preffer hosting and writing FT. :no_entry_sign:");
                                    return;
                                }
                                await e.Channel.SendMessage("I'm sorry, But bots aren't allowed to spectate the game. They'd have no use there. :no_entry_sign:");
                            }
                        }
                    } else {
                        if(!Program.servers[e.Server].isSpectating(e.User)) {
                            Program.servers[e.Server].AddSpectate(e.User);
                            await e.Channel.SendMessage(e.User.Mention + " started spectating the game! :white_check_mark: ");
                            return;
                        }
                        await e.Channel.SendMessage(e.User.Mention + " you're already spectating the game! :x:");
                    }
                });
        }
    }
}