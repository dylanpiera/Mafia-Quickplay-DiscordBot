using Discord;
using Discord.Commands;
using System;
using System.Threading.Tasks;

namespace DiscordBot.Commands
{
    static class UnVoteCommand
    {
        public static void createCommand(DiscordClient _client)
        {
            _client.GetService<CommandService>().CreateCommand("unvote")
                .Parameter("", ParameterType.Unparsed)
                .Description("Removes your current vote.")
                .Do(e =>
                {
                    try
                    {
                        Program.servers[e.Server].Find(e.User).LynchTarget = null;
                    } catch(Exception) { }
                });
        }
    }
}