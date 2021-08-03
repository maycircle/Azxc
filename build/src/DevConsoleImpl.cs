using Azxc.Patches;
using DuckGame;
using Harmony;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Azxc
{
    public class DevConsoleImpl
    {
        public static bool hooked, enabled;

        public static void HookAndToggle(bool toggle)
        {
            enabled = toggle;
            if (!hooked)
            {
                InitializeCommands();
                Azxc.GetCore().GetHarmony().Patch(typeof(DevConsole).GetMethod("RunCommand"),
                    prefix: new HarmonyMethod(typeof(DevConsoleImpl), "Prefix"));
                hooked = true;
            }
        }

        private static void InitializeCommands()
        {
            DevConsole.AddCommand(new CMD("azxc_steal", new CMD.Argument[]
            {
                new CProfile("profile")
            }, delegate (CMD cmd)
            {
                Profile profile = cmd.Arg<Profile>("profile");
                if (profile.team != null)
                {
                    HatStealer.CheckSaveFolder();
                    string output = HatStealer.SaveCustomTeam(profile.team);
                    DevConsole.Log("Saved as \"" + output + "\".", Color.Yellow);
                }
                else
                {
                    DevConsole.Log("Profile isn't wearing a hat right now.", Color.Yellow);
                }
            }));
        }

        private static string ReplaceVars(string command)
        {
            return Regex.Replace(command, @"\[([al]?)p(\d)(:\w+)?\]", delegate (Match match)
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

        private static void Prefix(ref string command)
        {
            if (enabled)
                command = ReplaceVars(command);
        }
    }
}
