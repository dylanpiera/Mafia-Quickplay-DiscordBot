using Discord;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

public class GameLogger
{
    public IMessageChannel Channel { get; private set; }

    public GameLogger(IMessageChannel channel)
    {
        this.Channel = channel;
    }

    public static void LogError(string error)
    {
        Console.WriteLine("-An error occured- " + error);
    }

    public static void LogToConsole(string log)
    {
        Console.WriteLine("-Log- " + log);
    }

    public async void Log(string log, IUser user)
    {
        await Channel.SendMessageAsync("", false, new EmbedBuilder() { Color = Color.DarkBlue, Author = new EmbedAuthorBuilder() { Name = "Monitoring Engine", IconUrl = "https://cdn4.iconfinder.com/data/icons/business-bicolor-1/512/spy-512.png"}, ThumbnailUrl = user?.GetAvatarUrl(), Description = log});
    }
    public async void Log(string log, string imgUrl = "")
    {
        await Channel.SendMessageAsync("", false, new EmbedBuilder() { Color = Color.DarkBlue, Author = new EmbedAuthorBuilder() { Name = "Monitoring Engine", IconUrl = "https://cdn4.iconfinder.com/data/icons/business-bicolor-1/512/spy-512.png" }, ThumbnailUrl = imgUrl, Description = log });
    }

}
