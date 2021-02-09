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
    internal static class BulletHit
    {
        public static bool hooked, enabled;
        public static Type trigger = typeof(PhysicsObject);

        public static void HookAndToggle(bool toggle)
        {
            enabled = toggle;
            if (!hooked)
            {
                Azxc.core.harmony.Patch(AccessTools.Method(typeof(Bullet), "RaycastBullet"),
                    transpiler: new HarmonyMethod(typeof(BulletHit), "Transpiler"));
                hooked = true;
            }
        }

        static bool HitExactThing(MaterialThing thing, Bullet bullet, Vec2 hitPos)
        {
            Type thingType = thing.GetType();
            if (enabled && (thingType == trigger || thingType.IsSubclassOf(trigger)))
                return thing.DoHit(bullet, hitPos);
            else if (!enabled)
                return thing.DoHit(bullet, hitPos);
            return false;
        }

        // RaycastBullet@Bullet
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions,
            ILGenerator generator)
        {
            MethodInfo doHit = AccessTools.Method(typeof(MaterialThing), nameof(MaterialThing.DoHit));

            foreach (CodeInstruction instruction in instructions)
            {
                if (instruction.opcode == OpCodes.Callvirt && (MethodInfo)instruction.operand == doHit)
                    yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(BulletHit),
                        nameof(BulletHit.HitExactThing)));
                else
                    yield return instruction;
            }
        }
    }
}
