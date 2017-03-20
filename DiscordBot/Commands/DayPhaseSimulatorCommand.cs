using Discord;
using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBot.Commands
{
    class DayPhaseSimulatorCommand
    {
        public static void createCommand(DiscordClient _client)
        {
            //This command purely was there to simulate if the bot would acurratly wait 10 min for the phase and post updates.
            _client.GetService<CommandService>().CreateCommand("dayPhaseSim")
                .Alias(new string[] {"dps"})
                .Hide()
                .Do(async e =>
                {
                    await e.Channel.SendMessage("@everyone Day phase has begun, the day phase will last 10 minutes.");
                    await Task.Delay((1000 * 60 * 5));
                    await e.Channel.SendMessage("@everyone There are 5 minutes left in the day phase!");
                    await Task.Delay((1000 * 60 * 5));
                    await e.Channel.SendMessage("@everyone Day phase has ended!");
                });
        }
    }
}
