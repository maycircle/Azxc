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
                hooked = true;
            }
        }

        // RunCommand@DevConsole
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions,
            ILGenerator generator)
        {
            FieldInfo enabled = AccessTools.Field(typeof(CommandsBypass), "enabled");

            List<CodeInstruction> codes = new List<CodeInstruction>(instructions);

            Pattern countPattern = new Pattern(codes);
            string[] textPattern = countPattern.AddInstructions(new string[]
            {
                "call Boolean get_isActive()",
                "brtrue Label",
                "call DuckGame.Level get_current()",
                "isinst DuckGame.ChallengeLevel",
                "brtrue Label?", // "?" means that this line is optional, in some cases it may
                                 // appear and in some not
                "call DuckGame.Level get_current()?",
                "isinst DuckGame.ArcadeLevel?",
                "brfalse Label"
            });
            int count = countPattern.Search().Count;

            for (int i = 0; i < count; i++)
            {
                Pattern pattern = new Pattern(codes);
                pattern.AddInstructions(textPattern);
                Tuple<int, int> code = pattern.Search()[i];

                CodeInstruction ldsfld = new CodeInstruction(codes[code.Item2 + 1]);
                ldsfld.operand = enabled;
                CodeInstruction brfalses = new CodeInstruction(codes[code.Item2]);
                codes[code.Item2].opcode = OpCodes.Brtrue;
                codes[code.Item2 + 1].labels.Clear();

                codes.Insert(code.Item2, ldsfld);
                codes.Insert(code.Item2, brfalses);
            }

            return codes.AsEnumerable();
        }
    }
}
