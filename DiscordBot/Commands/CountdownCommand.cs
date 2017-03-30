using Discord;
using Discord.Commands;
using DiscordBot.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBot.Commands
{
    class CountdownCommand
    {
        public static bool countdownRunning = false;

        public static void createCommand(DiscordClient _client)
        {
            _client.GetService<CommandService>().CreateCommand("countdown")
                .Parameter("time", ParameterType.Required)
                .Description("Countsdown from [time] in seconds")
                .Hide()
                .Do(async e =>
                {
                    if (!countdownRunning)
                    {
                        countdownRunning = true;
                        int timer, i = 1;
                        string arg = e.Args.FirstOrDefault<string>();

                        try
                        {
                            timer = Convert.ToInt32(arg);
                        }
                        catch (Exception)
                        {
                            return;
                        }

                        Message message = await e.Channel.SendMessage(e.User.Mention + " Countdown " + timer + "!");

                        do
                        {
                            await Task.Delay(TimeConverter.SecToMS(1));
                            await message.Edit(e.User.Mention + " Countdown " + (timer - i) + "!");
                            i++;

                        } while (i < timer);
                        await Task.Delay(TimeConverter.SecToMS(1));
                        await message.Edit(e.User.Mention + " Countdown Ended!");
                        countdownRunning = false;
                    } else
                    {
                        await e.Message.Delete();
                    }
                });
        }
    }
}