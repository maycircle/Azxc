using DuckGame;
using Harmony;

namespace Azxc.Patches
{
    internal static class PauseTimer
    {
        public static bool hooked, enabled;

        public static void Hook()
        {
            if (!hooked)
            {
                Azxc.GetCore().GetHarmony().Patch(typeof(ChallengeLevel).GetMethod("Update"),
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
