using Discord;
using System.Linq;

namespace DiscordBot.Commands
{
    public class Setup
    {

        private string setupName, creatorName;
        private ulong creatorID;
        private string lynchTieHandler, majorityType;
        private int dayLength, nightLength;

        public string SetupName
        {
            get
            {
                return setupName;
            }

            set
            {
                setupName = new string(value.Where(c => char.IsLetterOrDigit(c) || char.IsWhiteSpace(c) || c == '-').ToArray()); 
            }
        }

        public User Creator
        {
            set
            {
                creatorName = value.Mention;
                creatorID = value.Id;
            }
        }
        public string CreatorName
        {
            get
            {
                return creatorName;
            }
            set
            {
                creatorName = value;
            }
        }

        public ulong CreatorID
        {
            get
            {
                return creatorID;
            }

            set
            {
                creatorID = value;
            }
        }

        public string LynchTieHandler
        {
            get
            {
                return lynchTieHandler;
            }

            set
            {
                lynchTieHandler = value;
            }
        }

        public string MajorityType
        {
            get
            {
                return majorityType;
            }

            set
            {
                majorityType = value;
            }
        }

        public int DayLength
        {
            get
            {
                return dayLength;
            }

            set
            {
                dayLength = value;
            }
        }

        public int NightLength
        {
            get
            {
                return nightLength;
            }

            set
            {
                nightLength = value;
            }
        }

        public Setup()
        {
        }
    }
}