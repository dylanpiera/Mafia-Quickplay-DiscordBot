using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Discord_Mafia_Bot.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord_Mafia_Bot.Commands
{
    
    public class Commands : ModuleBase
    {
        #region MiscCommands
        public class MiscCommands : ModuleBase
        {
            [Command("howtoplay"), Summary("Explains how to use the Mafia Bot and play a game!")]
            public async Task Ping()
            {
                await ReplyAsync("**How to play quickplay mafia games, with ME**\n\nYou can join a game by typing `!join` in the chat, once you're ready you'll have to type `!ready`.\nThe game won't start before __5__ people have joined!\nEveryone must be ready before the game starts.\n\nOnce the game starts you get your role PM. Any actions will be explained to you in your role PM.\nIn the game chat you can vote by typing `VOTE: @playername`\nFor a list of commands type `!help`.");
            }

            [Command("bug"), Summary("What to do when you find a bug.")]
            public async Task Bug()
            {
                await ReplyAsync("Found a bug? Or wanna recommend something be added/removed/changed?\nSend SoaringDylan a PM about it,\nPost it on our github https://github.com/dylanpiera/Mafia-Quickplay-DiscordBot \nor join our development server https://discord.gg/Tu82eWU");
            }

            [Command("cookie"), Summary("Gives you a cookie!")]
            public async Task Cookie()
            {
                await Context.Message.AddReactionAsync(new Emoji("🍪"));
            }

            [Command("inviteLink"), DiscordbotAdminPrecon(), Hidden()]
            public async Task inviteLink()
            {
                
                await ReplyAsync(Sneaky.botInvite);
            }
        }
        #endregion

        #region DebugCommands
        [Group("debug")]
        public class DebugCommands : ModuleBase
        {
            [Command("ping"), Summary("Returns with Pong!")]
            public async Task Ping()
            {
                await ReplyAsync("Pong!");
            }
        }
        #endregion

        #region JoinCommand
        public class JoinCommand : ModuleBase
        {
            [Command("join"), Summary("")]
            public async Task Join()
            {
                if (!Program.Servers[Context.Guild].gameRunning)
                {
                    if (!Program.Servers[Context.Guild].inGame(Context.User as IGuildUser))
                    {
                        Program.Servers[Context.Guild].Add(Context.User as IGuildUser);
                        await ReplyAsync("", false, new EmbedBuilder() { Title = "Player Joined!", Color = Color.Blue, Description = $"{Context.User.Mention} has joined the game! :white_check_mark:", Footer = new EmbedFooterBuilder() { Text = $"Current amount of players : {Program.Servers[Context.Guild].PlayerAmount}" } });

                        return;
                    }
                    else
                    {
                        await ReplyAsync("", false, new EmbedBuilder() { Title = "Already in Game!", Color = Color.DarkRed, Description = $"{Context.User.Mention} you're already in the game! :x:", Footer = new EmbedFooterBuilder() { Text = $"Current amount of players : {Program.Servers[Context.Guild].PlayerAmount}" } });
                    }
                }
                else
                {
                    await ReplyAsync("", false, new EmbedBuilder() { Title = "Ongoing Game!", Color = Color.DarkRed, Description = $"{Context.User.Mention} I'm sorry, but the game has already started. :no_entry_sign:", Footer = new EmbedFooterBuilder() { Text = $"Current amount of players alive: {Program.Servers[Context.Guild].TownAlive + Program.Servers[Context.Guild].MafiaAlive}/{Program.Servers[Context.Guild].PlayerAmount}" } });
                }
            }

            [Command("join"), Summary("(optional) argument: User | (make user) join the game."), RequireUserPermission(GuildPermission.Administrator)]
            public async Task Join(params IGuildUser[] user)
            {
                if (!Program.Servers[Context.Guild].gameRunning)
                {
                    if(Context.Message.MentionedUserIds.Count == 0) await ReplyAsync("", false, new EmbedBuilder() { Title = "Ongoing Game!", Color = Color.Orange, Description = $"{Context.User.Mention} You need to mention a user. :x:" });
                    foreach (ulong id in Context.Message.MentionedUserIds)
                    {
                        IGuildUser mentionedUser = await Context.Guild.GetUserAsync(id);
                        if (!mentionedUser.IsBot || Context.Message.Content.Contains("--force") && mentionedUser != Context.Client.CurrentUser)
                        {
                            if (!Program.Servers[Context.Guild].inGame(mentionedUser as IGuildUser))
                            {
                                Program.Servers[Context.Guild].Add(mentionedUser as IGuildUser);
                                await ReplyAsync("", false, new EmbedBuilder() { Title = "Player Added!", Color = Color.Blue, Description = $"{mentionedUser.Mention} was added to the game by {Context.User.Mention}! :white_check_mark:", Footer = new EmbedFooterBuilder() { Text = $"Current amount of players : {Program.Servers[Context.Guild].PlayerAmount}" } });
                            }
                            else
                            {
                                await ReplyAsync("", false, new EmbedBuilder() { Title = "Already in Game!", Color = Color.DarkRed, Description = $"{Context.User.Mention}, {mentionedUser.Mention} already is in the game! :x:", Footer = new EmbedFooterBuilder() { Text = $"Current amount of players : {Program.Servers[Context.Guild].PlayerAmount}" } });

                            }
                        }
                        else
                        {
                            await ReplyAsync("", false, new EmbedBuilder() { Title = "Bots can't join!", Color = Color.DarkRed, Description = $"{Context.User.Mention} Bots aren't allowed to play, in particular ME. :no_entry_sign:" });
                        }
                    }
                }
                else
                {
                    await ReplyAsync("", false, new EmbedBuilder() { Title = "Ongoing Game!", Color = Color.DarkRed, Description = $"{Context.User.Mention} I'm sorry, but the game has already started. :no_entry_sign:", Footer = new EmbedFooterBuilder() { Text = $"Current amount of players alive: {Program.Servers[Context.Guild].TownAlive + Program.Servers[Context.Guild].MafiaAlive}/{Program.Servers[Context.Guild].PlayerAmount}" } });
                }

            }
            [Command("join"), Hidden(), Priority(-1), RequireUserPermission(GuildPermission.Administrator)]
            public async Task Join(params string[] s)
            {
                await ReplyAsync("", false, new EmbedBuilder() { Title = "Ongoing Game!", Color = Color.Orange, Description = $"{Context.User.Mention} You need to mention a user. :x:"});
            }
        }
        #endregion

        #region LeaveCommand
        public class LeaveCommand : ModuleBase
        {
            [Command("leave"), Summary("Leave the current game signups.")]
            public async Task Leave()
            {
                
            }
        }

        #endregion
    }
}
