using Discord;
using Discord.Commands;
using DiscordBot.Game;
using DiscordBot.Resources;
using DiscordBot.Roles;
using DiscordBot.Roles.RoleUtil;
using DiscordBot.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord_Mafia_Bot.Core
{
    public static class StartGame
    {
        /// <summary>
        /// Start the process of starting a game
        /// </summary>
        /// <param name="Context">Command Context</param>
        /// <param name="game">The game</param>
        public static async void startGame(ICommandContext Context, GamePlayerList game)
        {
            IGuildChannel gameChannel = await Context.Guild.CreateTextChannelAsync("Mafia-Game-Room", new RequestOptions() {AuditLogReason = "Start of Mafia Game" });
            await gameChannel.AddPermissionOverwriteAsync(Context.Client.CurrentUser, new OverwritePermissions(manageChannel: PermValue.Allow, addReactions: PermValue.Allow,readMessages: PermValue.Allow, sendMessages: PermValue.Allow, mentionEveryone: PermValue.Allow, managePermissions: PermValue.Allow), new RequestOptions { AuditLogReason = "Start of Mafia Game" });
            await gameChannel.AddPermissionOverwriteAsync(Context.Guild.EveryoneRole, new OverwritePermissions(readMessages: PermValue.Deny, sendMessages: PermValue.Deny, addReactions: PermValue.Deny), new RequestOptions {AuditLogReason = "Start of Mafia Game" });

            foreach (Player players in game.Objects)
            {
                await gameChannel.AddPermissionOverwriteAsync(players.User, new OverwritePermissions(readMessages: PermValue.Allow, sendMessages: PermValue.Deny));
            }

            EmbedBuilder builder = new EmbedBuilder() {Color = Color.LightGrey, Title = "Starting game..." };
            builder.Description += "@everyone the game is starting up...\n\n";
            IUserMessage msg = await (gameChannel as IMessageChannel).SendMessageAsync("",false,builder.Build());
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
            await mafiaChannel.AddPermissionOverwriteAsync(Context.Client.CurrentUser, new OverwritePermissions(manageChannel: PermValue.Allow, addReactions: PermValue.Allow,readMessages: PermValue.Allow, sendMessages: PermValue.Allow, mentionEveryone: PermValue.Allow, managePermissions: PermValue.Allow), new RequestOptions { AuditLogReason = "Start of Mafia Game" });
            await mafiaChannel.AddPermissionOverwriteAsync(Context.Guild.EveryoneRole, new OverwritePermissions(readMessages: PermValue.Deny, sendMessages: PermValue.Deny, addReactions: PermValue.Deny), new RequestOptions { AuditLogReason = "Start of Mafia Game" });
            game.setChats(gameChannel, mafiaChannel);

            foreach (Player player in game.Objects)
            {
                if(player.Role.Allignment == Allignment.Mafia)
                    await game.MafiaChat.AddPermissionOverwriteAsync(player.User, new OverwritePermissions(readMessages: PermValue.Allow,sendMessages: PermValue.Allow));
            }

            await (game.MafiaChat as IMessageChannel).SendMessageAsync("", false, new EmbedBuilder() {Title = "Welcome Scummy friends :smiling_imp:", Description = "You can freely discuss in this chat during both day & night phases.\nOnce night hits I will make an announcement for you to post your Night Kill Target." });
            await Task.Delay(500);

            builder.Description += "Game preperation, completed. :white_check_mark:";
            await msg.ModifyAsync(x => x.Embed = builder.Build());

            game.Phase = Phases.Day;
            game.PhaseCounter = 1;

            EmbedBuilder b = new EmbedBuilder() {Color = Color.LightOrange, Title = "Start of D1", Description = $":sunny: @everyone **It is now Day 1.** Day 1 will end in {game.PhaseLengthInMin} minutes. :sunny:" };
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
            //GameManager.runGame(game, context);

        }

        private static void distributeRoles(GamePlayerList game, List<MafiaRole> setup)
        {
            List<Player> players = ListHelper.ShuffleList<Player>(game.Objects);
            List<MafiaRole> roles = ListHelper.ShuffleList<MafiaRole>(setup);
            MafiaRole role;

            if(players.Count == roles.Count)
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
    }
}

