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

using Azxc.Patches.Misc;

namespace Azxc.Patches
{
    internal static class HatStealer
    {
        public static bool hooked, enabled, autoConvert;
        public static string savePath;

        public static void HookAndToggle(bool toggle, string savePath = null)
        {
            enabled = toggle;
            if (savePath != null)
                HatStealer.savePath = savePath;
            else
                CheckSaveFolder();
            if (toggle && DuckNetwork.active)
            {
                foreach (Profile profile in Profiles.active)
                {
                    // Saves only if profile is wearing a custom hat
                    if (!profile.localPlayer && profile.team.hasHat &&
                        profile.customTeams.Contains(profile.team))
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

        public static string DefaultHatStealerSavePath()
        {
            string savePath = ModLoader.GetMod<Azxc>().configuration.directory + "/HatStealer";
            if (!Directory.Exists(savePath))
                Directory.CreateDirectory(savePath);
            Azxc.core.config.TrySet("HatStealerSavePath", savePath);
            return savePath;
        }

        public static void CheckSaveFolder()
        {
            string savePath = Azxc.core.config.TryGetSingle("HatStealerSavePath", "");
            if (savePath == "" || !Directory.Exists(savePath))
                HatStealer.savePath = DefaultHatStealerSavePath();
            else
                HatStealer.savePath = savePath;
        }

        private static void SaveCustomTeam(Team customTeam)
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

        public static void SaveCustomTeam(Team customTeam, out string output)
        {
            Texture2D texture = customTeam.hat.texture.nativeObject as Texture2D;

            string pngFile = savePath + "/" + customTeam.name + ".png";
            using (FileStream file = new FileStream(pngFile, FileMode.Create))
                texture.SaveAsPng(file, texture.Width, texture.Height);

            if (autoConvert)
            {
                string hatFile = savePath + "/" + customTeam.name + ".hat";
                HatConverter.ExportFromPNG(pngFile, customTeam.name, hatFile);
            }

            output = customTeam.name + ".png";
        }

        // OnMessage@DuckNetwork
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            MethodInfo saveCustomTeam = AccessTools.Method(typeof(HatStealer), "SaveCustomTeam",
                new Type[] { typeof(Team) });
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
