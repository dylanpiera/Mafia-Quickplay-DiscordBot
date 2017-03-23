using Discord;
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
        protected String title;
        protected String description; //Including explanation on how to use power
        protected String rolePM = "";

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

        public MafiaRole(String title = "vanilla", String description = "The vanilla towny with no actual powers.", Allignment allignment = Allignment.Town, Wincon wincon = Wincon.DefeatMafia)
        {
            this.allignment = allignment;
            this.wincon = wincon;
            this.title = title;
            this.description = description;

            canVote = true; canVoteNow = false;
        }

        public virtual void Power() { }
        public virtual async void sendRolePM(User user)
        {
            await user.CreatePMChannel();
            await user.SendMessage(rolePM);
        }
    }
}
