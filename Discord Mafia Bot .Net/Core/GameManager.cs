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
        public static async void StartGame(ICommandContext Context, GamePlayerList game)
        {
            game.GameRunning = true;

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
                DistributeRoles(game, setup);
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
                await Task.Delay(500);
            }
            await Task.Delay(500);

            builder.Description += "Creating Mafia Chat...\n\n";
            await msg.ModifyAsync(x => x.Embed = builder.Build());

            IGuildChannel mafiaChannel = await Context.Guild.CreateTextChannelAsync("Mafia-Night-Chat", new RequestOptions() { AuditLogReason = "Start of Mafia Game" });
            await mafiaChannel.AddPermissionOverwriteAsync(Context.Client.CurrentUser, new OverwritePermissions(manageChannel: PermValue.Allow, addReactions: PermValue.Allow, readMessages: PermValue.Allow, sendMessages: PermValue.Allow, mentionEveryone: PermValue.Allow, managePermissions: PermValue.Allow), new RequestOptions { AuditLogReason = "Start of Mafia Game" });
            await mafiaChannel.AddPermissionOverwriteAsync(Context.Guild.EveryoneRole, new OverwritePermissions(readMessages: PermValue.Deny, sendMessages: PermValue.Deny, addReactions: PermValue.Deny), new RequestOptions { AuditLogReason = "Start of Mafia Game" });
            
            

            EmbedBuilder mafiaBuilder = new EmbedBuilder() { Title = "Welcome Scummy friends :smiling_imp:", Description = "You can freely discuss in this chat during both day & night phases.\nOnce night hits I will make an announcement for you to post your Night Kill Target.\n" };
            EmbedFieldBuilder mafiaField = new EmbedFieldBuilder {Name = "The Team", IsInline = true };

            foreach (Player player in game.Objects.Where(x => x.Role.Allignment == Allignment.Mafia))
            {
                mafiaField.Value += $"{player.User.Nickname ?? player.User.Username} as: {player.Role.Title}\n";
                await mafiaChannel.AddPermissionOverwriteAsync(player.User, new OverwritePermissions(readMessages: PermValue.Allow, sendMessages: PermValue.Allow));
                await Task.Delay(500);
            }

            mafiaBuilder.AddField(mafiaField);
            await (mafiaChannel as IMessageChannel).SendMessageAsync("", false, mafiaBuilder.Build());
            await Task.Delay(500);

            builder.Description += "Running Finishing Touches...\n\n";
            await msg.ModifyAsync(x => x.Embed = builder.Build());

            IGuildChannel graveyardChannel = await Context.Guild.CreateTextChannelAsync("Graveyard-Chat", new RequestOptions() { AuditLogReason = "Start of Mafia Game"});
            await graveyardChannel.AddPermissionOverwriteAsync(Context.Client.CurrentUser, new OverwritePermissions(manageChannel: PermValue.Allow, addReactions: PermValue.Allow, readMessages: PermValue.Allow, sendMessages: PermValue.Allow, mentionEveryone: PermValue.Allow, managePermissions: PermValue.Allow), new RequestOptions { AuditLogReason = "Start of Mafia Game" });
            await graveyardChannel.AddPermissionOverwriteAsync(Context.Guild.EveryoneRole, new OverwritePermissions(readMessages: PermValue.Deny, sendMessages: PermValue.Deny, addReactions: PermValue.Deny), new RequestOptions { AuditLogReason = "Start of Mafia Game" });
            game.SetChats(gameChannel, mafiaChannel, graveyardChannel, Context.Channel as IGuildChannel);

            builder.Description += "Game preparation, completed. :white_check_mark:";
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
            RunGame(game, Context);

        }

        /// <summary>
        /// Randomly distribute roles among players.
        /// </summary>
        /// <param name="game">The game where to pull the players from.</param>
        /// <param name="setup">The setup where to pull the roles from.</param>
        private static void DistributeRoles(GamePlayerList game, List<MafiaRole> setup)
        {
            List<Player> players = game.Objects;
            List<MafiaRole> roles = ListHelper.ShuffleList<MafiaRole>(setup);
            MafiaRole role;

            if (players.Count == roles.Count)
            {
                foreach (Player player in players)
                {
                    player.AssignRole(role = roles.FirstOrDefault().SetupRole(player));
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
                if (g.MafiaPlayers > 1)
                {
                    g.MafiaPlayers--;
                    g.Godfathers++;
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
                setup.Add(new Doctor());
                g.TownAlive++;
            }
            for (int i = 0; i < g.Cops; i++)
            {
                setup.Add(new Cop());
                g.TownAlive++;
            }
            for (int i = 0; i < g.TownPlayers; i++)
            {
                setup.Add(new Vanilla(Allignment.Town));
                g.TownAlive++;
            }
            for (int i = 0; i < g.Godfathers; i++)
            {
                setup.Add(new Vanilla(VanillaTypes.Godfather));
                g.MafiaAlive++;
            }
            for (int i = 0; i < g.MafiaPlayers; i++)
            {
                setup.Add(new Vanilla(Allignment.Mafia));
                g.MafiaAlive++;
            }

            return setup;
        }
        #endregion

        #region RunGame
        public async static void RunGame(GamePlayerList game, ICommandContext context)
        {
            do
            {
                if (game.Phase == Phases.Day)
                {
                    game.Token = new CancellationTokenSource();

                    try
                    {
                        await RunDayPhase(game, context);
                    }
                    catch (Exception) { }
                    finally
                    {
                        await RunDayRecap(game);
                    }
                }
                else if (game.Phase == Phases.Night)
                {
                    game.Token = new CancellationTokenSource();

                    try
                    {
                        await RunNightPhase(game, context);
                    }
                    catch (Exception) { }
                    finally
                    {
                        await RunNightRecap(game, context);
                    }
                }
                else
                {
                    await (game.GameChat as IMessageChannel).SendMessageAsync("", false, new EmbedBuilder() { Title = "End of Game", Description = "Stopping game in 1 minute...\nFeel free to discuss the game in the signup channel :slight_smile:", Color = Color.LightGrey });
                    await Task.Delay(TimeConverter.MinToMS(1));
                    game.Reset();
                }
            } while (game.GameRunning);
        }

        #region NightPhase
        private static async Task RunNightPhase(GamePlayerList game, ICommandContext context)
        {
            HandlePowerRoles(game, context);

            game.NightkillHandler = new Func<SocketMessage, Task>((e) => NightKillHandler(e, game, context));
            (context.Client as DiscordSocketClient).MessageReceived += game.NightkillHandler;

            await (game.MafiaChat as IMessageChannel).SendMessageAsync("", false, new EmbedBuilder() { Title = "Night actions available.", Color = Color.DarkerGrey, Description = $"Dear Scum, It is now Night {game.PhaseCounter}. Please select your Night Kill target by typing `KILL: playername`. " });

            await Task.Delay(TimeConverter.MinToMS((game.PhaseLengthInMin / 4)), game.Token.Token);
            await (game.GameChat as IMessageChannel).SendMessageAsync("", false, new EmbedBuilder() { Title = $"Night {game.PhaseCounter} midway point", Color = Color.DarkBlue, Description = $":warning: There are only {game.PhaseLengthInMin / 4} minutes left in the night phase. :warning:" });
            await Task.Delay(TimeConverter.MinToMS((game.PhaseLengthInMin / 4)), game.Token.Token);
        }

        private static async Task NightKillHandler(SocketMessage e, GamePlayerList game, ICommandContext context)
        {
            if (e.Channel.Id == game.MafiaChat.Id && e.Content.StartsWith("KILL: "))
            {
                string target = e.Content.Replace("KILL: ", "");
                Console.WriteLine("Night kill target: " + target);
                if (game.InGame(game.Find(target)))
                {
                    game.MafiaKillTarget = game.Find(target);
                    Console.WriteLine("Target set.");
                    await (game.MafiaChat as IMessageChannel).SendMessageAsync("", false, new EmbedBuilder() { Color = Color.DarkerGrey, Title = "Nightkill Target", Description = $"The current kill target is: {game.MafiaKillTarget.User.Mention}. Use `KILL: [playername]` to change your target." });
                }
                else
                {
                    Console.WriteLine("Couldn't find target.");
                    await (game.MafiaChat as IMessageChannel).SendMessageAsync("", false, new EmbedBuilder() { Color = Color.DarkRed, Title = "Invalid Input", Description = $"Your input was invalid. You inputted: {target}" });
                }
                Console.WriteLine("End of nightkill handler.");
            }

        }

        private static async void HandlePowerRoles(GamePlayerList game, ICommandContext context)
        {
            foreach (Player player in game.Objects.Where(x => x.Alive && x.Role.PowerRole))
            {
                await player.User.SendMessageAsync(player.Role.Power);
                (context.Client as DiscordSocketClient).MessageReceived += player.Role.PowerHandler(game);
            }
        }

        private static async Task RunNightRecap(GamePlayerList game, ICommandContext context)
        {
            game.Phase = Phases.Day;

            EmbedBuilder builder = new EmbedBuilder() { Title = $"Night {game.PhaseCounter} Recap", Color = Color.DarkTeal, Description = $":night_with_stars: @everyone the Night phase has ended! Recapping now... :night_with_stars:\n\n" };
            IUserMessage msg = await (game.GameChat as IMessageChannel).SendMessageAsync("", false, builder.Build());

            foreach (Player player in game.Objects.Where(x => x.Alive))
            {
                await game.GameChat.AddPermissionOverwriteAsync(player.User, new OverwritePermissions(readMessages: PermValue.Allow, sendMessages: PermValue.Deny));
                if (player.Role.PowerRole)
                {
                    (context.Client as DiscordSocketClient).MessageReceived -= player.Role.PowerHandler(game);
                    await player.Role.powerResult(player.User, player.Role.Target);
                }
            }

            (context.Client as DiscordSocketClient).MessageReceived -= game.NightkillHandler;

            try //inpractical fix. But fix none-the-less
            {
                if (game.MafiaKillTarget != null && game.MafiaKillTarget != game.Objects.Where(x => x.Role.Title == "Doctor" && x.Alive).FirstOrDefault().Role.Target)
                {
                    builder.Description += $"When everyone woke up in the morning, they found out someone was missing: {game.MafiaKillTarget.User.Mention}\nOnce they arived at their home, they were found death on the ground.\n\n**{game.MafiaKillTarget.User.Mention} was killed by the Mafia. They were:**\n**Role PM:**\n```{game.MafiaKillTarget.Role.RolePM}```\n";
                    Player target;
                    (target = game.MafiaKillTarget).Alive = false;
                    await game.GraveyardChat.AddPermissionOverwriteAsync(target.User, new OverwritePermissions(readMessages: PermValue.Allow, sendMessages: PermValue.Allow));
                    if (target.Role.Allignment == Allignment.Mafia) { game.MafiaAlive--; await game.MafiaChat.AddPermissionOverwriteAsync(target.User, new OverwritePermissions(readMessages: PermValue.Allow, sendMessages: PermValue.Deny)); }
                    else if (target.Role.Allignment == Allignment.Town) game.TownAlive--;
                    game.MafiaKillTarget = null;
                }

                else
                {
                    if (game.MafiaKillTarget != null)
                        await (game.MafiaChat as IMessageChannel).SendMessageAsync("", false, new EmbedBuilder() { Color = Color.DarkRed, Title = "Kill Failed!", Description = $"__Your kill on {game.MafiaKillTarget.User.Mention} was unsuccesful.__" });
                    builder.Description += "Tonight has been a quiet night... Nothing happened...";
                }
                await msg.ModifyAsync(x => x.Embed = builder.Build());
            }
            catch (Exception)
            {
                if (game.MafiaKillTarget != null)
                {
                    builder.Description += $"When everyone woke up in the morning, they found out someone was missing: {game.MafiaKillTarget.User.Mention}\nOnce they arived at their home, they were found death on the ground.\n\n**{game.MafiaKillTarget.User.Mention} was killed by the Mafia. They were:**\n**Role PM:**\n```{game.MafiaKillTarget.Role.RolePM}```\n";
                    Player target;
                    (target = game.MafiaKillTarget).Alive = false;
                    await game.GraveyardChat.AddPermissionOverwriteAsync(target.User, new OverwritePermissions(readMessages: PermValue.Allow, sendMessages: PermValue.Allow));
                    if (target.Role.Allignment == Allignment.Mafia) { game.MafiaAlive--; await game.MafiaChat.AddPermissionOverwriteAsync(target.User, new OverwritePermissions(readMessages: PermValue.Allow, sendMessages: PermValue.Deny)); }
                    else if (target.Role.Allignment == Allignment.Town) game.TownAlive--;
                    game.MafiaKillTarget = null;
                }

                else
                {
                    if (game.MafiaKillTarget != null)
                        await (game.MafiaChat as IMessageChannel).SendMessageAsync("", false, new EmbedBuilder() { Color = Color.DarkRed, Title = "Kill Failed!", Description = $"__Your kill on {game.MafiaKillTarget.User.Mention} was unsuccesful.__" });
                    builder.Description += "Tonight has been a quiet night... Nothing happened...";
                }
            }
            builder.Description += $"";
            await msg.ModifyAsync(x => x.Embed = builder.Build());

            if (await CheckWinConditions(game)) return;
            foreach (Player player in game.Objects.Where(x => x.Alive))
            {
                await game.GameChat.AddPermissionOverwriteAsync(player.User, new OverwritePermissions(sendMessages: PermValue.Allow, readMessages: PermValue.Allow));
            }
            game.PhaseCounter++;
        }
        #endregion

        #region DayPhase
        /// <summary>
        /// Runs the day phase.
        /// </summary>
        /// <param name="game">The game to run.</param>
        /// <param name="context">The client context.</param>
        private static async Task RunDayPhase(GamePlayerList game, ICommandContext context)
        {

            Func<SocketMessage, Task> voteHandler = (e) => VoteHandler(e, game, context);
            (context.Client as DiscordSocketClient).MessageReceived += voteHandler;

            await Task.Delay(TimeConverter.MinToMS(game.PhaseLengthInMin / 2), game.Token.Token);

            Commands.Commands.Game_Commands.CountVotes(game);
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
                        builder.Description += $"{i}. {player.User.Mention} {player.VotesOn}: {Commands.Commands.Game_Commands.VotedFor(game.Objects.Where(x => x.Alive).ToList(), player)}\n";
                    }
                    catch (Exception) { }
                }
            }
            else
            {
                builder.Description += "There are no votes.";
            }
            builder.Description += "\n";
            builder.AddField(x => { x.Name = $"Time Left"; x.Value = $":warning: There are only {game.PhaseLengthInMin / 2} minutes left in the day phase. :warning:"; });

            await (game.GameChat as IMessageChannel).SendMessageAsync("", false, builder.Build());

            await Task.Delay(TimeConverter.MinToMS(game.PhaseLengthInMin / 2), game.Token.Token);
            (context.Client as DiscordSocketClient).MessageReceived -= voteHandler;
        }

        private static async Task RunDayRecap(GamePlayerList game)
        {
            game.Phase = Phases.Night;

            foreach (Player player in game.Objects.Where(x => x.Alive))
            {
                await game.GameChat.AddPermissionOverwriteAsync(player.User, new OverwritePermissions(sendMessages: PermValue.Deny, readMessages: PermValue.Allow));
            }

            EmbedBuilder builder = new EmbedBuilder() { Title = $"Day {game.PhaseCounter} Recap", Color = Color.DarkOrange, Description = $":city_sunset: @everyone the Day phase has ended! Recapping now... :city_sunset:\n\n" };
            IUserMessage msg = await (game.GameChat as IMessageChannel).SendMessageAsync("", false, builder.Build());

            Commands.Commands.Game_Commands.CountVotes(game);

            List<Player> list = game.Objects.Where(x => x.Alive).OrderByDescending(x => x.VotesOn).ToList();
            Player lynchee = list.FirstOrDefault();
            if (list.Where(x => x.VotesOn == lynchee.VotesOn).Count() > 1)
            {
                builder.Description += "It seems like everyone was very indecisive today,\nand because of there being no majority, nobody got lynched.\n\n";
            }
            else
            {
                lynchee.Alive = false;
                if (lynchee.Role.Allignment == Allignment.Mafia) { game.MafiaAlive--; await game.MafiaChat.AddPermissionOverwriteAsync(lynchee.User, new OverwritePermissions(sendMessages: PermValue.Deny, readMessages: PermValue.Allow)); }
                if (lynchee.Role.Allignment == Allignment.Town) game.TownAlive--;

                await game.GraveyardChat.AddPermissionOverwriteAsync(lynchee.User, new OverwritePermissions(readMessages: PermValue.Allow, sendMessages: PermValue.Allow));
                builder.Description += $"It seems like all of you have decided on your lynch target, **{lynchee.User.Username}**, so let's see what they are!\n";
                builder.Description += $" **Role PM: **\n```{ lynchee.Role.RolePM}```\n";
            }
            await msg.ModifyAsync(x => x.Embed = builder.Build());

            if (await CheckWinConditions(game)) return;

            await (game.GameChat as IMessageChannel).SendMessageAsync("", false, new EmbedBuilder() { Color = Color.DarkBlue, Title = $"Night {game.PhaseCounter} start", Description = $":full_moon: It is now Night {game.PhaseCounter}. The phase will end in {game.PhaseLengthInMin / 2} minutes. :full_moon:" });

            foreach (Player player in game.Objects)
            {
                player.LynchTarget = null;
                if (player.Alive)
                    await game.GameChat.AddPermissionOverwriteAsync(player.User, new OverwritePermissions(readMessages: PermValue.Allow, sendMessages: PermValue.Allow));

            }
        }

        #endregion

        private static async Task<bool> CheckWinConditions(GamePlayerList game)
        {
            if (game.MafiaAlive <= 0)
            {
                game.Phase = Phases.EndPhase;
                await (game.GameChat as IMessageChannel).SendMessageAsync("", false, new EmbedBuilder() { Title = "End of game!", Color = Color.Gold, Description = "**__Congratulations Town! All Mafia Players are dead. You win the game!__**" });
                ShowAllPlayers(game, Allignment.Town);
                return true;
            }
            else if (game.MafiaAlive >= game.TownAlive)
            {

                game.Phase = Phases.EndPhase;
                await (game.GameChat as IMessageChannel).SendMessageAsync("", false, new EmbedBuilder() { Title = "End of game!", Color = Color.Gold, Description = "**__Congratulations Mafia! You have outnumbered the town. You win the game!__**" });
                ShowAllPlayers(game, Allignment.Mafia);
                return true;
            }
            return false;
        }

        private static async void ShowAllPlayers(GamePlayerList game, Allignment winningAllignment)
        {

            EmbedBuilder builder = new EmbedBuilder() {Title = "Everyone & Their Roles", Color = Color.Gold};
            EmbedFieldBuilder winField = new EmbedFieldBuilder() {Name = "The Winners", IsInline = false};
            EmbedFieldBuilder loseField = new EmbedFieldBuilder() {Name = "The Losers", IsInline = false};

            foreach (Player player in game.Objects)
            {
                //TODO: Make this actually look good, instead of just a spacing with a single tab.
                if(player.Role.Allignment == winningAllignment)
                {
                    winField.Value += $"{player.User.Nickname ?? player.User.Username}\tas\t{player.Role.Allignment.ToString()} {player.Role.Title}\n";
                }
                else
                {
                    loseField.Value += $"{player.User.Nickname ?? player.User.Username}\tas\t{player.Role.Allignment.ToString()} {player.Role.Title}\n";
                }
            }
            builder.AddField(winField);
            builder.AddField(loseField);
            await (game.GameChat as IMessageChannel).SendMessageAsync("", false, builder.Build());
            try
            {
                await (game.SignupChannel as IMessageChannel).SendMessageAsync("", false, builder.Build());
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
        private static async Task VoteHandler(SocketMessage e, GamePlayerList game, ICommandContext context)
        {
            if (e.Channel.Id == game.GameChat.Id)
            {
                if (e.Content.StartsWith("VOTE: ") && e.MentionedUsers.Count == 1)
                {
                    IGuildUser target;
                    if (game.InGame(target = (e.MentionedUsers.FirstOrDefault() as IGuildUser)) && game.Find(e.Author as IGuildUser).Alive)
                    {
                        if (game.Find(target).Alive)
                        {
                            game.Find(e.Author as IGuildUser).LynchTarget = game.Find(target);
                            await (e as IUserMessage).AddReactionAsync(new Emoji("✅"));
                            return;
                        }
                        else
                        {
                            await (e as IUserMessage).AddReactionAsync(new Emoji("❌"));
                            return;
                        }
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
                            return;
                        }
                        else
                        {
                            await (e as IUserMessage).AddReactionAsync(new Emoji("❌"));
                            return;
                        }
                    }
                    catch (Exception) { }
                }
                else if (e.Content.StartsWith("VOTE: "))
                {
                    await e.Author.SendMessageAsync("Please mention only 1 user who you're voting for.");
                    await (e as IUserMessage).AddReactionAsync(new Emoji("❌"));
                    return;
                }
            }
        }
        #endregion
    }
}

