using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using System.Threading;
using DiscordBot.Util;
using Discord.WebSocket;

namespace DiscordBot.Game
{
    public class GamePlayerList
    {
        private int PhaseLength, phaseCounter, townPlayers, mafiaPlayers, townAlive, mafiaAlive, cops;
        Func<SocketMessage,Task> nightkillHandler;

        public int PhaseCounter
        {
            get
            {
                return phaseCounter;
            }
            set
            {
                phaseCounter = value;
            }
        }

        public int PhaseLengthInMin
        {
            get
            {
                return PhaseLength;
            }
            set
            {
                PhaseLength = value;
            }
        }

        public List<Player> Objects
        {
            get
            {
                return players;
            }
            set
            {
                this.players = value;
            }
        }

        private CancellationTokenSource token;
        private Player mafiaKillTarget;
        private Phases phase;

        public Phases Phase
        {
            get
            {
                return phase;
            }
            set
            {
                phase = value;
            }
        }
        public CancellationTokenSource Token
        {
            get
            {
                return token;
            }
            set
            {
                token = value;
            }
        }

        private List<Player> players;

        private IGuildChannel gameChat, mafiaChat;

        public void setChats(IGuildChannel gameChat, IGuildChannel mafiaChat)
        {
            this.gameChat = gameChat;
            this.mafiaChat = mafiaChat;
        }

        /// <summary>
        /// Sets or gets if the game is running or not
        /// </summary>
        public bool gameRunning { get; set; }

        internal void Reset()
        {
            this.Phase = Phases.EndPhase;
            this.token.Cancel();
            
            this.gameRunning = false;
            this.Objects.Clear();
            this.GameChat.DeleteAsync();
            this.MafiaChat.DeleteAsync();
            this.Cops = 0;
            this.Godfathers = 0;
            this.Doctors = 0;
            this.MafiaPlayers = 0;
            this.TownPlayers = 0;
        }

        public GamePlayerList()
        {
            players = new List<Player>();
            gameRunning = false;
            PhaseLengthInMin = 4;
        }

        /// <summary> Add's a User to the game, and assigns them to an instance of Player</summary>
        /// <param name ="Player">the user to add to the game as a Player</param>
        public void Add(IGuildUser player)
        {
            this.players.Add(new Player(player,this));
        }

        /// <summary> removes a User from the game</summary>
        /// <param name ="Player">the instance of Player to remove from the list</param>
        public void Remove(Player player)
        {
            this.players.Remove(player);
        }
        /// <summary> removes a User from the game with an instance of User</summary>
        /// <param name ="Player">the instance of User to remove from the list</param>
        public void Remove(IGuildUser player)
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

        /// <summary>Finds and returns a player by name</summary>
        /// <param name ="userName">the discord user name of a player</param>
        public Player Find(string userName)
        {
            foreach (Player player in players)
            {
                if ((player.User.Nickname ?? player.User.Username).ToLower() == userName.ToLower()) return player;
            }

            return null;
        }
        /// <summary>Finds and returns a player by user instance</summary>
        /// <param name ="user">the discord user</param>
        public Player Find(IGuildUser user)
        {
            foreach (Player player in players)
            {
                if (player.User == user) return player;
            }

            return null;
        }


        /// <summary>Checks if a player is in the game</summary>
        /// <param name ="Player">the instance of Player to check</param>
        public bool inGame(Player player)
        {
            foreach (Player item in players)
            {
                if (item == player) return true;
            }
            
            return false;
        }

        public bool inGame(ulong id)
        {
            foreach (Player item in players)
            {
                if (item.User.Id == id) return true;
            }

            return false;
        }
        /// <summary>Checks if a player is in the game</summary>
        /// <param name ="Player">the instance of User to check</param>
        public bool inGame(IGuildUser player)
        {
            foreach (Player item in players)
            {
                if (item.User == player) return true;
            }

            return false;
        }
        
        public IGuildChannel GameChat
        {
            get
            {
                return gameChat;
            }
        }
        public IGuildChannel MafiaChat
        {
            get
            {
                return mafiaChat;
            }
        }

        public Player MafiaKillTarget
        {
            get
            {
                return mafiaKillTarget;
            }

            set
            {
                mafiaKillTarget = value;
            }
        }

        public int TownPlayers
        {
            get
            {
                return townPlayers;
            }

            set
            {
                townPlayers = value;
            }
        }

        public int MafiaPlayers
        {
            get
            {
                return mafiaPlayers;
            }

            set
            {
                mafiaPlayers = value;
            }
        }

        public int TownAlive
        {
            get
            {
                return townAlive;
            }

            set
            {
                townAlive = value;
            }
        }

        public int MafiaAlive
        {
            get
            {
                return mafiaAlive;
            }

            set
            {
                mafiaAlive = value;
            }
        }

        public int Cops
        {
            get
            {
                return cops;
            }

            set
            {
                cops = value;
            }
        }

        public Func<SocketMessage, Task> NightkillHandler
        {
            get
            {
                return nightkillHandler;
            }

            set
            {
                nightkillHandler = value;
            }
        }

        public int Doctors { get; internal set; }
        public int Godfathers { get; internal set; }
    }
}
