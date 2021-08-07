using Microsoft.VisualBasic.CompilerServices;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent.Bestiary;

namespace JBMiner
{
    // A utility class.
    public struct TimeSince
    {
        private float _time;

        public static implicit operator TimeSince(float f)
        {
            
            return new(){_time = (float) Main.time-f};
        }
        
        public static implicit operator float(TimeSince t)
        {
            return ((float)Main.time - t._time) / 60;
        }
    }
}