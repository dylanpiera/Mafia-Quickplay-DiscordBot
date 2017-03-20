using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using DiscordBot.Core;

namespace DiscordBot.Commands
{
    static class CommandInitializer
    {
        public static void init(DiscordClient _client)
        {
            PingCommand.createCommand(_client);
            JoinCommand.createCommand(_client);
            LeaveCommand.createCommand(_client);
            PlayerListCommand.createCommand(_client);
            CountdownCommand.createCommand(_client);

            _client.GetService<CommandService>().CreateCommand("argThrower").Parameter("",ParameterType.Multiple).Hide().Do(async e =>
            {
                if (e.Args.Length <= 10)
                {
                    await e.Channel.SendIsTyping();
                    foreach (var arg in e.Args)
                    {
                        await e.Channel.SendMessage(arg);
                    }
                    await e.Channel.SendMessage("Done throwing.");
                }

            });

            //Bot Invite Link Storage
            _client.GetService<CommandService>().CreateCommand("inviteLink").Hide().Do(async e => { if(e.User.ServerPermissions.Administrator) await e.Channel.SendMessage(Sneaky.botInvite); } );
        }
    }
}
