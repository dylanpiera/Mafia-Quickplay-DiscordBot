using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using DiscordBot.Game;
using DiscordBot.Roles.RoleUtil;

namespace DiscordBot.Roles
{
    class Vanilla : MafiaRole, IMafiaRole
    {
        private VanillaTypes type;

        public Vanilla(Allignment allignment)
        {
            this.allignment = allignment;
            this.type = VanillaTypes.Vanilla;
        }
        public Vanilla(VanillaTypes type)
        {
            this.type = VanillaTypes.Godfather;
        }

        public override MafiaRole SetupRole(Player player)
        {
            this.Player = player;
            if (type == VanillaTypes.Godfather)
            {
                this.Title = "Godfather";
                this.description = "You are the leader of the mafia, and will show up as town when scanned by an investigative role like Cop.\nYou also have the power to vote in the Mafia Chat every night on whom to kill.";
                this.allignment = Allignment.Mafia;
                this.wincon = Wincon.OutnumberTown;
                this.rolePM = $"Dear **{Player.User.Nickname ?? player.User.Username}**,\n\nYou are one of the most basic mafia-aligned roles in existence,\nYou are the **{Title}**.\n\n{description}\n\nYou win with the **Mafia** whose goal is to outnumber all members of the Town";
            }
            else
            {
                if (Allignment == Allignment.Town)
                {
                    this.Title = "Vanilla";
                    this.description = "You do not have any night actions.";
                    this.allignment = Allignment.Town;
                    this.wincon = Wincon.DefeatMafia;
                    this.rolePM = $"Dear **{Player.User.Nickname ?? player.User.Username}**,\n\nYou are the most basic of roles in existence,\nYou are the **{Title}**.\n\n{description}\n\nYou win with the **Town** whose goal is to defeat all members of the Mafia.";
                }
                else if (Allignment == Allignment.Mafia)
                {
                    this.Title = "Goon";
                    this.description = "You have the power to vote in the Mafia Chat every night on whom to kill.";
                    this.allignment = Allignment.Mafia;
                    this.wincon = Wincon.OutnumberTown;
                    this.rolePM = $"Dear **{Player.User.Nickname ?? player.User.Username}**,\n\nYou are the most basic of the mafia-aligned roles in existence,\nYou are the **{Title}**.\n\n{description}\n\nYou win with the **Mafia** whose goal is to outnumber all members of the Town";
                }
            }

            return this;
        }
    }

    public enum VanillaTypes
    {
        Vanilla,
        Godfather
    }
}
