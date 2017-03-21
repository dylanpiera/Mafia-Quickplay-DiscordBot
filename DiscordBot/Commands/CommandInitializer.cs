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
            //CountdownCommand.createCommand(_client);
            StopGameCommand.createCommand(_client);
            StartGameCommand.createCommand(_client);
            LynchVoteCommand.createCommand(_client);
            UnVoteCommand.createCommand(_client);
            VoteTallyCommand.createCommand(_client);
            

            //Bot Invite Link Storage
            _client.GetService<CommandService>().CreateCommand("inviteLink").Hide().Do(async e => { if(e.User.ServerPermissions.Administrator) await e.Channel.SendMessage(Sneaky.botInvite); } );
        }
    }
}
