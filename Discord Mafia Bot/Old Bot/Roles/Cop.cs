using Discord;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DiscordBot.Game;

namespace DiscordBot.Roles
{
    class Cop : MafiaRole
    {
        public Cop(string username) :base("Cop","Every night you can target someone, you will learn their alignment.\n\nEach night you can target someone by saying: `SCAN: [playername]` in your PM!")
        {
            this.rolePM = $"Dear **{username}**,\nYou are the **{Title}**.\n\n{description}\n\nYou win with the **Town** whose goal is to defeat all members of the Mafia.";
            power = "You can now select your cop target. Please do so in the following format: `SCAN: [playername]`";
            PowerRole = true;
        }

        protected override async void powerHandler(object s, MessageEventArgs e, GamePlayerList g)
        {
            if (e.Message.RawText.StartsWith("SCAN: ") && e.Channel.Id == e.User.PrivateChannel.Id)
            {
                string target = e.Message.RawText.Replace("SCAN: ", "");
                if(g.inGame(g.Find(target)))
                {
                    Target = g.Find(target);
                    if(Target.User.Nickname != null)
                        await e.User.SendMessage($"You will be scanning: {Target.User.Nickname} tonight. Use `SCAN: [playername]` to change your target.");
                    else
                        await e.User.SendMessage($"You will be scanning: {Target.User.Name} tonight. Use `SCAN: [playername]` to change your target.");
                } else
                {
                    await e.User.SendMessage($"Your input was invalid. You inputted: {target}");
                }
            }
        }

        public override async Task<bool> powerResult(User user, Player target)
        {
            try
            {
                if (target.Role.Title == "Godfather")
                    await user.SendMessage($"You checked {target.User.Name}, they are: {RoleUtil.Allignment.Town.ToString()}");
                else if (target.Role.Title == "Miller")
                    await user.SendMessage($"You checked {target.User.Name}, they are: {RoleUtil.Allignment.Mafia.ToString()}");
                else
                    await user.SendMessage($"You checked {target.User.Name}, they are: {target.Role.Allignment}");
            } catch(NullReferenceException)
            {
                await user.SendMessage("You didn't target anyone last night.");
            }
            return await base.powerResult(user, target);
        }

    }
}
