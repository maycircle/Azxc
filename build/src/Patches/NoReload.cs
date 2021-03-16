using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Reflection;
using System.Reflection.Emit;

using Harmony;
using DuckGame;

namespace Azxc.Patches
{
    internal static class NoReload
    {
        public static bool hooked, enabled;

        public static void HookAndToggle(bool toggle)
        {
            enabled = toggle;
            if (!hooked)
            {
                Azxc.GetCore().GetHarmony().Patch(typeof(Gun).GetMethod("Fire"),
                    postfix: new HarmonyMethod(typeof(NoReload), "FirePostfix"));
                Azxc.GetCore().GetHarmony().Patch(typeof(Gun).GetMethod("OnHoldAction"),
                    prefix: new HarmonyMethod(typeof(NoReload), "OnHoldActionPrefix"));
                hooked = true;
            }
        }

        // Fire@Gun
        static void FirePostfix(Gun __instance)
        {
            __instance.loaded = __instance.loaded | enabled;
        }

        // OnHoldAction@Gun
        static bool OnHoldActionPrefix(Gun __instance)
        {
            if (enabled)
            {
                __instance.Fire();
                return false;
            }
            return true;
        }
    }
}
