using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Reflection;
using System.Reflection.Emit;

using Harmony;
using DuckGame;

using Azxc.Hacks.Scanning;

namespace Azxc.Hacks
{
    internal static class InfiniteAmmo
    {
        public static bool hooked, enabled;

        public static void HookAndToggle(bool toggle)
        {
            enabled = toggle;
            if (!hooked)
            {
                Azxc.core.harmony.Patch(typeof(Gun).GetMethod("Reload"),
                    transpiler: new HarmonyMethod(typeof(InfiniteAmmo), "Transpiler"));
                hooked = true;
            }
        }

        // Reload@Gun
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            FieldInfo enabled = AccessTools.Field(typeof(InfiniteAmmo), "enabled");

            List<CodeInstruction> codes = new List<CodeInstruction>(instructions);

            Pattern callvirtPattern = new Pattern(codes);
            callvirtPattern.AddInstructions(new string[]
            {
                "call Void PopShell(Boolean)",
                "ldarg.0 NULL [Label2]",
                "ldarg.0 NULL"
            });
            int callvirt = callvirtPattern.Search()[0].Item1;

            CodeInstruction ldsfld = new CodeInstruction(OpCodes.Ldsfld, enabled);
            CodeInstruction brtrue = new CodeInstruction(OpCodes.Brtrue);
            codes.Insert(callvirt + 1, brtrue);
            codes.Insert(callvirt + 1, ldsfld);

            Pattern stfldPattern = new Pattern(codes);
            stfldPattern.AddInstructions(new string[]
            {
                "stfld Int32 ammo",
                "ldarg.0 NULL [Label1]",
                "ldc.i4.1 NULL"
            });
            int stfld = stfldPattern.Search()[0].Item1;

            Label label = generator.DefineLabel();
            brtrue.operand = label;
            codes[stfld + 1].labels.Add(label);

            return codes.AsEnumerable();
        }
    }
}
