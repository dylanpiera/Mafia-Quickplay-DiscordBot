using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord_Mafia_Bot.Util
{
    static class Sneaky
    {
        private static string token = "MjU5MDIzNTk3OTQ5OTQzODI5.C7By8g.5vP4l6VJM7O6SC2nL4Lpz73QgQg", boturl = "https://discordapp.com/oauth2/authorize?&client_id=259023597949943829&scope=bot", connectionString = "Server=198.211.125.25;Database=mafiaDB;User id=MafiaEncoded;Password=MEsPassword;";

        public static string botInvite
        {
            get
            {
                return boturl;
            }
        }

        public static string BotToken
        {
            get
            {
                return token;
            }
        }

        public static string ConnectionString
        {
            get
            {
                return connectionString;
            }

            set
            {
                connectionString = value;
            }
        }
    }
}
