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

using Azxc.Hacks.Misc;

namespace Azxc.Hacks
{
    internal static class HatStealer
    {
        public static bool hooked, enabled, autoConvert;
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

            string pngFile = savePath + "/" + customTeam.name + ".png";
            using (FileStream file = new FileStream(pngFile, FileMode.Create))
                texture.SaveAsPng(file, texture.Width, texture.Height);
            
            if (autoConvert)
            {
                string hatFile = savePath + "/" + customTeam.name + ".hat";
                HatConverter.ExportFromPNG(pngFile, customTeam.name, hatFile);
            }
        }

        // OnMessage@DuckNetwork
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            MethodInfo saveCustomTeam = AccessTools.Method(typeof(HatStealer), "SaveCustomTeam");
            MethodInfo teamDeserialize = AccessTools.Method(typeof(Team),
                "Deserialize", new Type[] { typeof(byte[]) });
            FieldInfo customConnection = AccessTools.Field(typeof(Team), "customConnection");

            foreach (CodeInstruction instruction in instructions)
            {
                yield return instruction;

                if (instruction.opcode == OpCodes.Call &&
                    (MethodInfo)instruction.operand == teamDeserialize)
                    yield return new CodeInstruction(OpCodes.Dup);
                else if (instruction.opcode == OpCodes.Stfld &&
                    (FieldInfo)instruction.operand == customConnection)
                    yield return new CodeInstruction(OpCodes.Call, saveCustomTeam);
            }
        }
    }
}
