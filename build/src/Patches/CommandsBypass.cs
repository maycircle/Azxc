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
    internal static class CommandsBypass
    {
        public static bool hooked, enabled;

        public static void HookAndToggle(bool toggle)
        {
            enabled = toggle;
            if (!hooked)
            {
                Azxc.core.harmony.Patch(typeof(DevConsole).GetMethod("RunCommand"),
                    transpiler: new HarmonyMethod(typeof(CommandsBypass), "Transpiler"));
                Azxc.core.harmony.Patch(AccessTools.Method(typeof(DevConsole), "CheckCheats"),
                    prefix: new HarmonyMethod(typeof(CommandsBypass), "Prefix"));
                hooked = true;
            }
        }

        // CheckCheats@DevConsole
        static bool Prefix()
        {
            if (enabled)
                return false;
            return true;
        }

        // RunCommand@DevConsole
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions,
            ILGenerator generator)
        {
            FieldInfo enabled = AccessTools.Field(typeof(CommandsBypass), "enabled");

            List<CodeInstruction> codes = instructions.ToList();

            for (int i = 0; i < codes.Count; i++)
            {
                CodeInstruction instruction = codes[i];
                yield return instruction;

                if (instruction.opcode == OpCodes.Isinst &&
                    (Type)instruction.operand == typeof(ArcadeLevel))
                {
                    yield return new CodeInstruction(codes[i + 1]);
                    yield return new CodeInstruction(codes[i + 2]) { operand = enabled };

                    codes[i + 1].opcode = OpCodes.Brtrue;
                    codes[i + 2].labels.Clear();
                }
            }
        }
    }
}
