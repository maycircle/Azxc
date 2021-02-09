using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Reflection;
using System.Reflection.Emit;

using Harmony;
using DuckGame;

namespace Azxc.Patches
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
            // if (Level.current == null || Level.current.things == null || Level.current.things.Count == 0)
            //     return 0.0f;
            float furthestThing = Math.Abs(Level.current.things.Aggregate((furthest, next) =>
                Math.Abs(next.x) > Math.Abs(furthest.x) ? next : furthest).x);
            return Level.current.camera.width + furthestThing;
        }

        // .ctor@Bullet
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions,
            ILGenerator generator)
        {
            FieldInfo enabled = AccessTools.Field(typeof(RangeHack), "enabled");
            MethodInfo getRange = AccessTools.Method(typeof(RangeHack), "GetRange");
            FieldInfo range = AccessTools.Field(typeof(Bullet), "range");
            FieldInfo tracer = AccessTools.Field(typeof(Bullet), "_tracer");

            List<CodeInstruction> codes = new List<CodeInstruction>(instructions);

            bool foundDistance = false;
            CodeInstruction brs = null;
            for (int i = 0; i < codes.Count; i++)
            {
                CodeInstruction instruction = codes[i];
                yield return instruction;

                if (instruction.opcode == OpCodes.Stfld && (FieldInfo)instruction.operand == tracer)
                {
                    Label label1 = generator.DefineLabel();
                    yield return new CodeInstruction(OpCodes.Ldsfld, enabled);
                    yield return new CodeInstruction(OpCodes.Brfalse_S, label1);
                    yield return new CodeInstruction(OpCodes.Ldarg_0);
                    yield return new CodeInstruction(OpCodes.Call, getRange);
                    yield return new CodeInstruction(OpCodes.Stfld, range);
                    brs = new CodeInstruction(OpCodes.Br_S, generator.DefineLabel());
                    yield return brs;

                    codes[i + 1].labels.Add(label1);
                }
                else if (instruction.opcode == OpCodes.Ldarg_S && brs != null && !foundDistance)
                {
                    instruction.labels.Add((Label)brs.operand);
                    foundDistance = true;
                }
            }
        }
    }
}
