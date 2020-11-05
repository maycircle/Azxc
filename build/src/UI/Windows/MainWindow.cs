using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

using Harmony;
using DuckGame;

using Azxc.UI.Events;
using Azxc.UI.Controls;

namespace Azxc.UI
{
    class MainWindow : Controls.Window
    {
        public Expander<FancyBitmapFont> weapons, commands, arcade, misc;

        public MainWindow(Vec2 position, SizeModes sizeMode = SizeModes.Static) : base(position, sizeMode)
        {
            weapons = new Expander<FancyBitmapFont>(new WeaponsWindow(position, SizeModes.Flexible),
                "Weapons", "Weapon(s) hacks.", Azxc.core.uiManager.font);

            commands = new Expander<FancyBitmapFont>(new ConsoleWindow(position, SizeModes.Flexible),
                "Commands", "Probably every command that this game has.", Azxc.core.uiManager.font);

            arcade = new Expander<FancyBitmapFont>(new ArcadeWindow(position, SizeModes.Flexible),
                "Arcade", "Challenges-related stuff.", Azxc.core.uiManager.font);

            misc = new Expander<FancyBitmapFont>(new MiscWindow(position, SizeModes.Flexible),
                "Misc", "Random hacks.", Azxc.core.uiManager.font);

            Prepare();
        }

        public void Prepare()
        {
            AddItem(weapons);
            AddItem(commands);
            AddItem(arcade);
            AddItem(misc);
        }
    }
}
