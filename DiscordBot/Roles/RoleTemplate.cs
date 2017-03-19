using Discord;
using DiscordBot.Roles.RoleUtil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBot.Roles
{
    class RoleTemplate
    {
        Allignment alignment;
        Wincon wincon;
        String title;
        String description; //Including explanation on how to use power


        public virtual void Power() { }
    }
}
