using Discord;
using DiscordBot.Roles;
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
        private MafiaRole role;

        public Player(User user)
        {
            this.user = user;
        }

        public void AssignRole(MafiaRole role)
        {
            this.role = role;
        }

        public User User
        {
            get
            {
                return user;
            }
        }
        public MafiaRole Role
        {
            get
            {
                return role;
            }
        }
    }
}
