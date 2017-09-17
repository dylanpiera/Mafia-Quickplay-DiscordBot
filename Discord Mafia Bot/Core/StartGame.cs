using Discord;
using Discord.Commands;
using Discord.WebSocket;
using DiscordBot.Game;
using DiscordBot.Resources;
using DiscordBot.Roles;
using DiscordBot.Roles.RoleUtil;
using DiscordBot.Util;
using Discord_Mafia_Bot.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Discord_Mafia_Bot.Core
{
    public static class GameManager
    {
        #region startGame 
        /// <summary>
        /// Start the process of starting a game
        /// </summary>
        /// <param name="Context">Command Context</param>
        /// <param name="game">The game</param>
        public static async void startGame(ICommandContext Context, GamePlayerList game)
        {
            IGuildChannel gameChannel = await Context.Guild.CreateTextChannelAsync("Mafia-Game-Room", new RequestOptions() { AuditLogReason = "Start of Mafia Game" });
            await gameChannel.AddPermissionOverwriteAsync(Context.Client.CurrentUser, new OverwritePermissions(manageChannel: PermValue.Allow, addReactions: PermValue.Allow, readMessages: PermValue.Allow, sendMessages: PermValue.Allow, mentionEveryone: PermValue.Allow, managePermissions: PermValue.Allow), new RequestOptions { AuditLogReason = "Start of Mafia Game" });
            await gameChannel.AddPermissionOverwriteAsync(Context.Guild.EveryoneRole, new OverwritePermissions(readMessages: PermValue.Deny, sendMessages: PermValue.Deny, addReactions: PermValue.Deny), new RequestOptions { AuditLogReason = "Start of Mafia Game" });

            foreach (Player players in game.Objects)
            {
                await gameChannel.AddPermissionOverwriteAsync(players.User, new OverwritePermissions(readMessages: PermValue.Allow, sendMessages: PermValue.Deny));
            }

            EmbedBuilder builder = new EmbedBuilder() { Color = Color.LightGrey, Title = "Starting game..." };
            builder.Description += "@everyone the game is starting up...\n\n";
            IUserMessage msg = await (gameChannel as IMessageChannel).SendMessageAsync("", false, builder.Build());
            await Task.Delay(500);

            //Distribute the roles
            builder.Description += "Distributing roles...\n\n";
            await msg.ModifyAsync(x => x.Embed = builder.Build());

            try
            {
                List<MafiaRole> setup = HandleSetup(game);
                distributeRoles(game, setup);
            }
            catch
            {
                await Context.Channel.SendMessageAsync(":no_entry_sign: An error occured. Game setup has stalled. Please contact a moderator. :no_entry_sign:");
            }

            await Task.Delay(500);

            builder.Description += "Sending Role PMs...\n\n";
            await msg.ModifyAsync(x => x.Embed = builder.Build());

            foreach (var player in game.Objects)
            {
                player.Role.sendRolePM(player.User);
            }
            await Task.Delay(500);

            builder.Description += "Creating Mafia Chat...\n\n";
            await msg.ModifyAsync(x => x.Embed = builder.Build());

            IGuildChannel mafiaChannel = await Context.Guild.CreateTextChannelAsync("Mafia-Night-Chat", new RequestOptions() { AuditLogReason = "Start of Mafia Game" });
            await mafiaChannel.AddPermissionOverwriteAsync(Context.Client.CurrentUser, new OverwritePermissions(manageChannel: PermValue.Allow, addReactions: PermValue.Allow, readMessages: PermValue.Allow, sendMessages: PermValue.Allow, mentionEveryone: PermValue.Allow, managePermissions: PermValue.Allow), new RequestOptions { AuditLogReason = "Start of Mafia Game" });
            await mafiaChannel.AddPermissionOverwriteAsync(Context.Guild.EveryoneRole, new OverwritePermissions(readMessages: PermValue.Deny, sendMessages: PermValue.Deny, addReactions: PermValue.Deny), new RequestOptions { AuditLogReason = "Start of Mafia Game" });
            game.setChats(gameChannel, mafiaChannel);

            foreach (Player player in game.Objects)
            {
                if (player.Role.Allignment == Allignment.Mafia)
                    await game.MafiaChat.AddPermissionOverwriteAsync(player.User, new OverwritePermissions(readMessages: PermValue.Allow, sendMessages: PermValue.Allow));
            }

            await (game.MafiaChat as IMessageChannel).SendMessageAsync("", false, new EmbedBuilder() { Title = "Welcome Scummy friends :smiling_imp:", Description = "You can freely discuss in this chat during both day & night phases.\nOnce night hits I will make an announcement for you to post your Night Kill Target." });
            await Task.Delay(500);

            builder.Description += "Game preperation, completed. :white_check_mark:";
            await msg.ModifyAsync(x => x.Embed = builder.Build());

            game.Phase = Phases.Day;
            game.PhaseCounter = 1;

            EmbedBuilder b = new EmbedBuilder() { Color = Color.LightOrange, Title = "Start of D1", Description = $":sunny: @everyone **It is now Day 1.** Day 1 will end in {game.PhaseLengthInMin} minutes. :sunny:" };
            b.AddField(x =>
            {
                x.Name = "More Info";
                x.Value = "To vote use: `VOTE: (mention)` or `VOTE: (username)`. For more info, use !howtoplay";
                x.IsInline = false;
            });
            await (game.GameChat as IMessageChannel).SendMessageAsync("", false, b.Build());
            await Task.Delay(100);
            foreach (Player player in game.Objects)
            {
                await game.GameChat.AddPermissionOverwriteAsync(player.User, new OverwritePermissions(sendMessages: PermValue.Allow, readMessages: PermValue.Allow));
            }

            //run the game manager
            runGame(game, Context);

        }

        /// <summary>
        /// Randomly distribute roles among players.
        /// </summary>
        /// <param name="game">The game where to pull the players from.</param>
        /// <param name="setup">The setup where to pull the roles from.</param>
        private static void distributeRoles(GamePlayerList game, List<MafiaRole> setup)
        {
            List<Player> players = ListHelper.ShuffleList<Player>(game.Objects);
            List<MafiaRole> roles = ListHelper.ShuffleList<MafiaRole>(setup);
            MafiaRole role;

            if (players.Count == roles.Count)
            {
                foreach (Player player in players)
                {
                    player.AssignRole(role = roles.FirstOrDefault());
                    roles.Remove(role);
                }
            }
            else
            {
                throw new InvalidOperationException();
            }
            game.Objects = players;
        }

        /// <summary>
        /// Handles the creation of setup. Based on what setup is selected
        /// <!-- TODO: allow multiple setups -->
        /// </summary>
        /// <param name="g">The game to pull player amount information from.</param>
        /// <returns>
        /// A list of roles (setup) to use for the game.
        /// <!-- TODO: use the Setup class instead of a list. -->
        /// </returns>
        private static List<MafiaRole> HandleSetup(GamePlayerList g)
        {
            List<MafiaRole> setup = new List<MafiaRole>();

            if (true) //change this later for default setup
            {
                int playerCount = g.Objects.Count;

                //int funFactor = ListHelper.r.Next(100);

                g.TownPlayers = ((int)Math.Floor(playerCount * 0.75));
                g.MafiaPlayers = ((int)Math.Ceiling(playerCount * 0.25));

                if (playerCount == 5 && g.MafiaPlayers == 2)
                {
                    g.TownPlayers++;
                    g.MafiaPlayers--;
                }
                int i = g.TownPlayers;
                while (i >= 3)
                {
                    if (i >= 4 && g.Doctors == 0)
                    {
                        g.TownPlayers--;
                        g.Doctors++;
                        i--;
                    }
                    g.TownPlayers--;
                    g.Cops++;
                    i -= 3;
                }
            }
            for (int i = 0; i < g.Doctors; i++)
            {
                setup.Add(new Doctor("Player"));
                g.TownAlive++;
            }
            for (int i = 0; i < g.Cops; i++)
            {
                setup.Add(new Cop("Player"));
                g.TownAlive++;
            }
            for (int i = 0; i < g.TownPlayers; i++)
            {
                setup.Add(new Vanilla(Allignment.Town, "Player"));
                g.TownAlive++;
            }
            for (int i = 0; i < g.Godfathers; i++)
            {
                setup.Add(new Vanilla("Godfather", "You are the leader of the mafia, and will show up as town when scanned by an investigative role like Cop.\nYou also have the power to vote in the Mafia Chat every night on whom to kill.", $"Dear Player,\n\nYou are the most basic of roles in existence,\nYou are the **Godfather**.\n\nYou are the leader of the mafia, and will show up as town when scanned by an investigative role like Cop.\nYou also have the power to vote in the Mafia Chat every night on whom to kill.\n\nYou win with the **Mafia** whose goal is to outnumber all members of the Town"));
            }
            for (int i = 0; i < g.MafiaPlayers; i++)
            {
                setup.Add(new Vanilla(Allignment.Mafia, "Player"));
                g.MafiaAlive++;
            }

            return setup;
        }
        #endregion

        #region RunGame
        public async static void runGame(GamePlayerList game, ICommandContext context)
        {
            do
            {
                if (game.Phase == Phases.Day)
                {
                    game.Token = new CancellationTokenSource();

                    try
                    {
                        await runDayPhase(game, context);
                    }
                    catch (Exception) { }
                    finally
                    {
                        await runDayRecap(game);
                    }
                }
            } while (false);
        }

        #region DayPhase
        /// <summary>
        /// Runs the day phase.
        /// </summary>
        /// <param name="game">The game to run.</param>
        /// <param name="context">The client context.</param>
        private static async Task runDayPhase(GamePlayerList game, ICommandContext context)
        {

            Func<SocketMessage, Task> voteHandler = (e) => VoteHandler(e, game, context);
            (context.Client as DiscordSocketClient).MessageReceived += voteHandler;

            await Task.Delay(TimeConverter.MinToMS(game.PhaseLengthInMin/2),game.Token.Token);

            Commands.Commands.ListCommand.countVotes(game);
            int i = 0;
            EmbedBuilder builder = new EmbedBuilder() { Color = Color.LightGrey, Title = $"{game.Phase.ToString()} {game.PhaseCounter} midday vote tally:" };

            List<Player> SortedList = game.Objects.Where(x => x.Alive && x.VotesOn > 0).OrderByDescending(o => o.VotesOn).ToList();
            if (SortedList.Count != 0)
            {
                foreach (Player player in SortedList)
                {
                    i++;
                    try
                    {
                        builder.Description += $"{i}. {player.User.Mention} {player.VotesOn}: {Commands.Commands.ListCommand.votedFor(game.Objects.Where(x => x.Alive).ToList(), player)}";
                    }
                    catch (Exception) { }
                }
            }
            else
            {
                builder.Description += "There are no votes.";
            }
            builder.Description += "\n";
            builder.AddField(x => { x.Name = $"Time Left"; x.Value = $":warning: There are only {game.PhaseLengthInMin/2} minutes left in the day phase. :warning:"; });

            await (game.GameChat as IMessageChannel).SendMessageAsync("", false, builder.Build());

            await Task.Delay(TimeConverter.MinToMS(game.PhaseLengthInMin / 2), game.Token.Token);
            (context.Client as DiscordSocketClient).MessageReceived -= voteHandler;
        }

        private static async Task runDayRecap(GamePlayerList game)
        {
            throw new NotImplementedException();
        }
        #endregion

        private static async Task VoteHandler(SocketMessage e, GamePlayerList game, ICommandContext context)
        {
            if(e.Channel.Id == game.GameChat.Id)
            {
                if(e.Content.StartsWith("VOTE: ") && e.MentionedUsers.Count == 1)
                {
                    IGuildUser target;
                    if(game.inGame(target = (e.MentionedUsers.FirstOrDefault() as IGuildUser)))
                    {
                        game.Find(e.Author as IGuildUser).LynchTarget = game.Find(target);
                        await (e as IUserMessage).AddReactionAsync(new Emoji("✅"));
                    }
                }
                else if (e.Content.StartsWith("UNVOTE"))
                {
                    try
                    {
                        Player user;
                        if (!((user = game.Find(e.Author as IGuildUser)).LynchTarget == null))
                        {
                            user.LynchTarget = null;
                            await (e as IUserMessage).AddReactionAsync(new Emoji("✅"));
                        }
                        else
                        {
                            await (e as IUserMessage).AddReactionAsync(new Emoji("❌"));
                        }
                    }
                    catch (Exception) { }
                }
                else if(e.Content.StartsWith("VOTE: "))
                {
                    await e.Author.SendMessageAsync("Please mention only 1 user who you're voting for.");
                    await (e as IUserMessage).AddReactionAsync(new Emoji("❌"));
                }
            }
        }

        #endregion
    }
}

