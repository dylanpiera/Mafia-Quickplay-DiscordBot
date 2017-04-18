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
            if (g.inGame(t))
            {
                try
                {
                    user.Role.Target = t;
                    Console.WriteLine(user.User.Name + " targeted: " + t.User.Name);
                    await e.Channel.SendMessage($":white_check_mark: your current target is: {user.Role.Target.User.Name}");
                    return true;
                } catch(Exception)
                {
                    await e.Channel.SendMessage("Something went wrong.");
                    return true;
                }
            } else
            {
                await e.Channel.SendMessage("Please provide the full name (Not the mention!) of the target.");
                return false;
            }
            
        }
    }
}
