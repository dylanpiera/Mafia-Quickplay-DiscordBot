using Discord;
using DiscordBot.Roles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBot.Game
{
    class Player
    {
        private User user;
        private Player lynchTarget;
        private int votesOn;
        private MafiaRole role;
        private bool ready;
        private bool alive;


        public Player(User user)
        {
            this.user = user;
            this.ready = false;
            this.lynchTarget = null;
            this.Alive = true;
        }

        public void AssignRole(MafiaRole role)
        {
            this.role = role;
        }

        public bool Ready
        {
            get
            {
                return ready;
            }
            set
            {
                ready = value;
            }
        }

        public bool readyUp(GamePlayerList list)
        {   
            ready = true;
            foreach (var item in list.Objects)
            {
                if (!item.Ready) return false;
            }
            return true;  
        }

        public User User
        {
            get
            {
                return user;
            }
        }
        public MafiaRole Role
        {
            get
            {
                return role;
            }
        }
        public Player LynchTarget
        {
            get
            {
                return lynchTarget;
            }

            set
            {
                lynchTarget = value;
            }
        }

        public int VotesOn
        {
            get
            {
                return votesOn;
            }

            set
            {
                votesOn = value;
            }
        }

        public bool Alive
        {
            get
            {
                return alive;
            }

            set
            {
                alive = value;
            }
        }
    }
}
