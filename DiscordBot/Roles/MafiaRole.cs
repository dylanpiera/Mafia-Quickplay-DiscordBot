using Discord;
using DiscordBot.Game;
using DiscordBot.Roles.RoleUtil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBot.Roles {
    class MafiaRole {
        protected Allignment allignment;
        protected bool copCorrect;
        protected Wincon wincon;
        private String title;
        protected String description; //Including explanation on how to use power
        protected String rolePM = "";
        private Player target;

        private bool canVote, canVoteNow;

        public bool CanVote {
            get {
                return canVoteNow;
            }

            set {
                if(canVote)
                    canVoteNow = value;
            }
        }

        public Allignment Allignment {
            get {
                return allignment;
            }
        }

        public bool CopCorrect {
            get {
                return copCorrect;
            }
        }

        public string RolePM {
            get {
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

        public MafiaRole(String title = "vanilla", String description = "The vanilla towny with no actual powers.", Allignment allignment = Allignment.Town, Wincon wincon = Wincon.DefeatMafia, string rolePM = "")
        {
            this.allignment = allignment;
            this.copCorrect = copCorrect;
            this.wincon = wincon;
            this.Title = title;
            this.description = description;
            this.rolePM = rolePM;

            canVote = true;
            canVoteNow = false;
        }
        
        public async virtual Task<bool> Power(Channel chat) { await Task.Delay(0); return true; }
        public virtual async void sendRolePM(User user)
        {

            await user.CreatePMChannel();
            await user.SendMessage(rolePM);
        }
    }
}