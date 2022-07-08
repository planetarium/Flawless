using System;
using System.Collections.Generic;
using System.Linq;

namespace Flawless
{
    public static class Utils
    {
        public static int Random(int upper, long seed, long salt)
        {
            Random random = new Random((int)(seed + salt));
            return random.Next(upper);
        }

        public static int Random(long seed, long salt = 0)
        {
            Random random = new Random((int)(seed + salt));
            return random.Next();
        }
    }
}
