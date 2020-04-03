using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

using Harmony;
using DuckGame;

namespace Azxc.UI.Controls
{
    class Window : Control, IAutoUpdate, IPosition, ISize
    {
        public Vec2 position { get; set; }
        public Vec2 size { get; set; }

        private List<Control> _items;

        public Window(Vec2 position, Vec2 size)
        {
            this.position = position;
            this.size = size;

            _items = new List<Control>();
        }

        public void Update()
        {

        }

        public override void Draw()
        {
            if (!visible)
                return;
        }

        public virtual void Show()
        {
            Azxc.core.uiManager.AddControl(this);
        }

        public virtual void Close()
        {
            Azxc.core.uiManager.RemoveControl(this);
        }
    }
}
