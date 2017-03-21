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
        public Vanilla(string username) :this(Allignment.Town, username)
        {
        }
        public Vanilla(Allignment allignment, string username)
        {
            if(allignment == Allignment.Town)
            {
                this.title = "Vanilla";
                this.description = "You do not have any night actions.";
                this.allignment = Allignment.Town;
                this.wincon = Wincon.DefeatMafia;
                this.rolePM = $"Dear {username},\n\nYou are the most basic of roles in excistence,\nThe {title}.\n\n{description}\n\nYou win with the Town whose goal is to defeat all members of the Mafia.";
            }
            if(allignment == Allignment.Mafia)
            {
                this.title = "Goon";
                this.description = "You have the power to vote in the Mafia Chat every night on whom to kill.";
                this.allignment = Allignment.Mafia;
                this.wincon = Wincon.OutnumberTown;
                this.rolePM = $"Dear {username},\n\nYou are the most basic of mafia-aligned roles in excistence,\nThe {title}.\n\n{description}\n\nYou win with the Mafia whose goal is to outnumber all members of the Town";
            }


        }
    }
}
