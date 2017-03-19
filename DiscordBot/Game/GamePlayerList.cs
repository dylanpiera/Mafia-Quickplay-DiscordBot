using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;

namespace DiscordBot.Game
{
    class GamePlayerList
    {
        public List<User> Objects
        {
            get
            {
                return players;
            }
        }

        private List<User> players;

        public bool gameRunning { get; set; }

        public GamePlayerList()
        {
            players = new List<User>();
            gameRunning = false;
        }

        public void Add(User player)
        {
            this.players.Add(player);
        }

        public void Remove(User player)
        {
            this.players.Remove(player);
        }

        public User Find(string userName)
        {
            foreach (User player in players)
            {
                if (player.Name == userName) return player;
            }

            return null;
        }

        public bool inGame(User player)
        {
            foreach (User item in players)
            {
                if (item == player) return true;
            }

            return false;
        }

    }
}
