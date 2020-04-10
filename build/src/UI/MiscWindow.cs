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
        public CheckBox<FancyBitmapFont> commandsBypass, lobbyTimout;

        public MiscWindow(Vec2 position, SizeModes sizeMode = SizeModes.Static) : base(position, sizeMode)
        {
            commandsBypass = new CheckBox<FancyBitmapFont>("Commands Bypass",
                "Ability to call extra commands in DevConsole.", Azxc.core.uiManager.font);
            commandsBypass.onChecked += CommandsBypass_Checked;

            lobbyTimout = new CheckBox<FancyBitmapFont>("Lobby Timeout",
                "Kick from lobby after being AFK for 5 minutes.", Azxc.core.uiManager.font, true);
            lobbyTimout.onChecked += LobbyTimeout_Checked;

            Prepare();
        }

        public void Prepare()
        {
            AddItem(commandsBypass);
            AddItem(lobbyTimout);
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

        private void LobbyTimeout_Checked(object sender, ControlEventArgs e)
        {
            CheckBox<FancyBitmapFont> checkBox = e.item as CheckBox<FancyBitmapFont>;
            CommandsBypass.enabled = checkBox.isChecked;

            FieldInfo showTimeout = AccessTools.Field(typeof(TeamSelect2), "_afkShowTimeout");
            FieldInfo maxTimeout = AccessTools.Field(typeof(TeamSelect2), "_afkMaxTimeout");

            if (checkBox.isChecked && Level.current is TeamSelect2)
            {
                showTimeout.SetValue(Level.current as TeamSelect2, 300f);
                maxTimeout.SetValue(Level.current as TeamSelect2, 241f);
            }
            else if (!checkBox.isChecked && Level.current is TeamSelect2)
            {
                showTimeout.SetValue(Level.current as TeamSelect2, float.MaxValue);
                maxTimeout.SetValue(Level.current as TeamSelect2, float.MaxValue);
            }
        }
    }
}
