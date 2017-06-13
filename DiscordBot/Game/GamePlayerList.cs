using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using DiscordBot.Util;
using System.Threading;


namespace DiscordBot.Game
{
    class GamePlayerList
    {
        private int PhaseLength, phaseCounter, townPlayers, mafiaPlayers, townAlive, mafiaAlive, cops;


        public int PhaseCounter {
            get {
                return phaseCounter;
            }
            set {
                phaseCounter = value;
            }
        }

        public int PhaseLengthInMin {
            get {
                return PhaseLength;
            }
            set {
                PhaseLength = value;
            }
        }

        public List<Player> Objects {
            get {
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

        public Phases Phase {
            get {
                return phase;
            }
            set {
                phase = value;
            }
        }
        public CancellationTokenSource Token {
            get {
                return token;
            }
            set {
                token = value;
            }
        }

        private List<Player> players;
        private List<Spectator> spectators;

        private Channel gameChat, mafiaChat;

        public void setChats(Channel gameChat, Channel mafiaChat) {
            this.gameChat = gameChat;
            this.mafiaChat = mafiaChat;
        }

        // <summary>Sets or gets if the game is running or not</summary>
        public bool gameRunning { get; set; }

        internal void Reset() {
            this.gameRunning = false;
            this.Objects.Clear();
            this.GameChat.Delete();
            this.MafiaChat.Delete();
        }

        public GamePlayerList() {
            players = new List<Player>();
            gameRunning = false;
            PhaseLengthInMin = 2;
        }

        // <summary> Add's a User to the game, and assigns them to an instance of Player</summary>
        // <param name ="Player">the user to add to the game as a Player</param>
        public void Add(User player)
        {
            this.players.Add(new Player(player,this));

        }

        public void AddSpectate(User spectator) {
            this.spectators.Add(new Spectator(spectator));
        }

        // <summary> removes a User from the game</summary>
        // <param name ="Player">the instance of Player to remove from the list</param>
        public void Remove(Player player) {
            this.players.Remove(player);
        }
        // <summary> removes a User from the game with an instance of User</summary>
        // <param name ="Player">the instance of User to remove from the list</param>
        public void Remove(User player) {
            foreach(Player item in players) {
                if(item.User.Equals(player)) {
                    players.Remove(item);
                    break;
                }
            }
        }

        // <summary>Finds and returns a player by name</summary>
        // <param name ="userName">the discord user name of a player</param>
        public Player Find(string userName)
        {
            foreach (Player player in players)
            {
                if (player.User.Name == userName || player.User.Nickname == userName) return player;

            }

            return null;
        }
        // <summary>Finds and returns a player by user instance</summary>
        // <param name ="user">the discord user</param>
        public Player Find(User user) {
            foreach(Player player in players) {
                if(player.User == user)
                    return player;
            }

            return null;
        }


        // <summary>Checks if a player is in the game</summary>
        // <param name ="Player">the instance of Player to check</param>
        public bool inGame(Player player) {
            foreach(Player item in players) {
                if(item == player)
                    return true;
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
        // <summary>Checks if a player is in the game</summary>
        // <param name ="Player">the instance of User to check</param>
        public bool inGame(User player) {
            foreach(Player item in players) {
                if(item.User == player)
                    return true;
            }

            return false;
        }

        public bool isSpectating(User spectator) {
            foreach(Spectator item in spectators) {
                if(item.User == spectator)
                    return true;
            }

            return false;
        }

        public Channel GameChat {
            get {
                return gameChat;
            }
        }
        public Channel MafiaChat {
            get {
                return mafiaChat;
            }
        }

        public Player MafiaKillTarget {
            get {
                return mafiaKillTarget;
            }

            set {
                mafiaKillTarget = value;
            }
        }

        public int TownPlayers {
            get {
                return townPlayers;
            }

            set {
                townPlayers = value;
            }
        }

        public int MafiaPlayers {
            get {
                return mafiaPlayers;
            }

            set {
                mafiaPlayers = value;
            }
        }

        public int TownAlive {
            get {
                return townAlive;
            }

            set {
                townAlive = value;
            }
        }

        public int MafiaAlive {
            get {
                return mafiaAlive;
            }

            set {
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

        public EventHandler<MessageEventArgs> NightkillHandler
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