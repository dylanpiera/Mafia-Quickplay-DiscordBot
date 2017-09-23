using Discord;
using Discord.Commands;
using Discord_Mafia_Bot.Util;
using System.Linq;
using System.Threading.Tasks;

namespace Discord_Mafia_Bot
{
    //someone remind me to clean this up and make it look beautiful.
    public class HelpModule : ModuleBase
    {
        private readonly CommandService _service;
        string prefix = "!";

        public HelpModule(CommandService service)
        {
            _service = service;
        }

        [Command("help"), Hidden()]
        public async Task HelpAsync()
        {
            var builder = new EmbedBuilder()
            {
                Color = new Color(114, 137, 218),
                Description = "These are the commands you can use"
            };

            string nName, oName = "";

            foreach (var module in _service.Modules)
            {
                string description = null;
                foreach (var cmd in module.Commands)
                {
                    var result = await cmd.CheckPreconditionsAsync(Context);
                    if (result.IsSuccess && cmd.Attributes.All(x => !(x is HiddenAttribute)))
                    {
                        nName = cmd.Aliases.First();
                        if (nName != oName)
                        {
                            description += $"{prefix}{nName}";
                            /*if (cmd.Preconditions.Count > 0)
                                description += $" ({cmd.Preconditions.First().ToString()})";*/
                            description += "\n";
                            oName = nName;
                        }
                    }
                        
                }

                if (!string.IsNullOrWhiteSpace(description))
                {
                    builder.AddField(x =>
                    {
                        x.Name = module.Name;
                        if (!string.IsNullOrWhiteSpace(module.Summary))
                            x.Value = $"_{module.Summary}_\n{description}";
                        else
                            x.Value = description;
                        x.IsInline = true;
                    });
                }
            }

            await ReplyAsync("", false, builder.Build());
        }

        [Command("help"), Hidden()]
        public async Task HelpAsync(string command)
        {
            var result = _service.Search(Context, command);

            if (!result.IsSuccess)
            {
                await ReplyAsync($"Sorry, I couldn't find a command like **!{command}**.");
                return;
            }
            
            var builder = new EmbedBuilder()
            {
                Color = new Color(114, 137, 218),
                Description = $"Here are some commands like **!{command}**"
            };

            foreach (var match in result.Commands)
            {
                var cmd = match.Command;
                if (cmd.Attributes.All(x => !(x is HiddenAttribute)))
                {
                    builder.AddField(x =>
                    {
                        x.Name = "!" + string.Join(", !", cmd.Aliases);
                        string param = $"{string.Join(", ", cmd.Parameters.Select(p => p.Name))}";
                        if(!string.IsNullOrWhiteSpace(param))
                            x.Value = $"Parameters: {string.Join(", ", cmd.Parameters.Select(p => p.Name))}\n" +
                                  $"Summary: {cmd.Summary}";
                        else
                            x.Value = $"Summary: {cmd.Summary}";
                        x.IsInline = false;
                    });
                }
            }

            await ReplyAsync("", false, builder.Build());
        }
    }
}