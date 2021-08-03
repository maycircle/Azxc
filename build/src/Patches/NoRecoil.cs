
using DuckGame;
using Harmony;

namespace Azxc.Patches
{
    internal static class NoRecoil
    {
        public static bool hooked, enabled;

        public static void HookAndToggle(bool toggle)
        {
            enabled = toggle;
            if (!hooked)
            {
                Azxc.GetCore().GetHarmony().Patch(typeof(Gun).GetMethod("ApplyKick"),
                    prefix: new HarmonyMethod(typeof(NoRecoil), "Prefix"));
                hooked = true;
            }
        }

        // ApplyKick@Gun
        static bool Prefix()
        {
            if (enabled)
                return false;
            return true;
        }
    }
}
