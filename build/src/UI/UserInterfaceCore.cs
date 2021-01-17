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
        public UserInterfaceInteract interact;
        // Only for controls that need to be auto-updated (mostly those are windows)
        public List<IAutoUpdate> updatable;

        public Cursor cursor;

        public FancyBitmapFont font;

        public UserInterfaceCore()
        {
            interact = new UserInterfaceInteract();

            updatable = new List<IAutoUpdate>();
            cursor = new Cursor(1f, Vec2.One);

            font = new FancyBitmapFont("smallFont");
            font.scale = new Vec2(0.35f);
        }
    }
}
