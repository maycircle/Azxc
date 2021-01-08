using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Harmony;
using DuckGame;

namespace Azxc.Hacks
{
    internal static class PauseTimer
    {
        public static bool hooked, enabled;

        public static void Hook()
        {
            if (!hooked)
            {
                Azxc.core.harmony.Patch(typeof(ChallengeLevel).GetMethod("Update"),
                    prefix: new HarmonyMethod(typeof(PauseTimer), "Prefix"));
                hooked = true;
            }
        }

        // Update@ChallengeLevel
        static bool Prefix()
        {
            if (ChallengeLevel.timer != null && enabled)
                ChallengeLevel.timer.Stop();
            return true;
        }
    }
}
