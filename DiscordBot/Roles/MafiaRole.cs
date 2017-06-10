using Discord;
using DiscordBot.Game;
using DiscordBot.Roles.RoleUtil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBot.Roles
{
    class MafiaRole
    {
        protected Allignment allignment;
        protected Wincon wincon;
        private String title;
        protected String description; //Including explanation on how to use power
        protected String rolePM = "";
        protected String power = "";
        private bool powerRole = false;
        private Player target;

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

        public EventHandler<MessageEventArgs> PowerHandler(GamePlayerList g)
        {
            return new EventHandler<MessageEventArgs>((s, e) => powerHandler(s, e, g));
            
        }

        public MafiaRole(String title = "vanilla", String description = "The vanilla towny with no actual powers.", Allignment allignment = Allignment.Town, Wincon wincon = Wincon.DefeatMafia)
        {
            this.allignment = allignment;
            this.wincon = wincon;
            this.Title = title;
            this.description = description;

            canVote = true; canVoteNow = false;
        }


        //public async virtual Task<bool> Power(Channel chat) { await Task.Delay(0); return true; }
        #pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        protected virtual async void powerHandler(object s, MessageEventArgs e, GamePlayerList g) { }
        public virtual async Task<bool> powerResult(User user, Player target) { return true; }
        #pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously



        public virtual async void sendRolePM(User user)
        {
            await user.CreatePMChannel();
            await user.SendMessage(rolePM);
        }
    }
}
