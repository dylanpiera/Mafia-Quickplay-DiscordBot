using Discord;
using DiscordBot.Game;
using DiscordBot.Roles;
using DiscordBot.Roles.RoleUtil;

namespace DiscordBot.Core
{
    class Doctor : MafiaRole
    {
        public Doctor(string username) : base("Doctor", "Every night you can target someone, this person will be protected from any direct harm brought to the user this night.\nYou do this by saying `PROTECT: [playername]` at night in your PM.", Allignment.Town, Wincon.DefeatMafia)
        {
            this.rolePM = $"Dear **{username}**,\nYou are the **{Title}**.\n\n{description}\n\nYou win with the **Town** whose goal is to defeat all members of the Mafia.";
            power = "You can now select your doctor target. Please do so in the following format: `PROTECT: [playername]`";
            PowerRole = true;
        }

        protected override async void powerHandler(object s, MessageEventArgs e, GamePlayerList g)
        {
            if (e.Message.RawText.StartsWith("PROTECT: ") && e.Channel.Id == e.User.PrivateChannel.Id)
            {
                string target = e.Message.RawText.Replace("PROTECT: ", "");
                if (g.inGame(g.Find(target)))
                {
                    Target = g.Find(target);
                    if (Target.User.Nickname != null)
                        await e.User.SendMessage($"You will be protecting: {Target.User.Nickname} tonight. Use `PROTECT: [playername]` to change your target.");
                    else
                        await e.User.SendMessage($"You will be protecting: {Target.User.Name} tonight. Use `PROTECT: [playername]` to change your target.");
                }
                else
                {
                    await e.User.SendMessage($"Your input was invalid. You inputted: {target}");
                }
            }
        }

    }
}