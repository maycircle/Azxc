using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

using Harmony;
using DuckGame;

namespace Azxc
{
    public class DevConsoleVars
    {
        public static bool hooked, enabled;

        public static void HookAndToggle(bool toggle)
        {
            enabled = toggle;
            if (!hooked)
            {
                Azxc.core.harmony.Patch(typeof(DevConsole).GetMethod("RunCommand"),
                    prefix: new HarmonyMethod(typeof(DevConsoleVars), "Prefix"));
                hooked = true;
            }
        }

        static void Prefix(ref string command)
        {
            if (!enabled)
                return;

            string oldCommand = command;
            command = Regex.Replace(command, @"\[([al]?)p(\d)(:\w+)?\]", delegate(Match match)
            {
                List<Profile> profiles = Profiles.all.ToList();
                if (match.Groups[1].Value == "a")
                    profiles = Profiles.activeNonSpectators;
                else if (match.Groups[1].Value == "l")
                    profiles = Profiles.activeNonSpectators.Where(x => x.localPlayer).ToList();

                int profileIndex = int.Parse(match.Groups[2].Value);
                if (profileIndex <= 0 || profileIndex > profiles.Count)
                    return match.Value;
                if (!match.Groups[3].Success)
                    return profiles[profileIndex - 1].name;
                else
                {
                    string propertyName = match.Groups[3].Value.Substring(1);
                    if (AccessTools.GetPropertyNames(typeof(Profile)).Contains(propertyName))
                    {
                        return AccessTools.Property(typeof(Profile), propertyName)
                            .GetValue(profiles[profileIndex - 1]).ToString();
                    }
                    else if (AccessTools.GetPropertyNames(typeof(Duck)).Contains(propertyName) &&
                        profiles[profileIndex - 1].duck != null)
                    {
                        return AccessTools.Property(typeof(Duck), propertyName)
                            .GetValue(profiles[profileIndex - 1].duck).ToString();
                    }
                    else
                        return match.Value;
                }
            });
        }
    }
}
