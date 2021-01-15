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
    internal static class RangeHack
    {
        public static bool hooked, enabled;

        public static void HookAndToggle(bool toggle)
        {
            enabled = toggle;
            if (!hooked)
            {
                Azxc.core.harmony.Patch(AccessTools.Constructor(typeof(Bullet),
                    new Type[] { typeof(float), typeof(float), typeof(AmmoType), typeof(float),
                        typeof(Thing), typeof(bool), typeof(float), typeof(bool), typeof(bool) }),
                    transpiler: new HarmonyMethod(typeof(RangeHack), "Transpiler"));
                hooked = true;
            }
        }

        static float GetRange()
        {
            float furthestThing = Level.current.things.Aggregate((furthest, next) =>
                next.x > furthest.x ? next : furthest).x;
            return Level.current.camera.width + furthestThing;
        }

        // .ctor@Bullet
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions,
            ILGenerator generator)
        {
            FieldInfo enabled = AccessTools.Field(typeof(RangeHack), "enabled");
            MethodInfo getRange = AccessTools.Method(typeof(RangeHack), "GetRange");
            FieldInfo range = AccessTools.Field(typeof(Bullet), "range");

            List<CodeInstruction> codes = new List<CodeInstruction>(instructions);
            Pattern ldargPattern = new Pattern(codes);
            ldargPattern.AddInstructions(new string[]
            {
                "ldarg.0 NULL",
                "ldarg.3 NULL",
                "ldfld Single range",
                "ldarg.3 NULL",
                "ldfld Single rangeVariation",
                "call Single Float(Single)",
                "sub NULL",
                "stfld Single range"
            });
            Tuple<int, int> ldarg = ldargPattern.Search()[0];

            Label label1 = generator.DefineLabel();
            Label label2 = generator.DefineLabel();
            codes[ldarg.Item1].labels.Add(label1);
            codes[ldarg.Item2 + 1].labels.Add(label2);
            codes.Insert(ldarg.Item1, new CodeInstruction(OpCodes.Br_S, label2));
            codes.Insert(ldarg.Item1, new CodeInstruction(OpCodes.Stfld, range));
            codes.Insert(ldarg.Item1, new CodeInstruction(OpCodes.Call, getRange));
            codes.Insert(ldarg.Item1, new CodeInstruction(OpCodes.Ldarg_0));
            codes.Insert(ldarg.Item1, new CodeInstruction(OpCodes.Brfalse_S, label1));
            codes.Insert(ldarg.Item1, new CodeInstruction(OpCodes.Ldsfld, enabled));

            return codes.AsEnumerable();
        }
    }
}
