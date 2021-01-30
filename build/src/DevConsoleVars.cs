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

        public static int ReadNextVariable(string s, ref int startIndex)
        {
            int index = startIndex;
            if (TextParser.ReadNextBrace(s, ref index) == "{")
            {
                startIndex = index;
                TextParser.ReadNextBrace(s, ref index);
            }
            if (index >= s.Length || s[startIndex] != '{')
                return startIndex;
            return index + 1;
        }

        public static List<string> GetAllVariables(string s)
        {
            List<string> variables = new List<string>();

            int startIndex = 0, endIndex = 0;
            while (endIndex + 1 < s.Length)
            {
                endIndex = ReadNextVariable(s, ref startIndex);
                if (startIndex >= endIndex)
                    break;
                variables.Add(s.Substring(startIndex, endIndex - startIndex));
                startIndex = endIndex;
            }

            return variables;
        }

        static void Prefix(ref string command)
        {
            if (!enabled)
                return;

            string oldCommand = command;
            command = Regex.Replace(command, @"{([al]?)p(\d)}", delegate(Match match)
            {
                List<Profile> profiles = Profiles.all.ToList();
                if (match.Groups[1].Value == "a")
                    profiles = Profiles.activeNonSpectators;
                else if (match.Groups[1].Value == "l")
                    profiles = Profiles.activeNonSpectators.Where(x => x.localPlayer).ToList();

                int profileIndex = int.Parse(match.Groups[2].Value);
                if (profileIndex <= 0 || profileIndex > profiles.Count)
                    return match.Value;
                return profiles[profileIndex - 1].name;
            });
        }
    }
}
