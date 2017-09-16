using Discord;
using Discord.Commands;
using DiscordBot.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.SqlClient;
using MySql.Data.MySqlClient;

namespace DiscordBot.Commands
{
    static class TestCommand
    {
        public static void createCommand2(DiscordClient _client)
        {
            _client.GetService<CommandService>().CreateCommand("test")
               .Hide()
               .Do(async c =>
               {
                   MySqlConnection conn = new MySqlConnection();
                   Console.WriteLine("Running !test");
                   conn.ConnectionString = Sneaky.connectionString;
                   Console.WriteLine("Attempting to connect...");
                   try
                   {
                       await conn.OpenAsync();
                       await c.Channel.SendMessage("Succesfully Connected!");
                       conn.Close();
                       conn.Dispose();
                   }
                   catch (Exception e)
                   {
                       await c.Channel.SendMessage("Failed to Connect.");
                       Console.WriteLine(e.ToString());
                   }
                   finally
                   {
                       Console.WriteLine("End of !test");
                   }
               });
        }

        public static void createCommand(DiscordClient _client)
        {
            _client.GetService<CommandService>().CreateCommand("mod")
                .Hide()
                .Do(async c =>
                {
                    Message message = await c.Channel.SendMessage("**Moderator Menu:**\n```1. force game to start\n2. force game to end\n3. force member to join. Parameter: [mention]```respond with `[number] [parameter]`");

                    List<Object> test = new List<Object>();
                    test.Shuffle<Object>();

                    EventHandler<MessageEventArgs> handler = null;
                    handler = new EventHandler<MessageEventArgs>((f, s) => eventHandler(f, s, c, message, _client, handler));

                    _client.MessageReceived += handler;

                });
        }
        static async void eventHandler(object f, MessageEventArgs s, CommandEventArgs c, Message message, DiscordClient _client, EventHandler<MessageEventArgs> handler)
        {
            if (s.User.Id == c.User.Id)
            {
                if (s.Message.RawText.StartsWith("1"))
                {
                    await message.Delete();
                    await c.Channel.SendMessage("You forced the game to start.");
                    _client.MessageReceived -= handler;
                }
                else if (s.Message.RawText.StartsWith("2"))
                {
                    await message.Delete();
                    await c.Channel.SendMessage("You forced the game to end.");
                    _client.MessageReceived -= handler;
                }
                else if (s.Message.RawText.StartsWith("3") && s.Message.MentionedUsers.Count() != 0)
                {
                    await message.Delete();
                    await c.Channel.SendMessage("You added " + s.Message.MentionedUsers.FirstOrDefault() + " to the game");
                    _client.MessageReceived -= handler;
                }
                else if (s.Message.RawText.StartsWith("exit"))
                {
                    await message.Delete();
                    _client.MessageReceived -= handler;
                }
                else
                {
                    await c.Channel.SendMessage("Please follow the format. or exit by saying `exit` " + c.User.Mention);
                }
            }
        }
    }
}
