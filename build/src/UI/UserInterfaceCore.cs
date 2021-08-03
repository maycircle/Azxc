using Azxc.UI.Controls;
using DuckGame;
using System.Collections.Generic;

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
            cursor = new Cursor(1.1f, Vec2.One);

            font = new FancyBitmapFont("smallFont");
            font.scale = new Vec2(0.35f);
        }
    }
}
