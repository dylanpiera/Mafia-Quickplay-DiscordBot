using Discord;
using DiscordBot.Game;
using DiscordBot.Roles.RoleUtil;

namespace DiscordBot.Roles
{
    class RestlessSpirit : MafiaRole, IMafiaRole
    {
        public override MafiaRole SetupRole(Player player)
        {
            base.SetupRole(player, "Restless Spirit", "After your demise; you will still be able to talk in-thread. You however, will not be able to vote.\nYou still count as dead in terms of win-condition.");

            this.rolePM = $"Dear **{Player.User.Nickname ?? Player.User.Username}**,\nYou are the **{Title}**.\n\n{description}\n\nYou win with the **Town** whose goal is to defeat all members of the Mafia.";

            return this;
        }
    }
}