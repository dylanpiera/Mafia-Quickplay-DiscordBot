using Discord;
using Discord.Commands;
using Discord_Mafia_Bot.Core;
using Discord_Mafia_Bot.Util;
using DiscordBot.Game;
using DiscordBot.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Discord_Mafia_Bot.Commands
{
    public class Commands : ModuleBase
    {
        #region MiscCommands
        [Name("Misc. Commands")]
        public class MiscCommands : ModuleBase
        {
            //TODO: add version command

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

            [Command("inviteLink"), DiscordbotAdminPrecon(), Hidden()]
            public async Task InviteLink()
            {
                await ReplyAsync(Sneaky.botInvite);
            }
        }
        #endregion
		
        #region DebugCommands
        [Group("debug"), Name("Debug Commands"),Summary("Only for test builds.")]
        public class DebugCommands : ModuleBase
        {
            [Command("ping"), Summary("Returns with Pong!")]
            public async Task Ping()
            {
                await ReplyAsync("Pong!");
            }

            [Command("endgame"), Summary("(Bot Admin Only) ends the current game."), DiscordbotAdminPrecon(), Hidden()]
            public async Task EndGame()
            {
                if(Program.Servers[Context.Guild].GameRunning && Program.Servers[Context.Guild].Phase != Phases.EndPhase)
                {
                    await Context.Channel.SendMessageAsync("", false, new EmbedBuilder() { Title = "Game ended by Developer", Color = Color.DarkRed, Description = "The game has been ended by a developer." });

                    Program.Servers[Context.Guild].Reset();
                }
            }
            [Command("endphase"), Summary("(Bot Admin Only) ends the current phase."), DiscordbotAdminPrecon(), Hidden()]
            public async Task EndPhase()
            {
                if (Program.Servers[Context.Guild].GameRunning && Program.Servers[Context.Guild].Phase != Phases.EndPhase)
                {
                    await Context.Channel.SendMessageAsync("", false, new EmbedBuilder() { Title = "Phase skipped by Developer", Color = Color.DarkRed, Description = "This phase has been skipped by a developer." });

                    Program.Servers[Context.Guild].Token.Cancel();
                }
            }

            [Command("deleteChannel"), Hidden(), DiscordbotAdminPrecon()]
            public async Task DeleteChannel(ulong id)
            {
                await(await Context.Guild.GetChannelAsync(id)).DeleteAsync();
            }
        }
        #endregion

        #region Game Commands
        [Name("Game Commands"), Summary("")]
        public class Game_Commands : ModuleBase
        {
            #region JoinCommand

            [Command("join"), Summary("Join the current game signups.")]
            public async Task Join()
            {
                if (!Program.Servers[Context.Guild].GameRunning)
                {
                    if (!Program.Servers[Context.Guild].InGame(Context.User as IGuildUser))
                    {
                        Program.Servers[Context.Guild].Add(Context.User as IGuildUser);
                        await ReplyAsync("", false, new EmbedBuilder() { Title = "Player Joined!", Color = Color.Blue, Description = $"{Context.User.Mention} has joined the game! :white_check_mark:", Footer = new EmbedFooterBuilder() { Text = $"Current amount of players : {Program.Servers[Context.Guild].Objects.Count}" } });

                        return;
                    }
                    else
                    {
                        await ReplyAsync("", false, new EmbedBuilder() { Title = "Already in Game!", Color = Color.DarkRed, Description = $"{Context.User.Mention} you're already in the game! :x:", Footer = new EmbedFooterBuilder() { Text = $"Current amount of players : {Program.Servers[Context.Guild].Objects.Count}" } });
                    }
                }
                else
                {
                    await ReplyAsync("", false, new EmbedBuilder() { Title = "Ongoing Game!", Color = Color.DarkRed, Description = $"{Context.User.Mention} I'm sorry, but the game has already started. :no_entry_sign:", Footer = new EmbedFooterBuilder() { Text = $"Current amount of players alive: {Program.Servers[Context.Guild].TownAlive + Program.Servers[Context.Guild].MafiaAlive}/{Program.Servers[Context.Guild].Objects.Count}" } });
                }
            }

            [Command("join"), Summary("force [user] to join the current game signups."), RequireUserPermission(GuildPermission.ManageGuild), Name("!join [user]")]
            public async Task Join(params IGuildUser[] user)
            {
                if (!Program.Servers[Context.Guild].GameRunning)
                {
                    if (Context.Message.MentionedUserIds.Count == 0) await ReplyAsync("", false, new EmbedBuilder() { Title = "Ongoing Game!", Color = Color.Orange, Description = $"{Context.User.Mention} You need to mention a user. :x:" });
                    foreach (ulong id in Context.Message.MentionedUserIds)
                    {
                        IGuildUser mentionedUser = await Context.Guild.GetUserAsync(id);
                        if (!mentionedUser.IsBot || Context.Message.Content.Contains("--force") && mentionedUser != Context.Client.CurrentUser)
                        {
                            if (!Program.Servers[Context.Guild].InGame(mentionedUser as IGuildUser))
                            {
                                Program.Servers[Context.Guild].Add(mentionedUser as IGuildUser);
                                await ReplyAsync("", false, new EmbedBuilder() { Title = "Player Added!", Color = Color.Blue, Description = $"{mentionedUser.Mention} was added to the game by {Context.User.Mention}! :white_check_mark:", Footer = new EmbedFooterBuilder() { Text = $"Current amount of players : {Program.Servers[Context.Guild].Objects.Count}" } });
                            }
                            else
                            {
                                await ReplyAsync("", false, new EmbedBuilder() { Title = "Already in Game!", Color = Color.DarkRed, Description = $"{Context.User.Mention}, {mentionedUser.Mention} already is in the game! :x:", Footer = new EmbedFooterBuilder() { Text = $"Current amount of players : {Program.Servers[Context.Guild].Objects.Count}" } });
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
                    await ReplyAsync("", false, new EmbedBuilder() { Title = "Ongoing Game!", Color = Color.DarkRed, Description = $"{Context.User.Mention} I'm sorry, but the game has already started. :no_entry_sign:", Footer = new EmbedFooterBuilder() { Text = $"Current amount of players alive: {Program.Servers[Context.Guild].TownAlive + Program.Servers[Context.Guild].MafiaAlive}/{Program.Servers[Context.Guild].Objects.Count}" } });
                }

            }
            [Command("join"), Hidden(), Priority(-1), RequireUserPermission(GuildPermission.ManageGuild)]
            public async Task Join(params string[] s)
            {
                await ReplyAsync("", false, new EmbedBuilder() { Title = "Missing Mention!", Color = Color.Orange, Description = $"{Context.User.Mention} You need to mention a user. :x:" });
            }
            #endregion
            #region LeaveCommand
            [Command("leave"), Summary("Leave the current game signups.")]
            public async Task Leave()
            {
                if (!Program.Servers[Context.Guild].GameRunning)
                {
                    if (Program.Servers[Context.Guild].InGame(Context.User as IGuildUser))
                    {
                        Program.Servers[Context.Guild].Remove(Context.User as IGuildUser);
                        await ReplyAsync("", false, new EmbedBuilder() { Title = "Player Left!", Color = Color.DarkGrey, Description = $"{Context.User.Mention} has left the game! :heavy_check_mark:", Footer = new EmbedFooterBuilder() { Text = $"Current amount of players : {Program.Servers[Context.Guild].Objects.Count}" } });
                    }
                    else
                    {
                        await ReplyAsync("", false, new EmbedBuilder() { Title = "Not in Game!", Color = Color.DarkRed, Description = $"{Context.User.Mention} you ain't in the game! :x:" });
                    }
                }
                else
                {
                    await ReplyAsync("", false, new EmbedBuilder() { Title = "Not in Game!", Color = Color.DarkRed, Description = $"{Context.User.Mention} You can not leave a game in progress. :no_entry_sign:" });
                }
            }

            [Command("leave"), Summary("Force (mentioned) to leave the game."), RequireUserPermission(GuildPermission.ManageGuild), Alias("kick")]
            public async Task Leave(params IGuildUser[] users)
            {
                if (!Program.Servers[Context.Guild].GameRunning)
                {
                    if (Context.Message.MentionedUserIds.Count == 0) await ReplyAsync("", false, new EmbedBuilder() { Title = "Ongoing Game!", Color = Color.Orange, Description = $"{Context.User.Mention} You need to mention a user. :x:" });
                    foreach (ulong id in Context.Message.MentionedUserIds)
                    {
                        IGuildUser mentionedUser = await Context.Guild.GetUserAsync(id);
                        if (Program.Servers[Context.Guild].InGame(mentionedUser as IGuildUser))
                        {
                            Program.Servers[Context.Guild].Remove(mentionedUser as IGuildUser);
                            if (!Context.Message.Content.Contains("--silent"))
                                await ReplyAsync("", false, new EmbedBuilder() { Title = "Player Left!", Color = Color.DarkGrey, Description = $"{Context.User.Mention} removed {mentionedUser.Mention} from the game! :heavy_check_mark:", Footer = new EmbedFooterBuilder() { Text = $"Current amount of players : {Program.Servers[Context.Guild].Objects.Count}" } });
                        }
                        else
                        {
                            if (!Context.Message.Content.Contains("--silent"))
                                await ReplyAsync("", false, new EmbedBuilder() { Title = "Not in Game!", Color = Color.DarkRed, Description = $"{Context.User.Mention}, {mentionedUser.Mention} is not in the game! :x:" });
                        }
                    }
                    #region Possibility2
                    //
                    //foreach (IGuildUser mentionedUser in users)
                    //{
                    //    if (Program.Servers[Context.Guild].inGame(mentionedUser as IGuildUser))
                    //    {
                    //        Program.Servers[Context.Guild].Remove(mentionedUser as IGuildUser);
                    //        if (!Context.Message.Content.Contains("--silent"))
                    //            await ReplyAsync("", false, new EmbedBuilder() { Title = "Player Left!", Color = Color.DarkGrey, Description = $"{Context.User.Mention} removed {mentionedUser.Mention} from the game! :heavy_check_mark:", Footer = new EmbedFooterBuilder() { Text = $"Current amount of players : {Program.Servers[Context.Guild].PlayerAmount}" } });
                    //    }
                    //    else
                    //    {
                    //        if (!Context.Message.Content.Contains("--silent"))
                    //            await ReplyAsync("", false, new EmbedBuilder() { Title = "Not in Game!", Color = Color.DarkRed, Description = $"{Context.User.Mention}, {mentionedUser.Mention} is not in the game! :x:" });
                    //    }
                    //}
                    #endregion
                }
            }
            [Command("leave"), Hidden(), Priority(-1), RequireUserPermission(GuildPermission.ManageGuild), Alias("kick")]
            public async Task Leave(params string[] s)
            {
                await ReplyAsync("", false, new EmbedBuilder() { Title = "Missing Mention!", Color = Color.Orange, Description = $"{Context.User.Mention} You need to mention a user. :x:" });
            }
            #endregion
            #region ReadyCommand
            [Command("ready", RunMode = RunMode.Async), Summary("Ready up for the game to start!")]
            public async Task Ready()
            {
                GamePlayerList game = Program.Servers[Context.Guild];

                if (!game.GameRunning)
                {
                    if (game.InGame(Context.User as IGuildUser))
                    {
                        Player player = game.Find(Context.User as IGuildUser);
                        if (!player.Ready)
                        {
                            bool everyoneReady = player.readyUp(game);
                            await ReplyAsync("", false, new EmbedBuilder() { Title = "Player Ready!", Color = Color.Green, Description = $"{Context.User.Mention} is ready! :white_check_mark:", Footer = new EmbedFooterBuilder() { Text = $"Ready players: {game.Objects.Where(x => x.Ready).Count()}/{game.Objects.Count}" } });

                            if (everyoneReady && game.Objects.Count > 4)
                            {
                                await ReplyAsync("", false, new EmbedBuilder() { Title = "Game Start!", Color = Color.Green, Description = $"@everyone is ready! Starting up the game..." });
                                await Task.Delay(TimeConverter.SecToMS(2));
                                GameManager.StartGame(Context, game);
                            }
                            else if (everyoneReady)
                            {
                                await ReplyAsync("", false, new EmbedBuilder() { Title = "Too little players!", Color = Color.Orange, Description = $"@everyone is ready, but we don't have enough players. ", Footer = new EmbedFooterBuilder() { Text = $"[{game.Objects.Count}/5] required." } });
                            }
                        }
                        else
                        {
                            player.Ready = false;
                            await ReplyAsync("", false, new EmbedBuilder() { Title = "Player no longer ready!", Color = Color.Red, Description = $"{Context.User.Mention} is no longer ready! :x:", Footer = new EmbedFooterBuilder() { Text = $"Ready players: {game.Objects.Where(x => x.Ready).Count()}/{game.Objects.Count}" } });
                        }
                    }
                    else
                    {
                        await ReplyAsync("", false, new EmbedBuilder() { Title = "Not in game!", Color = Color.Red, Description = $"{Context.User.Mention} you're not in the game! Please join first by typing !join :no_entry_sign:" });
                    }
                }
            }

            [Command("startgame"), Summary("Admin only: Force game to start"), RequireUserPermission(GuildPermission.Administrator | GuildPermission.BanMembers)]
            public async Task StartGame()
            {
                GamePlayerList game = Program.Servers[Context.Guild];

                if (!game.GameRunning && game.Objects.Count > 4)
                {
                    await ReplyAsync("", false, new EmbedBuilder() { Title = "Game Forced Start!", Color = Color.DarkGreen, Description = $"The game has been started by a Moderator @everyone, Starting up the game..." });
                    //game.gameRunning = true; //Should be moved to startGame()
                    await Task.Delay(TimeConverter.SecToMS(2));
                    GameManager.StartGame(Context, game);
                }
                else if (!game.GameRunning && game.Objects.Count <= 4)
                {
                    await ReplyAsync("", false, new EmbedBuilder() { Title = "Failed to start!", Color = Color.DarkOrange, Description = $"{Context.User.Mention} you can not force launch the game, it has less than 5 players. :no_entry_sign:", Footer = new EmbedFooterBuilder() { Text = $"[{game.Objects.Count}/5] required." } });
                }
            }
            #endregion
            #region ListCommand
            [Command("list"), Summary("Get a list of people currently in the mafia game on the current server."), Alias("players"), Ratelimit(1,0.1, Measure.Minutes)]
            public async Task List()
            {
                if (!Program.Servers[Context.Guild].GameRunning)
                {
                    if (Program.Servers[Context.Guild].Objects.Count > 0)
                    {
                        EmbedBuilder builder = new EmbedBuilder()
                        {
                            Color = Color.LighterGrey,
                            Title = "Player List:"
                        };
                        int i = 0;
                        foreach (Player player in Program.Servers[Context.Guild].Objects)
                        {
                            i++;

                            string ready = player.Ready ? ":thumbsup::skin-tone-1:" : ":thumbsdown::skin-tone-1:";

                            builder.Description += $"{i}. {player.User.Nickname ?? player.User.Username}\t-\tReady: {ready}\n";
                        }
                        await ReplyAsync("", false, builder.Build());
                        return;
                    }
                    else
                    {
                        await ReplyAsync("", false, new EmbedBuilder() { Title = "Empty Game!", Color = Color.DarkRed, Description = $"{Context.User.Username} the game is empty! :x:" });
                    }
                }
                else
                {
                    EmbedBuilder builder = new EmbedBuilder()
                    {
                        Color = Color.LighterGrey,
                        Title = "Player list with votes:"
                    };

                    EmbedFieldBuilder alive = new EmbedFieldBuilder() { Name = "Alive", IsInline = false }, dead = new EmbedFieldBuilder() { Name = "The Dead", IsInline = false};

                    foreach (Player player in Program.Servers[Context.Guild].Objects)
                    {
                        if (player.Alive)
                        {
                            Console.WriteLine("Alive.");
                            try
                            {
                                alive.Value += $"{player.User.Nickname ?? player.User.Username}\tvotes: **{player.LynchTarget.User.Nickname ?? player.LynchTarget.User.Username}**\n";
                            }
                            catch (Exception)
                            {
                                alive.Value += $"{player.User.Nickname ?? player.User.Username}\tvotes: -\n";
                            }
                        }
                        else
                        {
                            try
                            {
                                dead.Value += $"{player.User.Nickname ?? player.User.Username}\twas: {player.Role.Allignment.ToString()} {player.Role.Title}\n";
                            }
                            catch (Exception) { }
                        }
                    }

                    builder.AddField(alive);
                    builder.AddField(x =>
                    {
                        x.Name = dead.Name;
                        x.IsInline = dead.IsInline;
                        try
                        {
                            x.Value = dead.Value;
                        }
                        catch (Exception)
                        {
                            x.Value = "No dead players.";
                        }
                    });

                    await ReplyAsync("", false, builder.Build());
                    return;
                }
            }

            [Command("votecount"), Alias("tally"), Summary("Shows the total votes on all players.")]
            public async Task VoteCount()
            {
                GamePlayerList game = Program.Servers[Context.Guild];
                if (game.Phase == Phases.Day)
                {
                    CountVotes(game);
                    int i = 0;
                    EmbedBuilder builder = new EmbedBuilder() { Color = Color.LightGrey, Title = $"{game.Phase.ToString()} {game.PhaseCounter} vote tally:" };

                    List<Player> SortedList = game.Objects.Where(x => x.Alive && x.VotesOn > 0).OrderByDescending(o => o.VotesOn).ToList();
                    if (SortedList.Count != 0)
                    {
                        foreach (Player player in SortedList)
                        {
                            i++;
                            try
                            {
                                builder.Description += $"{player.User.Mention} \t{player.VotesOn}: {VotedFor(game.Objects.Where(x => x.Alive).ToList(), player)}\n";
                            }
                            catch (Exception) { }
                        }
                    }
                    else
                    {
                        builder.Description += "There are no votes.";
                    }

                    await ReplyAsync("", false, builder.Build());
                }
            }

            public static string VotedFor(List<Player> sortedList, Player lynchee)
            {
                string s = "";

                foreach (Player player in sortedList)
                {
                    if (player.LynchTarget == lynchee)
                        s += player.User.Mention + ", ";
                }
                s.Remove(s.Length - 2);
                return s;
            }

            public static void CountVotes(GamePlayerList game)
            {
                foreach (Player player in game.Objects)
                {
                    player.VotesOn = 0;
                }
                foreach (Player player in game.Objects)
                {
                    try
                    {
                        player.LynchTarget.VotesOn++;
                    }
                    catch (Exception) { }
                }
            }

            #endregion
        }
        #endregion

	    #region FunCommands
	[Group("fun"), Name("Fun Commands"), Summary("For fun!")]
	public class FunCommands : ModuleBase
		{
		[Command("cookie"), Summary("Gives you a cookie!")]
        	public async Task Cookie()
            	{
                	await Context.Message.AddReactionAsync(new Emoji("🍪"));
            	}
		
		[Command("moo"), Summary("Moooooo!")]
            	public async Task Moo()
            	{
			await Context.Message.AddReactionAsync(new Emoji("🐮"));
            	}
	}
	#endregion
    }
}
