using Discord;
using Discord.Commands;
using DiscordBot.Resources;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DiscordBot.Commands
{
    static class SetupCommand
    {
        public static List<ulong> usersInSetup = new List<ulong>();

        public static void createCommand(DiscordClient _client)
        {
            _client.GetService<CommandService>().CreateCommand("setup")
                .Do(async e =>
                {
                    if (e.Channel.IsPrivate)
                    {
                        await e.Channel.SendMessage("This command can not be started from a PM. Please try to access it through a server. :x:");
                        return;
                    }

                    MySqlConnection conn = new MySqlConnection(Sneaky.connectionString);

                    Message m1 = await e.Channel.SendMessage("Setup Menu\nFor a list of all setups, type `1`\nTo create your own setup, type `2`\nTo edit one of your setups, type `3`");
                    Message m2 = null;
                    setupState state = setupState.none;
                    Setup setup = new Setup();

                    EventHandler<MessageEventArgs> menuHandler = null, setupCreator = null;
                    menuHandler = new EventHandler<MessageEventArgs>(async (f, s) =>
                    {
                        MessageEventArgs sender = s;
                        try
                        {
                            if (sender.User.Id == e.User.Id && e.Channel.Id == sender.Channel.Id && !sender.User.IsBot)
                            {
                                if (sender.Message.RawText.StartsWith("1"))
                                {

                                    await sender.Message.Delete();
                                    await m1.Delete();
                                    if (m2 != null) await m2.Delete();
                                    showGameList(e, conn);
                                    _client.MessageReceived -= menuHandler;
                                }
                                else if (sender.Message.RawText.StartsWith("2"))
                                {
                                    if (SetupCommand.usersInSetup.Contains(sender.User.Id))
                                    {
                                        await sender.Channel.SendMessage("You're already creating a setup. " + sender.User.Mention);
                                        return;
                                    }
                                    await sender.Message.Delete();
                                    if (m2 != null) await m2.Delete();
                                    //run create setup handler
                                    await m1.Edit("Welcome to the setup creator. Setups are handled in PMs.\nInitializing setup creator in your pm... :clock2:");
                                    await e.User.SendMessage("**Welcome to the setup creator.** You can exit at any point by typing `exit`\nPlease provide us with a name for the setup.");
                                    setup.Creator = s.User;
                                    state = setupState.name;
                                    SetupCommand.usersInSetup.Add(sender.User.Id);
                                    //add setup creation handler
                                    _client.MessageReceived += setupCreator;
                                    _client.MessageReceived -= menuHandler;
                                }
                                else if (sender.Message.RawText.StartsWith("3"))
                                {

                                    await sender.Message.Delete();
                                    if (m2 != null) await m2.Delete();
                                    m2 = await e.Channel.SendMessage("This feature has yet to be implemented, Returning to menu.");
                                }
                            }
                        }
                        catch { }
                    });
                    setupCreator = new EventHandler<MessageEventArgs>(async (f, sender) =>
                    {
                        MessageEventArgs s = sender;

                        if (e.User.Id == s.User.Id && e.User.PrivateChannel.Id == s.Channel.Id && !sender.User.IsBot)
                        {
                            if (s.Message.RawText.ToLower().StartsWith("exit")) { _client.MessageReceived -= setupCreator; await s.User.SendMessage(":x: Setup Aborted :x:"); SetupCommand.usersInSetup.Remove(s.User.Id); return; }
                            switch (state)
                            {
                                case setupState.name:
                                    setup.SetupName = s.Message.RawText;
                                    state = setupState.nameReceived;
                                    await s.Channel.SendMessage($"is: ```{setup.SetupName}``` the name the setup is supposed to have? Yes / No");

                                    break;
                                case setupState.nameReceived:
                                    if (s.Message.RawText.ToLower().Equals("y") || s.Message.RawText.ToLower().StartsWith("yes"))
                                    {
                                        await s.Channel.SendMessage($"How long will the day phase last (in minutes) for  your game \"{setup.SetupName}\"\n_min 2 min / max 20 min_");
                                        state = setupState.dayLength;
                                    }
                                    else if (s.Message.RawText.ToLower().Equals("n") || s.Message.RawText.ToLower().StartsWith("no"))
                                    {
                                        await s.Channel.SendMessage("What is the name of your setup ?");
                                        state = setupState.name;
                                    }
                                    else
                                    {
                                        await s.Channel.SendMessage($"Please answer with Yes / No\nIs ```{setup.SetupName}``` the name the setup is supposed to have?");
                                    }
                                    break;

                                case setupState.dayLength:
                                    try
                                    {
                                        int condition = Int32.Parse(new string(s.Message.RawText.Where(c => char.IsDigit(c)).ToArray()));
                                        if (condition >= 2 && condition <= 20)
                                        {
                                            setup.DayLength = condition;
                                            await s.Channel.SendMessage($"How long will the night phase last (in minutes) for  your game \"{setup.SetupName}\"\n_min 2 min / max 20 min_");
                                            state = setupState.nightLength;
                                        }
                                        else await s.Channel.SendMessage("Please provide a number between 2 and 20.");
                                    }
                                    catch
                                    {
                                        await s.Channel.SendMessage("Please provide a number between 2 and 20.");
                                    }

                                    break;
                                case setupState.nightLength:
                                    try
                                    {
                                        int condition = Int32.Parse(new string(s.Message.RawText.Where(c => char.IsDigit(c)).ToArray()));
                                        if (condition >= 2 && condition <= 20)
                                        {
                                            setup.NightLength = condition;
                                            await s.Channel.SendMessage($"The Day / Night phase lenghts for {setup.SetupName} are: {setup.DayLength} min / {setup.NightLength} min. Is this correct? Yes / No");
                                            state = setupState.lenghtReceived;
                                        }
                                        else await s.Channel.SendMessage("Please provide a number between 2 and 20.");
                                    }
                                    catch
                                    {
                                        await s.Channel.SendMessage("Please provide a number between 2 and 20.");
                                    }

                                    break;

                                case setupState.lenghtReceived:
                                    if (s.Message.RawText.ToLower().Equals("y") || s.Message.RawText.ToLower().StartsWith("yes"))
                                    {
                                        await s.Channel.SendMessage($"The setup you created is:\n```Setup Name: {setup.SetupName} | Created by: {setup.CreatorName} | Day/Night time: {setup.DayLength} min / {setup.NightLength} min ```\n:white_check_mark: Step 1 has been completed. The next step is to assign roles.");
                                        await s.Channel.SendMessage("Roles aren't supported in this version of the setup creator. :x:\n\n:x: Setup couldn't be completed. Aborting setup... :x:");
                                        _client.MessageReceived -= setupCreator;
                                        SetupCommand.usersInSetup.Remove(s.User.Id);
                                    }
                                    else if (s.Message.RawText.ToLower().Equals("n") || s.Message.RawText.ToLower().StartsWith("no"))
                                    {
                                        await s.Channel.SendMessage($"How long will the day phase last (in minutes) for  your game \"{setup.SetupName}\"\n_min 2 min / max 20 min_");
                                        state = setupState.dayLength;
                                    }
                                    else
                                    {
                                        await s.Channel.SendMessage($"Please answer with Yes / No");
                                    }

                                    break;

                                default:
                                    await Task.Delay(0);
                                    break;
                            }
                        }

                    });

                    _client.MessageReceived += menuHandler;

                    await Task.Delay(0);
                });
        }

        static async void showGameList(CommandEventArgs e, MySqlConnection conn)
        {
            using (conn)
            {
                Message loadingMessage = await e.Channel.SendMessage("Establishing Connection to the Database... :clock2:");
                try
                {
                    await conn.OpenAsync();
                    await loadingMessage.Edit("Connection Established. :white_check_mark:");

                    MySqlCommand cmd = conn.CreateCommand();
                    cmd.CommandText = "select * from setups order by setupID";

                    MySqlDataReader reader = await cmd.ExecuteReaderAsync() as MySqlDataReader;

                    string message = "Setups:\n```";
                    int index = 0;
                    while (await reader.ReadAsync() && index < 10)
                    {
                        message += ($"ID: {reader.GetString("setupID")} | Setup name: {reader.GetString("setupName")} | Created by: {reader.GetString("creatorName")} | Day/Night time: {reader.GetString("dayLength")} min / {reader.GetString("nightLength")} min\n");
                        index++;
                    }
                    message += "```";
                    await loadingMessage.Edit(message);

                }
                catch (Exception exc)
                {
                    await loadingMessage.Edit("Error connecting to the database. See log for more details. :x:");
                    Console.WriteLine(exc);
                }
            }

        }
    }
}
