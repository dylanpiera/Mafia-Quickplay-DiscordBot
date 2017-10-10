using Discord;
using Discord.WebSocket;
using DiscordBot.Game;
using DiscordBot.Roles.RoleUtil;
using System;
using System.Threading.Tasks;

namespace DiscordBot.Roles
{
    public class MafiaRole : IMafiaRole
    {
        protected Allignment allignment;
        protected Wincon wincon;
        private String title;
        protected String description; //Including explanation on how to use power



        protected String rolePM = "";
        protected String power = "";
        private bool powerRole = false;
        private Player target;
        public Player Player { get; set; }
        private Func<SocketMessage, Task> eventHandler;

        private bool canVote, canVoteNow;

        public bool CanVote
        {
            get
            {
                return canVoteNow;
            }

            set
            {
                if (canVote) canVoteNow = value;
            }
        }

        public Allignment Allignment
        {
            get
            {
                return allignment;
            }
        }

        public string RolePM
        {
            get
            {
                return rolePM;
            }
        }

        public string Title
        {
            get
            {
                return title;
            }

            protected set
            {
                title = value;
            }
        }

        public Player Target
        {
            get
            {
                return target;
            }

            set
            {
                target = value;
            }
        }

        public string Power
        {
            get
            {
                return power;
            }

            set
            {
                power = value;
            }
        }

        public bool PowerRole
        {
            get
            {
                return powerRole;
            }

            set
            {
                powerRole = value;
            }
        }

        public Func<SocketMessage, Task> PowerHandler(GamePlayerList g)
        {
            if (this.eventHandler == null)
                this.eventHandler = new Func<SocketMessage, Task>((s) => PowerHandler(s, g));
            return this.eventHandler;
        }

        protected virtual MafiaRole SetupRole(Player player, string title, string description, Allignment allignment = Allignment.Town, Wincon wincon = Wincon.DefeatMafia, string rolePM = "")
        {
            this.Player = player;
            this.allignment = allignment;
            this.wincon = wincon;
            this.Title = title;
            this.description = description;
            this.rolePM = rolePM;

            canVote = true; canVoteNow = false;
            return this;
        }


#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        protected virtual async Task PowerHandler(SocketMessage e, GamePlayerList g) { }
        public virtual async Task<bool> PowerResult(IGuildUser user, Player target) { return true; }
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously

        public virtual async void SendRolePM(IGuildUser user)
        {
            IDMChannel channel = await user.GetOrCreateDMChannelAsync();
            await channel.SendMessageAsync(rolePM);
        }

        public virtual MafiaRole SetupRole(Player player)
        {
            throw new NotImplementedException();
        }
    }

    public interface IMafiaRole
    {
        MafiaRole SetupRole(Player player);
    }
}
