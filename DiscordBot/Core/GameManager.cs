using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using DiscordBot.Game;
using DiscordBot.Util;
using DiscordBot.Commands;
using System.Threading;

namespace DiscordBot.Core
{
    static class GameManager
    {
        public async static void runGame(GamePlayerList g, DiscordClient _client)
        {
            do
            {
                if (g.Phase == Phases.Day)
                {
                    g.Token = new CancellationTokenSource();

                    try
                    {
                        if (await runDayPhase(g, _client))
                        {
                        }
                    } catch(Exception)
                    {
                    }
                    finally
                    {
                        await runDayRecap(g);
                    }
                }
                else if (g.Phase == Phases.Night)
                {
                    g.Token = new CancellationTokenSource();

                    try
                    {
                        if (await runNightPhase(g, _client))
                        {
                        }
                    }
                    catch (Exception)
                    {
                    }
                    finally
                    {
                        await runNightRecap(g, _client);
                    }
                }
                else
                {
                    await g.GameChat.SendMessage("Stopping game in 1 minute...");
                    await Task.Delay(TimeConverter.MinToMS(1));
                    g.Reset();
                }
            } while (g.gameRunning);

            
        }

        public static async Task<bool> checkWinConditions(GamePlayerList g)
        {
            if(g.MafiaAlive <= 0)
            {
                g.Phase = Phases.EndPhase;
                await g.GameChat.SendMessage($"**__Congratulations Town! All Mafia Players are dead. You win the game!__**");
                return true;
            }
            if(g.MafiaAlive >= g.TownAlive)
            {
                g.Phase = Phases.EndPhase;
                await g.GameChat.SendMessage($"**__Congratulations Mafia! You have outnumbered the town. You win the game!__**");
                return true;
            }

            return false;
        }

        public static async Task<bool> runNightPhase(GamePlayerList g, DiscordClient _client)
        {
            handlePowerRoles(g, _client);

            g.NightkillHandler = null;
            g.NightkillHandler = new EventHandler<MessageEventArgs>((s, e) => NightkillHandler(s, e, g, _client));
            _client.MessageReceived += g.NightkillHandler;

            await g.MafiaChat.SendMessage($"Dear Scum, It is now Night {g.PhaseCounter}. Please select your Night Kill target by typing `KILL: [playername]`. ");
            


            await Task.Delay(TimeConverter.MinToMS((g.PhaseLengthInMin / 2)), g.Token.Token);
            await g.GameChat.SendMessage($":warning: There are only {g.PhaseLengthInMin / 2} minutes left in the night phase. :warning:");

            await Task.Delay(TimeConverter.MinToMS((g.PhaseLengthInMin / 2)), g.Token.Token);
            return true;
        }

        private static async void NightkillHandler(object s, MessageEventArgs e, GamePlayerList g, DiscordClient _client)
        {
            if(e.Channel.Id == g.MafiaChat.Id && e.Message.RawText.StartsWith("KILL: "))
            {
                string target = e.Message.RawText.Replace("KILL: ", "");
                if (g.inGame(g.Find(target)))
                {
                    g.MafiaKillTarget = g.Find(target);
                    if (g.MafiaKillTarget.User.Nickname != null)
                        await g.MafiaChat.SendMessage($"The current kill target is: {g.MafiaKillTarget.User.Nickname}. Use `KILL: [playername]` to change your target.");
                    else
                        await g.MafiaChat.SendMessage($"The current kill target is: {g.MafiaKillTarget.User.Name}. Use `KILL: [playername]` to change your target.");
                }
                else
                {
                    await g.MafiaChat.SendMessage($"Your input was invalid. You inputted: {target}");
                }
            }
        }

        public static async Task<bool> runNightRecap(GamePlayerList g, DiscordClient _client)
        {
            g.Phase = Phases.Day; g.PhaseCounter++;

            await g.GameChat.SendMessage(":night_with_stars: @everyone the Night phase has ended! Recapping now... :night_with_stars:");
            foreach (var item in g.Objects.Where(x => x.Alive == true))
            {
                await g.GameChat.AddPermissionsRule(item.User, new ChannelPermissionOverrides(readMessages: PermValue.Allow, sendMessages: PermValue.Deny));
            }

            _client.MessageReceived -= g.NightkillHandler;
            g.Objects.ForEach(async x =>
            {
                _client.MessageReceived -= x.Role.PowerHandler(g);
                await x.Role.powerResult(x.User, x.Role.Target);
            });



            if(g.MafiaKillTarget != null && g.MafiaKillTarget != g.Objects.Where(x => x.Role.Title=="Doctor").FirstOrDefault().Role.Target)
            {
                await g.GameChat.SendMessage($"When everyone woke up in the morning, they found out someone was missing: {g.MafiaKillTarget.User.Name}\nOnce they arived at their home, they were found death on the ground.\n\n**{g.MafiaKillTarget.User.Name} was killed by the Mafia. They were:**");
                await g.GameChat.SendMessage($"**Role PM:**\n```{g.MafiaKillTarget.Role.RolePM}```\n");
                g.MafiaKillTarget.Alive = false;
                if (g.MafiaKillTarget.Role.Allignment == Roles.RoleUtil.Allignment.Mafia) g.MafiaAlive--;
                else if(g.MafiaKillTarget.Role.Allignment == Roles.RoleUtil.Allignment.Town) g.TownAlive--;
                g.MafiaKillTarget = null;
            } else
            {
                if (g.MafiaKillTarget != null)
                    await g.MafiaChat.SendMessage($"__Your kill on {g.MafiaKillTarget.User.Mention} was unsuccesful.__");
                await g.GameChat.SendMessage($"Tonight has been a quiet night... Nothing happened...");
            }

            if (await checkWinConditions(g)) return true;

            await g.GameChat.SendMessage($":sunny: It is now Day {g.PhaseCounter}. The phase will end in {g.PhaseLengthInMin} minutes. :sunny:");
            foreach (var item in g.Objects)
            {
                if (item.Alive == true) await g.GameChat.AddPermissionsRule(item.User, new ChannelPermissionOverrides(readMessages: PermValue.Allow, sendMessages: PermValue.Allow));
                else
                {
                    if (item.Role.Allignment == Roles.RoleUtil.Allignment.Mafia) await g.MafiaChat.AddPermissionsRule(item.User, new ChannelPermissionOverrides(readMessages: PermValue.Allow, sendMessages: PermValue.Deny));
                }
            }
            return true;

        }

        private static void handlePowerRoles(GamePlayerList g, DiscordClient _client)
        {
            g.Objects.ForEach(async x =>
            {
                if (x.Role.PowerRole)
                {
                    await x.User.SendMessage(x.Role.Power);
                    _client.MessageReceived += x.Role.PowerHandler(g);
                }
            });
        }

        static async void VoteHandler(object s, MessageEventArgs e, GamePlayerList g, DiscordClient _client)
        {
            if(e.Channel.Id == g.GameChat.Id)
            {
                if (e.Message.RawText.StartsWith("VOTE: ") && e.Message.MentionedUsers.Count() != 0)
                {
                    if (g.inGame(e.Message.MentionedUsers.FirstOrDefault()))
                    {
                        g.Find(e.User).LynchTarget = g.Find(e.Message.MentionedUsers.FirstOrDefault());
                        await e.User.SendMessage("You're currently voting for: " + e.Message.MentionedUsers.FirstOrDefault());
                    }
                } else if (e.Message.RawText.StartsWith("UNVOTE"))
                {
                    try
                    {
                        g.Find(e.User).LynchTarget = null;
                    } catch (Exception) { }
                }
            }
        }

        public static async Task<bool> runDayPhase(GamePlayerList g, DiscordClient _client)
        {

            EventHandler<MessageEventArgs> voteHandler = null;
            voteHandler = new EventHandler<MessageEventArgs>((s, e) => VoteHandler(s, e, g, _client));
            _client.MessageReceived += voteHandler;

            await Task.Delay(TimeConverter.MinToMS((g.PhaseLengthInMin/2)), g.Token.Token);
            VoteTallyCommand.countVotes(g);
            int i = 0; string playerList = "";
            List<Player> SortedList = g.Objects.Where(x => x.Alive == true).OrderByDescending(o => o.VotesOn).ToList();
            foreach (var item in SortedList)
            {
                i++;
                try
                {
                    playerList += $"{i}. {item.User.Name} " + item.VotesOn + ": " + VoteTallyCommand.votedFor(SortedList, item) + "\n";
                }
                catch (Exception) { }
            }
            await g.GameChat.SendMessage($":warning: There are only {g.PhaseLengthInMin/2} minutes left in the day phase. :warning:\n\nMid day vote count:\n```{playerList}```");

            await Task.Delay(TimeConverter.MinToMS((g.PhaseLengthInMin / 2)), g.Token.Token);
            _client.MessageReceived -= voteHandler;
            return true;
        }

        public static async Task<bool> runDayRecap(GamePlayerList g)
        {
            g.Phase = Phases.Night;

            await g.GameChat.SendMessage(":city_sunset: @everyone the Day phase has ended! Recapping now... :city_sunset: ");
           
            foreach (var item in g.Objects.Where(x => x.Alive == true))
            {
                await g.GameChat.AddPermissionsRule(item.User, new ChannelPermissionOverrides(readMessages: PermValue.Allow, sendMessages: PermValue.Deny));
            }

            VoteTallyCommand.countVotes(g);

            Player[] list = g.Objects.OrderByDescending(x => x.VotesOn).ToArray();

            if(list.Where(x => x.VotesOn == list[0].VotesOn).Count() > 1)
            {
                await g.GameChat.SendMessage("It seems like everyone was very indecisive today,\nand because of there being no majority, nobody got lynched.");
            }
            else
            {
                list[0].Alive = false;
                if (list[0].Role.Allignment == Roles.RoleUtil.Allignment.Mafia) g.MafiaAlive--;
                else if (list[0].Role.Allignment == Roles.RoleUtil.Allignment.Town) g.TownAlive--;

                await g.GameChat.SendMessage($"It seems like all of you have decided on your lynch target, **{list[0].User.Name}**, so let's see what they are!");
                await g.GameChat.SendMessage($"**Role PM:**\n```{list[0].Role.RolePM}```\n");
            }

            if (await checkWinConditions(g)) return true;

            await g.GameChat.SendMessage($":full_moon: It is now Night {g.PhaseCounter}. The phase will end in {g.PhaseLengthInMin} minutes. :full_moon:");
            foreach (var item in g.Objects)
            {
                if(item.Alive == true) {
                    await g.GameChat.AddPermissionsRule(item.User, new ChannelPermissionOverrides(readMessages: PermValue.Allow, sendMessages: PermValue.Allow));
                } else
                {
                    if(item.Role.Allignment == Roles.RoleUtil.Allignment.Mafia) await g.MafiaChat.AddPermissionsRule(item.User, new ChannelPermissionOverrides(readMessages: PermValue.Allow, sendMessages: PermValue.Deny));
                }

                item.LynchTarget = null;
            }
            return true;
        }
    }
}
