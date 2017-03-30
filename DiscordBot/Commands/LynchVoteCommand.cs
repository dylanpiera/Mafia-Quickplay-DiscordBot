using Discord;
using Discord.Commands;
using System.Threading.Tasks;

namespace DiscordBot.Commands {
    static class LynchVoteCommand {
        public static void createCommand(DiscordClient _client) {
            _client.GetService<CommandService>().CreateCommand("vote")
                .Parameter("lynchee", ParameterType.Multiple)
                .Description("Place your lynch vote on `[lynchee]`")
                .Do(async e => {
                    var game = Program.servers[e.Server];
                    if(game.inGame(e.User) && game.Phase == Util.Phases.Day && e.Channel == game.GameChat) {
                        foreach(var item in e.Message.MentionedUsers) {
                            if(game.inGame(item)) {
                                game.Find(e.User).LynchTarget = game.Find(item);
                                return;
                            }
                        }
                        await e.User.SendMessage($"{e.User.Name} you vote wasn't accepted. Are you sure you mentioned your target?");
                        return;
                    }
                });
        }
    }
}