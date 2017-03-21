using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBot.Util
{
    static class TimeConverter
    {
        public static int MinToMS(int min)
        {
            return (min * 1000 * 60);
        }
        public static int MSToMin(int ms)
        {
            return ((ms / 1000) / 60);
        }

        public static int SecToMS(int sec)
        {
            return (sec * 1000);
        }
        public static int MSToSec(int ms)
        {
            return (ms / 1000);
        }
    }
}
