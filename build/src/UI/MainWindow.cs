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
    class MainWindow : Controls.Window
    {
        public Expander<FancyBitmapFont> misc;

        public MainWindow(Vec2 position, SizeModes sizeMode = SizeModes.Static) : base(position, sizeMode)
        {
            misc = new Expander<FancyBitmapFont>(new MiscWindow(position, SizeModes.Flexible),
                "Misc", "Random hacks.", Azxc.core.uiManager.font);

            Prepare();
        }

        public void Prepare()
        {
           AddItem(misc);
        }
    }
}
