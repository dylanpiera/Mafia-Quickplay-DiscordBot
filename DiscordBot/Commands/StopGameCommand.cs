using Discord;
using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBot.Commands
{
    class StopGameCommand
    {
        public static void createCommand(DiscordClient _client)
        {
            _client.GetService<CommandService>().CreateCommand("stopGame")
                .Description("Stops the Current game [Admin Only]")
                .Do(async e =>
                {
                    if (e.User.ServerPermissions.Administrator && Program.servers[e.Server].gameRunning)
                    {
                        Message m = await e.Channel.SendMessage(e.User.Mention + " Stopping the current game...");
                        await Task.Delay(TimeConverter.SecToMS(1));
                        Program.servers[e.Server].Reset();
                        try {
                            await e.Channel.SendMessage($":warning: {e.User.Mention} stopped the ongoing mafia game! :warning:");
                        } catch (Exception)
                        {
                            await e.Server.FindChannels("general").FirstOrDefault().SendMessage($":warning: {e.User.Mention} stopped the ongoing mafia game! :warning:");
                        }
                    }
                });
        }
    }
}
