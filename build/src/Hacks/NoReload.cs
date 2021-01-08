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
    internal static class NoReload
    {
        public static bool hooked, enabled;

        public static void HookAndToggle(bool toggle)
        {
            enabled = toggle;
            if (!hooked)
            {
                Azxc.core.harmony.Patch(typeof(Gun).GetMethod("Fire"),
                    postfix: new HarmonyMethod(typeof(NoReload), "FirePostfix"));
                Azxc.core.harmony.Patch(typeof(Gun).GetMethod("OnHoldAction"),
                    transpiler: new HarmonyMethod(typeof(NoReload), "OnHoldActionTranspiler"));
                hooked = true;
            }
        }

        // Fire@Gun
        public static void FirePostfix(Gun __instance)
        {
            __instance.loaded = __instance.loaded | enabled;
        }

        // OnHoldAction@Gun
        static IEnumerable<CodeInstruction> OnHoldActionTranspiler(IEnumerable<CodeInstruction> instructions,
            ILGenerator generator)
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

            CodeInstruction ldsfld = new CodeInstruction(OpCodes.Ldsfld, enabled);
            CodeInstruction brtrue = new CodeInstruction(OpCodes.Brtrue, label);
            brtrue.opcode = OpCodes.Brtrue;
            brtrue.operand = label;

            codes.Insert(ldfld.Item1 + 1, ldsfld);
            codes.Insert(ldfld.Item1 + 1, brtrue);

            return codes.AsEnumerable();
        }
    }
}
