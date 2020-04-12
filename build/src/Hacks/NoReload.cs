using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Reflection;
using System.Reflection.Emit;

using DuckGame;
using Harmony;

using Azxc.Hacks.Scanning;

namespace Azxc.Hacks
{
    internal static class NoReload
    {
        public static bool enabled;

        // Fire
        public static void Postfix(Gun __instance)
        {
            __instance.loaded = __instance.loaded | enabled;
        }

        // OnHoldAction
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            FieldInfo enabled = AccessTools.Field(typeof(NoReload), "enabled");

            List<CodeInstruction> codes = new List<CodeInstruction>(instructions);

            Pattern ldfldPattern = new Pattern(codes);
            ldfldPattern.AddInstructions(new string[]
            {
                "ldfld Boolean _fullAuto",
                "brfalse Label1",
                "ldarg.0 NULL"
            });
            Tuple<int, int> ldfld = ldfldPattern.Search()[0];

            // Disable reload itself
            Label label = generator.DefineLabel();
            codes[ldfld.Item2].labels.Add(label);

            CodeInstruction ldsfld = new CodeInstruction(codes[ldfld.Item1]);
            ldsfld.opcode = OpCodes.Ldsfld;
            ldsfld.operand = enabled;
            CodeInstruction brtrue = new CodeInstruction(codes[ldfld.Item1 + 1]);
            brtrue.opcode = OpCodes.Brtrue;
            brtrue.operand = label;

            codes.Insert(ldfld.Item1 + 1, ldsfld);
            codes.Insert(ldfld.Item1 + 1, brtrue);

            return codes.AsEnumerable();
        }
    }
}