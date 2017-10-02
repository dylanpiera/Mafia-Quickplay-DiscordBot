using Discord;
using Discord.WebSocket;
using DiscordBot.Game;
using DiscordBot.Roles;
using DiscordBot.Roles.RoleUtil;
using System.Threading.Tasks;

namespace DiscordBot.Roles
{
    class Doctor : MafiaRole, IMafiaRole
    {
        public override MafiaRole SetupRole(Player player)
        {
            base.SetupRole(player, "Doctor", "Every night you can target someone, this person will be protected from any direct harm brought to the user this night.\nYou do this by saying `PROTECT: [playername]` at night in your PM.", Allignment.Town, Wincon.DefeatMafia);
            
            this.rolePM = $"Dear **{Player.User.Nickname ?? Player.User.Username}**,\nYou are the **{Title}**.\n\n{description}\n\nYou win with the **Town** whose goal is to defeat all members of the Mafia.";
            power = "You can now select your doctor target. Please do so in the following format: `PROTECT: [playername]`";
            PowerRole = true;
            return this;
        }

        protected override async Task powerHandler(SocketMessage e, GamePlayerList g)
        {
            if (e.Content.StartsWith("PROTECT: ") && e.Channel.Id == (await this.Player.User.GetOrCreateDMChannelAsync() as IMessageChannel).Id)
            {
                string target = e.Content.Replace("PROTECT: ", "");
                if (g.InGame(g.Find(target)))
                {
                    Target = g.Find(target);
                    await e.Author.SendMessageAsync("", false, new EmbedBuilder()
                    {
                        Title = "Target Chosen",
                        Color = Color.Blue,
                        Description = $"You will be protecting: {Target.User.Nickname ?? Target.User.Username} tonight. Use `PROTECT: [playername]` to change your target."
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

    }
}