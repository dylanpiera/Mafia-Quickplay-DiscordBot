using Discord;
using Discord.Commands;
using System;
using System.Linq;

namespace DiscordBot.Commands
{
    static class TestCommand
    {
        public static void createCommand(DiscordClient _client)
        {
            _client.GetService<CommandService>().CreateCommand("mod")
                .Do(async c =>
                {
                    Message message = await c.Channel.SendMessage("**Moderator Menu:**\n```1. force game to start\n2. force game to end\n3. force member to join. Parameter: [mention]```respond with `[number] [parameter]`");


                    EventHandler<MessageEventArgs> handler = null;
                    handler = new EventHandler<MessageEventArgs>((f, s) => eventHandler(f, s, c,message, _client, handler));

                    _client.MessageReceived += handler;

                });
        }
        static async void eventHandler(object f, MessageEventArgs s, CommandEventArgs c,Message message, DiscordClient _client, EventHandler<MessageEventArgs> handler)
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
                else if(s.Message.RawText.StartsWith("exit"))
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
