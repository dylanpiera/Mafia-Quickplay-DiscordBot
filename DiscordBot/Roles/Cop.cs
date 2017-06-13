using Discord;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBot.Roles
{
    class Cop : MafiaRole
    {
        public Cop(string username) :base("Cop","Every night you can target someone, you will learn their alignment.\n\nTo target someone use the command `!target [Target]` in your PM at night!")
        {
            this.rolePM = $"Dear **{username}**,\nYou are the **{Title}**.\n\n{description}\n\nYou win with the **Town** whose goal is to defeat all members of the Mafia.";
        }

        public async override Task<bool> Power(Channel chat)
        {
            if(this.Target != null)
            {
                if(this.Target.Role.Title == "Godfather") {
                    await chat.SendMessage("Cop check result: Town"/* + Allignment.Town.ToString()*/);
                } else if(this.Target.Role.Title == "Miller") {
                    await chat.SendMessage("Cop check result: Mafia"/* + Allignment.Mafia.ToString()*/);
                } else {
                    await chat.SendMessage("Cop check result: " + this.Target.Role.Allignment.ToString());
                }
            }
        }

        public override async Task<bool> powerResult(User user, Player target)
        {
            if (target.Role.Title == "Godfather")
                await user.SendMessage($"You checked {target.User.Name}, they are: {RoleUtil.Allignment.Town.ToString()}");
            else
                await user.SendMessage($"You checked {target.User.Name}, they are: {target.Role.Allignment}");

            this.Target = null;
            return true;
        }
    }
}
