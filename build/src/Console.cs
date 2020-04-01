using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

using Harmony;
using DuckGame;

namespace Azxc
{
    [HarmonyPatch(typeof(DevConsole), "RunCommand")]
    internal class Console_RunCommand
    {
        // Walk through all methods in Console class and execute method if it has CommandAttribute
        // and user input equals to method's command
        static bool Prefix(string command)
        {
            MethodInfo[] methods = typeof(Console).GetMethods();
            foreach (MethodInfo method in methods)
            {
                object[] attributes = method.GetCustomAttributes(true);
                foreach (object attribute in attributes)
                {
                    if (attribute is CommandAttribute)
                    {
                        CommandAttribute found = (attribute as CommandAttribute);
                        string commandName = found.Command();

                        if (commandName == command)
                        {
                            method.Invoke(new Console(), null);
                            return false;
                        }
                    }
                }
            }
            return true;
        }
    }

    class Console
    {
        [Command("test")]
        public void Test()
        {
            // Add line in console
            DevConsole.core.lines.Enqueue(new DCLine
            {
                line = "Test command.",
                color = Color.Yellow
            });
        }
    }
}
