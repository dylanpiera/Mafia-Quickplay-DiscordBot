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
                            runDayRecap(g);
                            await g.GameChat.SendMessage("Debugging: Day ended normally.");
                        }
                    } catch(Exception)
                    {
                        await g.GameChat.SendMessage("Debugging: Day ended forcefully.");
                    }

                    

                }
                else if (g.Phase == Phases.Night)
                {

                }
                else
                {

                }
            } while (g.gameRunning);

        }

        public static async Task<bool> runDayPhase(GamePlayerList g)
        {
            await Task.Delay(TimeConverter.MinToMS(7.5), g.Token.Token);
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
            await g.GameChat.SendMessage($":warning: There are only 7:30 minutes left in the day phase. :warning:\n\nMid day vote count:\n```{playerList}```");

            await Task.Delay(TimeConverter.MinToMS(7.5), g.Token.Token);
            return true;
        }

        public static async void runDayRecap(GamePlayerList g)
        {
            await g.GameChat.SendMessage(":city_sunset: @everyone day phase has ended! Recapping now... :city_sunset: ");
            //TODO: !vote & !unvote won't work anymore
            foreach (var item in g.Objects)
            {
                await g.GameChat.AddPermissionsRule(item.User, new ChannelPermissionOverrides(readMessages: PermValue.Allow, sendMessages: PermValue.Deny));
            }

            VoteTallyCommand.countVotes(g);

            Player[] list = g.Objects.ToArray();
            List<Player> deathrow = new List<Player>();
            foreach (var item in list.Where(x => x.VotesOn == list[0].VotesOn))
            {
                deathrow.Add(item);
                item.Alive = false;

            }



        }
    }
}
