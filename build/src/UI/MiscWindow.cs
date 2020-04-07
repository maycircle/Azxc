using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

using Harmony;
using DuckGame;

using Azxc.UI.Controls;
using Azxc.UI.Events;
using Azxc.Hacks;

namespace Azxc.UI
{
    class MiscWindow : Controls.Window
    {
        public CheckBox<FancyBitmapFont> commandsBypass;

        public MiscWindow(Vec2 position, SizeModes sizeMode = SizeModes.Static) : base(position, sizeMode)
        {
            commandsBypass = new CheckBox<FancyBitmapFont>("Commands Bypass",
                "Ability to call extra commands in DevConsole.", Azxc.core.uiManager.font);
            commandsBypass.onChecked += CommandsBypass_Checked;

            Prepare();
        }

        public void Prepare()
        {
            AddItem(commandsBypass);
        }

        private void CommandsBypass_Checked(object sender, ControlEventArgs e)
        {
            CheckBox<FancyBitmapFont> checkBox = e.item as CheckBox<FancyBitmapFont>;
            CommandsBypass.enabled = checkBox.isChecked;

            // To avoid conflicts
            Azxc.core.harmony.Unpatch(typeof(DevConsole).GetMethod("RunCommand"), HarmonyPatchType.All);

            if (checkBox.isChecked)
            {
                Azxc.core.harmony.Patch(typeof(DevConsole).GetMethod("RunCommand"),
                    transpiler: new HarmonyMethod(typeof(Hacks.CommandsBypass), "Transpiler"));
            }
            else
            {
                Azxc.core.harmony.Patch(typeof(DevConsole).GetMethod("RunCommand"),
                    prefix: new HarmonyMethod(typeof(Console_RunCommand), "Prefix"));
            }
        }
    }
}
