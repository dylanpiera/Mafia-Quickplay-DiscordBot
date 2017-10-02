using Discord;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DiscordBot.Game;
using Discord.WebSocket;
using DiscordBot.Roles.RoleUtil;

namespace DiscordBot.Roles
{
    class Cop : MafiaRole, IMafiaRole
    {
        public override MafiaRole SetupRole(Player player)
        {
            base.SetupRole(player, "Cop", "Every night you can target someone, you will learn their alignment.\n\nEach night you can target someone by saying: `SCAN: [playername]` in your PM!");

            this.rolePM = $"Dear **{Player.User.Nickname ?? Player.User.Username}**,\nYou are the **{Title}**.\n\n{description}\n\nYou win with the **Town** whose goal is to defeat all members of the Mafia.";
            power = "You can now select your cop target. Please do so in the following format: `SCAN: [playername]`";
            PowerRole = true;

            return this;
        }

        protected override async Task powerHandler(SocketMessage e, GamePlayerList g)
        {
            if (e.Content.StartsWith("SCAN: ") && e.Channel.Id == (await this.Player.User.GetOrCreateDMChannelAsync() as IMessageChannel).Id)
            {
                string target = e.Content.Replace("SCAN: ", "");
                if (g.InGame(g.Find(target)))
                {
                    Target = g.Find(target);

                    await e.Author.SendMessageAsync("", false, new EmbedBuilder()
                    {
                        Title = "Cop Night Start",
                        Color = Color.Blue,
                        Description = $"You will be scanning: {Target.User.Nickname ?? Target.User.Username} tonight. Use `SCAN: [playername]` to change your target."
                    });
                }
                else
                {
                    await e.Author.SendMessageAsync("", false, new EmbedBuilder()
                    {
                        Title = "Invalid",
                        Color = Color.Blue,
                        Description = $"Your input was invalid. You inputted: {target}"
                    });
                }
            }
        }

        public override async Task<bool> powerResult(IGuildUser user, Player target)
        {
            try
            {
                if (target.Role.Title == "Godfather")
                    await user.SendMessageAsync("", false, new EmbedBuilder()
                    {
                        Title = "Godfather",
                        Color = Color.Blue,
                        Description = $"You checked {target.User.Username}, they are: {RoleUtil.Allignment.Town.ToString()}"
                    });
                else if (target.Role.Title == "Miller")
                    await user.SendMessageAsync("", false, new EmbedBuilder()
                    {
                        Title = "Miller",
                        Color = Color.Blue,
                        Description = $"You checked {target.User.Username}, they are: {RoleUtil.Allignment.Mafia.ToString()}"
                    });
                else
                    await user.SendMessageAsync("", false, new EmbedBuilder()
                    {
                        Title = "Cop Check Result",
                        Color = Color.Blue,
                        Description = $"You checked {target.User.Username}, they are:{target.Role.Allignment}"
                    });
            }
            catch (NullReferenceException)
            {
                await user.SendMessageAsync("", false, new EmbedBuilder()
                {
                    Title = "Did Not Target",
                    Color = Color.Blue,
                    Description = "You didn't target anyone last night."
                });
            }
            return await base.powerResult(user, target);
        }

    }
}
