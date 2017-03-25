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
        public async static void runGame(GamePlayerList g)
        {
            await Task.Delay(0);
            do
            {
                if (g.Phase == Phases.Day)
                {
                    g.Token = new CancellationTokenSource();

                    try
                    {
                        if (await runDayPhase(g))
                        {
                            await runDayRecap(g);
                            await g.GameChat.SendMessage("Debugging: Day ended normally.");
                        }
                    } catch(Exception)
                    {
                        await g.GameChat.SendMessage("Debugging: Day ended forcefully.");
                        await runDayRecap(g);
                    }

                    

                }
                else if (g.Phase == Phases.Night)
                {
                    g.Token = new CancellationTokenSource();

                    try
                    {
                        if (await runNightPhase(g))
                        {
                            await runNightRecap(g);
                            await g.GameChat.SendMessage("Debugging: Night ended normally.");
                        }
                    }
                    catch (Exception)
                    {
                        await g.GameChat.SendMessage("Debugging: Night ended forcefully.");
                        await runNightRecap(g);
                    }
                }
                else
                {
                    g.gameRunning = false;
                }
            } while (g.gameRunning);

        }

        public static async Task<bool> runNightPhase(GamePlayerList g)
        {
            await g.MafiaChat.SendMessage($"Dear Scum, It is now Night {g.PhaseCounter}. Please select your Night Kill target with `!kill`\n\n`!help kill` _for more info about the kill command._\n\nThe last target selected with `!kill` will be killed.");

            await Task.Delay(TimeConverter.MinToMS((g.PhaseLengthInMin / 2)), g.Token.Token);
            await g.GameChat.SendMessage($":warning: There are only {g.PhaseLengthInMin / 2} minutes left in the night phase. :warning:");

            await Task.Delay(TimeConverter.MinToMS((g.PhaseLengthInMin / 2)), g.Token.Token);
            return true;
        }

        public static async Task<bool> runNightRecap(GamePlayerList g)
        {
            g.Phase = Phases.Day; g.PhaseCounter++;
            await g.GameChat.SendMessage("`runNightRecap(GamePlayerList g)` has yet to be implemented.");
            return true;
        }

        public static async Task<bool> runDayPhase(GamePlayerList g)
        {
            await Task.Delay(TimeConverter.MinToMS((g.PhaseLengthInMin/2)), g.Token.Token);
            VoteTallyCommand.countVotes(g);
            int i = 0; string playerList = "";
            List<Player> SortedList = g.Objects.OrderByDescending(o => o.VotesOn).ToList();
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
            return true;
        }

        public static async Task<bool> runDayRecap(GamePlayerList g)
        {
            g.Phase = Phases.Night;

            await g.GameChat.SendMessage(":city_sunset: @everyone day phase has ended! Recapping now... :city_sunset: ");
            //TODO: !vote & !unvote won't work anymore
            foreach (var item in g.Objects.Where(x => x.Alive == true))
            {
                await g.GameChat.AddPermissionsRule(item.User, new ChannelPermissionOverrides(readMessages: PermValue.Allow, sendMessages: PermValue.Deny));
            }

            VoteTallyCommand.countVotes(g);

            Player[] list = g.Objects.OrderByDescending(x => x.VotesOn).ToArray();

            if(list.Where(x => x.VotesOn == list[0].VotesOn).Count() > 1)
            {
                await g.GameChat.SendMessage("It seems like everyone was very indecisive tonight,\nand because of there being no majority, nobody got lynched.");
            }
            else
            {
                list[0].Alive = false;

                await g.GameChat.SendMessage($"It seems like all of you have decided on your lynch target, **{list[0].User.Name}**, so let's see what they are!");
                await g.GameChat.SendMessage($"**Role PM:**\n{list[0].Role.RolePM}\n");
            }

            await g.GameChat.SendMessage($":full_moon: It is now Night {g.PhaseCounter}. The phase will end in {g.PhaseLengthInMin} minutes. :full_moon:");
            foreach (var item in g.Objects)
            {
                if(item.Alive == true) await g.GameChat.AddPermissionsRule(item.User, new ChannelPermissionOverrides(readMessages: PermValue.Allow, sendMessages: PermValue.Allow));
                item.LynchTarget = null;
            }
            return true;
        }
    }
}
