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
            StartGameCommand.createCommand(_client);
            LynchVoteCommand.createCommand(_client);
            UnVoteCommand.createCommand(_client);
            VoteTallyCommand.createCommand(_client);
            PlayerListCommand.createCommand(_client);
            StopGameCommand.createCommand(_client);

            //Bot Invite Link Storage
            _client.GetService<CommandService>().CreateCommand("inviteLink").Hide().Do(async e => { if(e.User.Id == 135735651059499008 || e.User.Id == 221620985684557826 || e.User.Id ==117878923370430464) await e.Channel.SendMessage(Sneaky.botInvite); } );
        }
    }
}
