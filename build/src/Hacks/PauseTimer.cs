using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using DuckGame;

namespace Azxc.Hacks
{
    internal static class PauseTimer
    {
        public static bool enabled;

        // Update@ChallengeLevel
        static bool Prefix()
        {
            if (ChallengeLevel.timer != null && enabled)
                ChallengeLevel.timer.Stop();
            return true;
        }
    }
}
