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

        // The position and size where all controls would start drawing
        public Vec2 workPlacePosition;
        public Vec2 workPlaceSize;

        protected List<Control> _items;

        // Not very important variable
        public Vec2 indent;

        public Window(Vec2 position, Vec2 size)
        {
            this.position = position;
            this.size = size;
            // This is only for pretty look, so i'll use standart value everywhere
            indent = Vec2.One / 2;

            _items = new List<Control>();
        }

        public void Update()
        {
            workPlacePosition = position + indent * 2;
            workPlaceSize = size - indent * 2;
        }

        public override void Draw()
        {
            if (!visible)
                return;

            // Window borders themselves
            Graphics.DrawRect(position, position + size, Color.Black);
            Graphics.DrawRect(workPlacePosition - indent, workPlacePosition + workPlaceSize - indent,
                Color.DarkSlateGray, 0.1f, false, 0.5f);
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
