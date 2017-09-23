using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord_Mafia_Bot.Util
{
    [AttributeUsage(AttributeTargets.Method)]
    class HiddenAttribute : Attribute
    {
        public bool Hidden { get; }

        public HiddenAttribute()
        {
            Hidden = true;
        }
    }
}
