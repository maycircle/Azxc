using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

using Harmony;
using DuckGame;

using Azxc.UI.Controls;

namespace Azxc.UI
{
    class MiscWindow : Controls.Window
    {
        public CheckBox<FancyBitmapFont> commandsBypass;

        public MiscWindow(Vec2 position, SizeModes sizeMode = SizeModes.Static) : base(position, sizeMode)
        {
            commandsBypass = new CheckBox<FancyBitmapFont>("Commands Bypass",
                "Ability to call extra commands in DevConsole.", Azxc.core.uiManager.font);

            Prepare();
        }

        public void Prepare()
        {
            AddItem(commandsBypass);
        }
    }
}
