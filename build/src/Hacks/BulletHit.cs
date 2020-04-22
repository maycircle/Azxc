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
    internal static class BulletHit
    {
        public static bool enabled;
        public static Type trigger = typeof(PhysicsObject);

        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions,
            ILGenerator generator)
        {
            FieldInfo enabled = AccessTools.Field(typeof(BulletHit), "enabled");
            FieldInfo trigger = AccessTools.Field(typeof(BulletHit), "trigger");

            MethodInfo getType = AccessTools.Method(typeof(object), "GetType");
            MethodInfo isAssignableFrom = AccessTools.Method(typeof(Type), "IsAssignableFrom");

            List<CodeInstruction> codes = new List<CodeInstruction>(instructions);

            Pattern ldlocPattern = new Pattern(codes);
            string[] textPattern = ldlocPattern.AddInstructions(new string[]
            {
                "ldloc.s 7 (DuckGame.MaterialThing)",
                "ldarg.0 NULL",
                "ldloc.1 NULL",
                "callvirt Boolean DoHit(DuckGame.Bullet, DuckGame.Vec2)",
                "stloc.2 NULL"
            });
            Tuple<int, int> ldloc = ldlocPattern.Search()[0];
            List<CodeInstruction> doHit = codes.GetRange(ldloc.Item1, ldloc.Item2 - ldloc.Item1 + 1);

            // The shit begins...
            Label label1 = generator.DefineLabel();
            codes[ldloc.Item2 + 1].labels.Add(label1);
            codes.Insert(ldloc.Item1, new CodeInstruction(OpCodes.Brtrue, label1));
            Label label2 = generator.DefineLabel();
            CodeInstruction ldsfld = new CodeInstruction(OpCodes.Ldsfld, enabled);
            ldsfld.labels.Add(label2);
            codes.Insert(ldloc.Item1, ldsfld);
            codes.Insert(ldloc.Item1, new CodeInstruction(OpCodes.Br_S, label1));
            codes.InsertRange(ldloc.Item1, doHit);
            codes.Insert(ldloc.Item1, new CodeInstruction(OpCodes.Brfalse, label2));
            codes.Insert(ldloc.Item1, new CodeInstruction(OpCodes.Callvirt, isAssignableFrom));
            codes.Insert(ldloc.Item1, new CodeInstruction(OpCodes.Callvirt, getType));
            codes.Insert(ldloc.Item1, doHit[0]);
            codes.Insert(ldloc.Item1, new CodeInstruction(OpCodes.Ldsfld, trigger));
            codes.Insert(ldloc.Item1, new CodeInstruction(OpCodes.Brfalse, label2));
            codes.Insert(ldloc.Item1, new CodeInstruction(OpCodes.Ldsfld, enabled));

            return codes.AsEnumerable();
        }
    }
}