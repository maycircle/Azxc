using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

using Harmony;
using DuckGame;

using Azxc.UI.Events;

namespace Azxc.UI.Controls
{
    public class Window : Control, IAutoUpdate
    {
        public SizeModes sizeMode { get; }

        protected Workplace workPlace;

        // Not very important variable
        public Vec2 indent;

        public IEnumerable<Control> items
        {
            get { return workPlace; }
        }

        public Window(Vec2 position, SizeModes sizeMode = SizeModes.Static)
        {
            this.position = position;

            this.size = Vec2.Zero;
            this.sizeMode = sizeMode;

            // This is only for pretty look, so i'll use standart value everywhere
            indent = Vec2.One / 2;

            workPlace = new Workplace();
        }

        public void Load()
        {
            OnLoad(new ControlEventArgs(this));
        }

        public event EventHandler<ControlEventArgs> onLoad;
        protected void OnLoad(ControlEventArgs e)
        {
            onLoad?.Invoke(this, e);
        }

        private void FitToItems()
        {
            float longest = 0f;
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
            }

            int count = (items.Count() / 24) + 1;
            width = count * longest + indent.x * 4;
            if (items.Count() > 24)
                width -= indent.x * count;

            // TODO: Optimize this code, probably
            float sumHeight = 0f;
            if (count > 1)
            {
                int index = 0;
                foreach (Control item in workPlace)
                {
                    if (index >= 24)
                        break;
                    sumHeight += item.height;
                    index++;
                }
                height = sumHeight + (workPlace.inner.y * 24) + indent.y * 5;
            }
            else
            {
                foreach (Control item in workPlace)
                {
                    sumHeight += item.height;
                }
                height = sumHeight + (workPlace.inner.y * workPlace.Count()) + indent.y * 5;
            }
        }

        public virtual void Update()
        {
            workPlace.position = position + indent * 2;
            workPlace.size = size - indent * 2;

            workPlace.Update();
        }

        public override void Draw()
        {
            if (!visible)
                return;

            // Draw windows borders
            Graphics.DrawRect(position, position + size, Color.Black);

            Vec2 end = workPlace.position + workPlace.size - indent;
            end.y += indent.y / 2;
            Graphics.DrawRect(workPlace.position - indent, end,
                Color.DarkSlateGray, 0.1f, false, 0.5f);

            workPlace.Draw();
        }

        public virtual void AddItem(Control item)
        {
            item.parent = this;
            workPlace.Add(item);
            if (sizeMode == SizeModes.Flexible)
                FitToItems();
        }

        public virtual void RemoveItem(Control item)
        {
            workPlace.Remove(item);
            if (sizeMode == SizeModes.Flexible)
                FitToItems();
        }

        public virtual void Clear()
        {
            workPlace.Clear();
        }

        public void Sort(Comparison<Control> comparison)
        {
            workPlace.Sort(comparison);
        }

        public virtual void Show()
        {
            Load();
            Azxc.core.uiManager.AddControl(this);
        }

        public virtual void Close()
        {
            Azxc.core.uiManager.RemoveControl(this);
        }
    }
}
