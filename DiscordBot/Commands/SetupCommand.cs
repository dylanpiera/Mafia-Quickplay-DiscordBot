using Discord;
using Discord.Commands;
using MySql.Data.MySqlClient;
using System;
using System.Threading.Tasks;

namespace DiscordBot.Commands
{
    static class SetupCommand
    {
        public static void createCommand(DiscordClient _client)
        {
            _client.GetService<CommandService>().CreateCommand("setup")
                .Do(async e =>
                {
                    MySqlConnection conn = new MySqlConnection(Sneaky.connectionString);

                    Message m1 = await e.Channel.SendMessage("Setup Menu\nFor a list of all setups, type `1`\nTo create your own setup, type `2`\nTo edit one of your setups, type `3`");
                    Message m2 = null;

                    EventHandler<MessageEventArgs> menuHandler = null, setupCreator = null;
                    menuHandler = new EventHandler<MessageEventArgs>(async (f, s) => 
                    {
                        MessageEventArgs sender = s;
                        if (sender.User.Id == e.User.Id && e.Channel.Id == sender.Channel.Id)
                        {
                            if (sender.Message.RawText.StartsWith("1"))
                            {
                                await sender.Message.Delete();
                                await m1.Delete();
                                if (m2 != null) await m2.Delete();
                                showGameList(e, conn);
                                _client.MessageReceived -= menuHandler;
                            }
                            if (sender.Message.RawText.StartsWith("2"))
                            {
                                await sender.Message.Delete();
                                await m1.Delete();
                                if (m2 != null) await m2.Delete();
                                //run create setup handler

                                //add setup creation handler
                                _client.MessageReceived -= menuHandler;
                            }
                            if (sender.Message.RawText.StartsWith("3"))
                            {
                                await sender.Message.Delete();
                                if (m2 != null) await m2.Delete();
                                m2 = await e.Channel.SendMessage("This feature has yet to be implemented, Returning to menu.");
                            }
                        }
                    });
                    setupCreator = new EventHandler<MessageEventArgs>(async (f, s) =>
                    {
                        await Task.Delay(0);
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
