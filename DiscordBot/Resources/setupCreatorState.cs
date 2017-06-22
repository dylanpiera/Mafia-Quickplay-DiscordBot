using Discord;
using Discord.Commands;
using System.Threading.Tasks;

namespace DiscordBot.Resources
{
    enum setupState
    {
        none,
        name,
        nameReceived,
        lynchTie,
        majorityType,
        dayLength,
        nightLength,
        lenghtReceived,
        roles
    }
}
