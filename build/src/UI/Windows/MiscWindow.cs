using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

using Harmony;
using DuckGame;

using Azxc.UI.Controls;
using Azxc.UI.Events;
using Azxc.Patches;

namespace Azxc.UI
{
    class MiscWindow : Controls.Window
    {
        private CheckBox<FancyBitmapFont> _commandsBypass, _lobbyTimout,
            _consoleImplementation;

        public MiscWindow(Vec2 position, SizeModes sizeMode = SizeModes.Static) :
            base(position, sizeMode)
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            bool.TryParse(Azxc.GetCore().GetConfig().TryGetSingle("EnableDevConsoleImpl", "True"),
                out DevConsoleImpl.enabled);
            DevConsoleImpl.HookAndToggle(DevConsoleImpl.enabled);
            _consoleImplementation = new CheckBox<FancyBitmapFont>("Console Implementation",
                "Enable Azxc's DevConsole implementation: syntax, commands |YELLOW|(EnableDevConsoleImpl)." +
                "|DGBLUE|\nSyntax examples: `[p1]`, `[lp1]`, `[ap1]`, `[p1:x]`, [p1:xp].\n" +
                "Available commands: azxc_steal.",
                Azxc.GetCore().GetUI().font, DevConsoleImpl.enabled);
            _consoleImplementation.onChecked += ConsoleImplementation_Checked;
            AddItem(_consoleImplementation);

            _commandsBypass = new CheckBox<FancyBitmapFont>("Commands Bypass",
                "Ability to call extra commands in DevConsole.", Azxc.GetCore().GetUI().font);
            _commandsBypass.onChecked += CommandsBypass_Checked; AddItem(_commandsBypass);

            AddItem(new Separator());

            _lobbyTimout = new CheckBox<FancyBitmapFont>("Lobby Timeout",
                "Kick from lobby after being AFK for 5 minutes.", Azxc.GetCore().GetUI().font, true);
            _lobbyTimout.onChecked += LobbyTimeout_Checked; AddItem(_lobbyTimout);
        }

        private void CommandsBypass_Checked(object sender, ControlEventArgs e)
        {
            CheckBox<FancyBitmapFont> checkBox = e.item as CheckBox<FancyBitmapFont>;
            CommandsBypass.HookAndToggle(checkBox.isChecked);
        }

        private void LobbyTimeout_Checked(object sender, ControlEventArgs e)
        {
            CheckBox<FancyBitmapFont> checkBox = e.item as CheckBox<FancyBitmapFont>;

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

        private void ConsoleImplementation_Checked(object sender, ControlEventArgs e)
        {
            CheckBox<FancyBitmapFont> checkBox = e.item as CheckBox<FancyBitmapFont>;
            DevConsoleImpl.HookAndToggle(checkBox.isChecked);
            Azxc.GetCore().GetConfig().TrySet("EnableDevConsoleImpl", checkBox.isChecked.ToString());
        }
    }
}
