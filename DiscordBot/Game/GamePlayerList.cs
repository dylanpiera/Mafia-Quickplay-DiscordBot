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
        public List<Player> Objects
        {
            get
            {
                return players;
            }
        }

        private List<Player> players;

        public bool gameRunning { get; set; }

        public GamePlayerList()
        {
            players = new List<Player>();
            gameRunning = false;
        }

        public void Add(User player)
        {
            this.players.Add(new Player(player));
        }

        public void Remove(Player player)
        {
            this.players.Remove(player);
        }
        public void Remove(User player)
        {
            foreach (Player item in players)
            {
                if (item.User.Equals(player))
                {
                    players.Remove(item);
                    break;
                }
            }
        }

        public Player Find(string userName)
        {
            foreach (Player player in players)
            {
                if (player.User.Name == userName) return player;
            }

            return null;
        }

        public bool inGame(Player player)
        {
            foreach (Player item in players)
            {
                if (item == player) return true;
            }

            return false;
        }
        public bool inGame(User player)
        {
            foreach (Player item in players)
            {
                if (item.User == player) return true;
            }

            return false;
        }

    }
}
