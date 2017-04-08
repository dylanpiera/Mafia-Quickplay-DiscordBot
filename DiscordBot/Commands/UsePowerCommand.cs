using Discord;
using Discord.Commands;
using System.Threading.Tasks;
using System.Linq;
using DiscordBot.Game;
using System;

namespace DiscordBot.Commands
{
    static class UsePowerCommand
    {
        public static void createCommand(DiscordClient _client)
        {
            _client.GetService<CommandService>().CreateCommand("target")
                .Alias(new string[] { "use" })
                .Parameter("Target", ParameterType.Multiple)
                .Do(async e =>
                {
                    if (!e.Channel.IsPrivate) return;
                    GamePlayerList g = new GamePlayerList();
                    
                    if (Program.servers.Any(x => 
                    {
                        if (x.Value.inGame(e.User.Id))
                        {
                            g = x.Value;
                            return true;
                        }
                        return false;
                    }))
                    {
                        await Task.WhenAll(Cop(e, g));
                    }

                });
        }

        public static async Task<bool> Cop(CommandEventArgs e, GamePlayerList g)
        {
            string target = e.Args.Aggregate((i, j) => i + " " + j);
            Player t = g.Find(target);
            Player user = g.Find(e.User);
            if(g.inGame(t))
            {
                user.Role.Target = t;
            }
            await e.Channel.SendMessage($"{e.User.Mention} your current target is: {g.Find(e.User).Role.Target.User.Name} #Debug");
            return true;
        }
    }
}
