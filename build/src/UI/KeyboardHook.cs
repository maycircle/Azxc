using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;

using Harmony;
using DuckGame;

namespace Azxc.UI
{
    // Hooks and cancels game keyboard input
    internal static class KeyboardHook
    {
        public static bool hooked, enabled;

        public static void HookAndToggle(bool toggle)
        {
            enabled = toggle;
            if (!hooked)
            {
                Azxc.core.harmony.Patch(typeof(Keyboard).GetMethod("Pressed"),
                    prefix: new HarmonyMethod(typeof(KeyboardHook), "PressedPrefix"));
                Azxc.core.harmony.Patch(typeof(Keyboard).GetMethod("Down"),
                    prefix: new HarmonyMethod(typeof(KeyboardHook), "DownPrefix"));
                hooked = true;
            }
        }

        // Down@Keyboard
        static bool DownPrefix(ref bool __result)
        {
            // Mainly hooks player controls, such as WASD, Jump and Escape menu
            if (enabled)
            {
                StackTrace stackTrace = new StackTrace();
                bool extraCall = stackTrace.GetFrames().Skip(1).Any(x => {
                    if (x.GetMethod().DeclaringType != null)
                        return x.GetMethod().DeclaringType.Namespace.Contains("Azxc");
                    return false;
                });

                if (extraCall)
                    return true;

                __result = false;
                return false;
            }
            return true;
        }

        // Pressed@Keyboard
        static bool PressedPrefix(ref bool __result)
        {
            if (enabled)
            {
                StackTrace stackTrace = new StackTrace();
                bool extraCall = stackTrace.GetFrames().Skip(1).Any(x => {
                    if (x.GetMethod().DeclaringType != null)
                        return x.GetMethod().DeclaringType.Namespace.Contains("Azxc") ||
                            // Handle Keyboard.keyString in a special way
                            x.GetMethod().Name == "updateKeyboardString";
                    return false;
                });

                if (extraCall)
                    return true;

                __result = false;
                return false;
            }
            return true;
        }
    }
}
