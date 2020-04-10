using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Reflection;
using System.Reflection.Emit;

using DuckGame;
using Harmony;

namespace Azxc.Hacks
{
    // I will don't add original method name in class name only for hacks patches, in other cases
    // i'll do it, like i did Console_RunCommand
    internal static class CommandsBypass
    {
        public static bool enabled;

        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            FieldInfo enabled = AccessTools.Field(typeof(CommandsBypass), "enabled");

            List<CodeInstruction> codes = new List<CodeInstruction>(instructions);

            // Do magic here...

            return codes.AsEnumerable();
        }
    }
}