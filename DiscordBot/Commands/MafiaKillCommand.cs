using Discord;
using Discord.Commands;
using System.Threading.Tasks;

namespace DiscordBot.Commands {
    static class MafiaKillCommand {
        public static void createCommand(DiscordClient _client) {
            _client.GetService<CommandService>().CreateCommand("kill")
                .Hide()
                .Parameter("target", ParameterType.Multiple)
                .Description("Targets the `[target]` for the mafia nightkill\n**[Mafia Chat Only] [Night Only]**")
                .Do(async e => {
                    var g = Program.servers[e.Server];
                    if(g.gameRunning && g.Phase == Util.Phases.Night && e.Channel == g.MafiaChat) {
                        foreach(var item in e.Message.MentionedUsers) {
                            if(g.inGame(item)) {
                                g.MafiaKillTarget = g.Find(item);
                                await e.Channel.SendMessage($"{item.Name} is the current Kill Target.");
                                return;
                            }
                        }
                        await e.Channel.SendMessage($"{e.User.Mention} Please mention the target.");
                        return;
                    }
                });
        }
    }
}