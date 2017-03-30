using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBot.Util
{
    static class TimeConverter
    {
        public static int MinToMS(double min)
        {
            return (int)(min * 1000 * 60);
        }
        public static int MSToMin(double ms)
        {
            return (int)((ms / 1000) / 60);
        }

        public static int SecToMS(double sec)
        {
            return (int)(sec * 1000);
        }
        public static int MSToSec(double ms)
        {
            return (int)(ms / 1000);
        }
    }
}