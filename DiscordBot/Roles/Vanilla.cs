using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using DiscordBot.Roles.RoleUtil;

namespace DiscordBot.Roles {
    class Vanilla : MafiaRole {
        public Vanilla(string username) : this(Allignment.Town, username) {
        }
        public Vanilla(Allignment allignment, string username)
        {
            if(allignment == Allignment.Town && copCorrect == true) {
                this.Title = "Vanilla";

                this.description = "You do not have any night actions.";
                this.allignment = Allignment.Town;
                this.wincon = Wincon.DefeatMafia;
                this.rolePM = $"Dear **{username}**,\n\nYou are the most basic of roles in excistence,\nYou are the **{Title}**.\n\n{description}\n\nYou win with the **Town** whose goal is to defeat all members of the Mafia.";
            }
            if(allignment == Allignment.Town && copCorrect == false) {
                this.Title = "Miller";

                this.description = "You do not have any night actions.";
                this.allignment = Allignment.Town;
                this.wincon = Wincon.DefeatMafia;
                this.rolePM = $"Dear **{username}**,\n\nYou are the **{Title}**.\n\n{description}\n\nYou win with the **Town** whose goal is to defeat all members of the Mafia.";
            }
            if(allignment == Allignment.Mafia && copCorrect == true)
            {
                this.Title = "Goon";
                this.description = "You have the power to vote in the Mafia Chat every night on whom to kill.";
                this.allignment = Allignment.Mafia;
                this.wincon = Wincon.OutnumberTown;
                this.rolePM = $"Dear **{username}**,\n\nYou are the most basic of the mafia-aligned roles in excistence,\nYou are the **{Title}**.\n\n{description}\n\nYou win with the **Mafia** whose goal is to outnumber all members of the Town";
            }
            if(allignment == Allignment.Mafia && copCorrect == false) {
                this.Title = "Godfather";
                this.description = "You have the power to vote in the Mafia Chat every night on whom to kill.";
                this.allignment = Allignment.Mafia;
                this.wincon = Wincon.OutnumberTown;
                this.rolePM = $"Dear **{username}**,\n\nYou are the boss of the mafia-aligned roles,\nYou are the **{Title}**.\n\n{description}\n\nYou win with the **Mafia** whose goal is to outnumber all members of the Town";
            }
        }
        public Vanilla(string title, string description, string rolePM) :base(title, description, Allignment.Mafia, Wincon.OutnumberTown, rolePM)
        {
        }
    }
}   