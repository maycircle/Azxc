using DuckGame;
using Harmony;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace Azxc.Patches
{
    internal static class InfiniteAmmo
    {
        public static bool hooked, enabled;

        public static void HookAndToggle(bool toggle)
        {
            enabled = toggle;
            if (!hooked)
            {
                Azxc.GetCore().GetHarmony().Patch(typeof(Gun).GetMethod("Reload"),
                    transpiler: new HarmonyMethod(typeof(InfiniteAmmo), "Transpiler"));
                hooked = true;
            }
        }

        // Reload@Gun
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions,
            ILGenerator generator)
        {
            FieldInfo enabled = AccessTools.Field(typeof(InfiniteAmmo), "enabled");
            MethodInfo popShell = AccessTools.Method(typeof(Gun), "PopShell");
            FieldInfo ammo = AccessTools.Field(typeof(Gun), "ammo");

            List<CodeInstruction> codes = instructions.ToList();

            Label label1 = generator.DefineLabel();
            for (int i = 0; i < codes.Count; i++)
            {
                CodeInstruction instruction = codes[i];
                yield return instruction;

                if (instruction.opcode == OpCodes.Call && (MethodInfo)instruction.operand == popShell)
                {
                    yield return new CodeInstruction(OpCodes.Ldsfld, enabled);
                    yield return new CodeInstruction(OpCodes.Brtrue, label1);
                }
                else if (instruction.opcode == OpCodes.Stfld && (FieldInfo)instruction.operand == ammo)
                {
                    codes[i + 1].labels.Add(label1);
                }
            }
        }
    }
}
