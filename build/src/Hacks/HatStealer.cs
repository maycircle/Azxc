using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Reflection.Emit;

using Microsoft.Xna.Framework.Graphics;
using Harmony;
using DuckGame;

using Azxc.Hacks.Scanning;

namespace Azxc.Hacks
{
    internal static class HatStealer
    {
        public static bool hooked, enabled;
        public static string savePath;

        public static void HookAndToggle(bool toggle, string savePath)
        {
            enabled = toggle;
            HatStealer.savePath = savePath;
            if (toggle && DuckNetwork.active)
            {
                foreach (Profile profile in Profiles.active)
                {
                    if (!profile.localPlayer)
                        SaveCustomTeam(profile.team);
                }
            }

            if (!hooked)
            {
                Azxc.core.harmony.Patch(AccessTools.Method(typeof(DuckNetwork), "OnMessage"),
                    transpiler: new HarmonyMethod(typeof(HatStealer), "Transpiler"));
                hooked = true;
            }
        }

        static void SaveCustomTeam(Team customTeam)
        {
            if (!enabled || customTeam == null)
                return;
            Texture2D texture = customTeam.hat.texture.nativeObject as Texture2D;

            string hatFile = savePath + "/" + customTeam.name + ".png";
            using (FileStream file = new FileStream(hatFile, FileMode.Create))
                texture.SaveAsPng(file, texture.Width, texture.Height);
        }

        // OnMessage@DuckNetwork
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            MethodInfo saveCustomTeam = AccessTools.Method(typeof(HatStealer), "SaveCustomTeam");

            List<CodeInstruction> codes = new List<CodeInstruction>(instructions);
            Pattern ldargPattern = new Pattern(codes);
            ldargPattern.AddInstructions(new string[]
            {
                "ldloc.s 4 (DuckGame.NMSpecialHat)",
                "callvirt DuckGame.NetworkConnection get_connection()",
                "stfld DuckGame.NetworkConnection customConnection"
            });
            Tuple<int, int> ldarg = ldargPattern.Search()[0];

            codes.Insert(ldarg.Item2 + 1, new CodeInstruction(OpCodes.Call, saveCustomTeam));
            codes.Insert(ldarg.Item1, new CodeInstruction(OpCodes.Dup));

            return codes.AsEnumerable();
        }
    }
}
