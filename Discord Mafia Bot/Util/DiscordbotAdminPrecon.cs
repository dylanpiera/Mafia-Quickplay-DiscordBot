using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord_Mafia_Bot.Util
{
    class DiscordbotAdminPrecon : PreconditionAttribute
    {
#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        public async override Task<PreconditionResult> CheckPermissions(ICommandContext context, CommandInfo command, IServiceProvider services)
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
        {
            List<ulong> admins = new List<ulong>() {221314609255153675, 135735651059499008, 22162098568455782, 117878923370430464};

            if (admins.Contains(context.User.Id))
                return PreconditionResult.FromSuccess();
            else
                return PreconditionResult.FromError($"{context.User.Username} tried to use {command.Name} but isn't an admin of this bot.");
        }
    }
}
