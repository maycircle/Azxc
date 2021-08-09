using Azxc.UI.Controls;
using DuckGame;
using System.Collections.Generic;

namespace Azxc.UI
{
    public class UserInterfaceCore
    {
        // Controls that needed to be auto-updated
        private List<IAutoUpdate> _updatable;

        public UserInterfaceState state;
        public UserInterfaceInteract interact;

        public Cursor cursor;
        public FancyBitmapFont font;

        public UserInterfaceCore()
        {
            _updatable = new List<IAutoUpdate>();

            interact = new UserInterfaceInteract();
            cursor = new Cursor(1.1f, Vec2.One);

            font = new FancyBitmapFont("smallFont");
            font.scale = new Vec2(0.35f);
        }

        public List<IAutoUpdate> GetUpdatable() => _updatable;

        public void AddUpdatable(IAutoUpdate autoUpdate)
        {
            _updatable.Add(autoUpdate);
        }

        public void RemoveUpdatable(IAutoUpdate autoUpdate)
        {
            _updatable.Remove(autoUpdate);
        }
    }
}
