using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

using Harmony;
using DuckGame;

namespace Azxc.UI.Controls
{
    class Window : Control, IAutoUpdate
    {
        public SizeModes sizeMode { get; }

        protected Workplace workPlace;

        // Not very important variable
        public Vec2 indent;

        public Window(Vec2 position, SizeModes sizeMode = SizeModes.Static)
        {
            this.position = position;

            this.size = Vec2.Zero;
            this.sizeMode = sizeMode;

            // This is only for pretty look, so i'll use standart value everywhere
            indent = Vec2.One / 2;

            workPlace = new Workplace();
        }

        private void FitToItems()
        {
            float longest = 0f;
            float sumHeight = 0f;
            foreach (Control item in workPlace)
            {
                if (item is IAutoUpdate)
                {
                    IAutoUpdate updatable = item as IAutoUpdate;
                    updatable.Update();
                }
                if (item.width > longest)
                {
                    IIndent impl = item as IIndent;
                    longest = item.width + impl.indent.x;
                }
                sumHeight += item.height;
            }
            width = longest + indent.x * 4;
            height = sumHeight + (workPlace.inner.y * workPlace.Count()) + indent.y * 5;
        }

        public void Update()
        {
            workPlace.position = position + indent * 2;
            workPlace.size = size - indent * 2;

            workPlace.Update();
        }

        public override void Draw()
        {
            if (!visible)
                return;

            // Window borders themselves
            Graphics.DrawRect(position, position + size, Color.Black);
            Graphics.DrawRect(workPlace.position - indent, workPlace.position + workPlace.size - indent,
                Color.DarkSlateGray, 0.1f, false, 0.5f);

            workPlace.Draw();
        }

        public virtual void AddItem(Control item)
        {
            workPlace.Add(item);
            if (sizeMode == SizeModes.Flexible)
                FitToItems();
        }

        public virtual void RemoveItem(Control item)
        {
            workPlace.Remove(item);
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
