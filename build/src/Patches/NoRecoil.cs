using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Reflection;
using System.Reflection.Emit;

using Harmony;
using DuckGame;

namespace Azxc.Hacks
{
    internal static class NoRecoil
    {
        public static bool hooked, enabled;

        public static void HookAndToggle(bool toggle)
        {
            enabled = toggle;
            if (!hooked)
            {
                Azxc.core.harmony.Patch(typeof(Gun).GetMethod("ApplyKick"),
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
