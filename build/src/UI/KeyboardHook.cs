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
        // Enables repeating feature (Keyboard.repeat) for Azxc
        public static bool repeat;

        public static void HookAndToggle(bool toggle)
        {
            enabled = toggle;
            if (!hooked)
            {
                Azxc.GetCore().GetHarmony().Patch(typeof(Keyboard).GetMethod("Pressed"),
                    prefix: new HarmonyMethod(typeof(KeyboardHook), "PressedPrefix"));
                Azxc.GetCore().GetHarmony().Patch(typeof(Keyboard).GetMethod("Down"),
                    prefix: new HarmonyMethod(typeof(KeyboardHook), "DownPrefix"));

                Azxc.GetCore().GetHarmony().Patch(typeof(Keyboard).GetMethod("MapDown"),
                    prefix: new HarmonyMethod(typeof(KeyboardHook), "MapDownPrefix"));
                Azxc.GetCore().GetHarmony().Patch(typeof(Keyboard).GetMethod("MapPressed"),
                    prefix: new HarmonyMethod(typeof(KeyboardHook), "MapPressedPrefix"));
                Azxc.GetCore().GetHarmony().Patch(typeof(Keyboard).GetMethod("MapReleased"),
                    prefix: new HarmonyMethod(typeof(KeyboardHook), "MapReleasedPrefix"));
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

        // MapDown@Keyboard
        static bool MapDownPrefix(ref bool __result, int mapping)
        {
            if (repeat)
            {
                bool ignoreCore = (bool)AccessTools.Field(typeof(Keyboard), "ignoreCore").GetValue(null);
                __result = (ignoreCore || (!DevConsole.open && !DuckNetwork.enteringText &&
                    !Editor.enteringText && !repeat)) && Keyboard.Down((Keys)mapping);
                return false;
            }
            return true;
        }

        // MapPressed@Keyboard
        static bool MapPressedPrefix(ref bool __result, int mapping, bool any)
        {
            if (repeat)
            {
                bool ignoreCore = (bool)AccessTools.Field(typeof(Keyboard), "ignoreCore").GetValue(null);
                List<Keys> repeatList = (List<Keys>)AccessTools.Field(typeof(Keyboard), "_repeatList").GetValue(null);
                __result = (ignoreCore || (!DevConsole.open && !DuckNetwork.enteringText &&
                    !Editor.enteringText && !repeat)) &&
                    (Keyboard.Pressed((Keys)mapping, any) ||
                    repeatList.Contains((Keys)mapping));
                return false;
            }
            return true;
        }

        // MapReleased@Keyboard
        static bool MapReleasedPrefix(ref bool __result, int mapping)
        {
            if (repeat)
            {
                bool ignoreCore = (bool)AccessTools.Field(typeof(Keyboard), "ignoreCore").GetValue(null);
                __result = (ignoreCore || (!DevConsole.open && !DuckNetwork.enteringText &&
                    !Editor.enteringText && !repeat)) &&
                    Keyboard.Released((Keys)mapping);
                return false;
            }
            return true;
        }
    }
}
