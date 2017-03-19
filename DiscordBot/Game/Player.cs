using Discord;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBot.Game
{
    class Player
    {
        private User user;

        public Player(User user)
        {
            this.user = user;
        }



        public User User
        {
            get
            {
                return user;
            }
        }
    }
}
