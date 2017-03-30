using Discord;
using DiscordBot.Roles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBot.Game {
    class Spectator {
        private User user;

        public Spectator(User user) {
            this.user = user;
        }

        public User User {
            get {
                return user;
            }
        }
    }
}
