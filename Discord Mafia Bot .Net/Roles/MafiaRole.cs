using Discord;
using Discord.WebSocket;
using DiscordBot.Game;
using DiscordBot.Roles.RoleUtil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBot.Roles
{
    public class MafiaRole
    {
        protected Allignment allignment;
        protected Wincon wincon;
        private String title;
        protected String description; //Including explanation on how to use power
        protected String rolePM = "";
        protected String power = "";
        private bool powerRole = false;
        private Player target;
        private Func<SocketMessage,Task> eventHandler;

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

        public Func<SocketMessage,Task> PowerHandler(GamePlayerList g)
        {
            if (this.eventHandler == null)
                this.eventHandler = new Func<SocketMessage, Task>((s) => powerHandler(s, g));
            return this.eventHandler;
        }

        public MafiaRole(String title = "vanilla", String description = "The vanilla towny with no actual powers.", Allignment allignment = Allignment.Town, Wincon wincon = Wincon.DefeatMafia, string rolePM = "")
        {
            this.allignment = allignment;
            this.wincon = wincon;
            this.Title = title;
            this.description = description;
            this.rolePM = rolePM;

            canVote = true; canVoteNow = false;
        }

        
        #pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        protected virtual async Task powerHandler(SocketMessage e, GamePlayerList g) { }
        public virtual async Task<bool> powerResult(IGuildUser user, Player target) { return true; }
        #pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously

        public virtual async void sendRolePM(IGuildUser user)
        {
            IDMChannel channel = await user.GetOrCreateDMChannelAsync();
            await channel.SendMessageAsync(rolePM);
        }
    }
}
