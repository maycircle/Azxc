using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

using Harmony;
using DuckGame;

using Azxc.Bindings;
using Azxc.UI.Controls;

namespace Azxc.UI
{
    public class UserInterfaceCore
    {
        public UserInterfaceState state;
        // Only for controls which needs to be auto-updated (Windows)
        public List<Control> controls;

        public Cursor cursor;
        public FancyBitmapFont font;

        public UserInterfaceCore()
        {
            controls = new List<Control>();
            cursor = new Cursor(1f, Vec2.One);
            font = new FancyBitmapFont("smallFont");
        }
    }
}
