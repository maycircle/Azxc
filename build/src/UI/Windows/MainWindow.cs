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
        private Expander<FancyBitmapFont> _weapons, _commands, _arcade, _network, _fun, _misc;

        public MainWindow(Vec2 position, SizeModes sizeMode = SizeModes.Static) :
            base(position, sizeMode)
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            _weapons = new Expander<FancyBitmapFont>(new WeaponsWindow(position, SizeModes.Flexible),
                "Weapons", "Weapon(s) hacks.", Azxc.core.uiManager.font); AddItem(_weapons);
            _commands = new Expander<FancyBitmapFont>(new ConsoleWindow(position, SizeModes.Flexible),
                "Commands", "GUI for console commands.", Azxc.core.uiManager.font); AddItem(_commands);
            _arcade = new Expander<FancyBitmapFont>(new ArcadeWindow(position, SizeModes.Flexible),
                "Arcade", "Challenges-related stuff.", Azxc.core.uiManager.font); AddItem(_arcade);
            _network = new Expander<FancyBitmapFont>(new NetworkWindow(position, SizeModes.Flexible),
                "Network", "(Duck)Networking-related stuff.", Azxc.core.uiManager.font); AddItem(_network);
            _fun = new Expander<FancyBitmapFont>(new FunWindow(position, SizeModes.Flexible),
                "Fun", "Stuff to show off with. Lots of glitches, have fun :)",
                Azxc.core.uiManager.font); AddItem(_fun);
            _misc = new Expander<FancyBitmapFont>(new MiscWindow(position, SizeModes.Flexible),
                "Misc", "Random stuff.", Azxc.core.uiManager.font); AddItem(_misc);
        }
    }
}
