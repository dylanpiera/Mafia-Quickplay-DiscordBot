using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using DiscordBot.Roles.RoleUtil;

namespace DiscordBot.Roles
{
    class Vanilla : MafiaRole
    {
        public Vanilla() :base()
        {
        }
        public Vanilla(Allignment allignment)
        {
            if(allignment == Allignment.Town)
            {
                this.title = "Vanilla";
                this.description = "The vanilla towny with no actual powers.";
                this.allignment = Allignment.Town;
                this.wincon = Wincon.DefeatMafia;
            }
            if(allignment == Allignment.Mafia)
            {
                this.title = "Mafioso";
                this.description = "The vanilla scum. You have the power to vote in the Mafia Chat every night on whom to kill.";
                this.allignment = Allignment.Mafia;
                this.wincon = Wincon.OutnumberTown;
            }
        }
        

    }
}
