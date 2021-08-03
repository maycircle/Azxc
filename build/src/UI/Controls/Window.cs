using Azxc.UI.Events;
using DuckGame;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Azxc.UI.Controls
{
    public class Window : Control, IEnumerable<Control>, IAutoUpdate
    {
        public SizeModes sizeMode { get; }

        public Vec2 indent;
        public Vec2 inner;

        private Vec2 panelPosition;
        private Vec2 panelSize;

        private List<Control> _items;

        public Window()
        {
            position = Vec2.Zero;

            size = Vec2.Zero;
            sizeMode = SizeModes.Flexible;

            panelPosition = new Vec2();
            panelSize = new Vec2();
            indent = Vec2.One / 2;
            inner = (Vec2.One * 1.5f) / 2;

            _items = new List<Control>();
        }

        public Window(Vec2 position, SizeModes sizeMode = SizeModes.Flexible)
        {
            this.position = position;

            size = Vec2.Zero;
            this.sizeMode = sizeMode;

            panelPosition = new Vec2();
            panelSize = new Vec2();
            // This is just for design, so I'll use standart value everywhere
            indent = Vec2.One / 2;
            inner = (Vec2.One * 1.5f) / 2;

            _items = new List<Control>();
        }

        public IEnumerator<Control> GetEnumerator()
        {
            return _items.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _items.GetEnumerator();
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

        private float CalculateHeights(int index)
        {
            float sumHeight = 0f;
            for (int i = 0; i < index; i++)
            {
                Control item = _items[i] as Control;
                sumHeight += item.height;
            }
            return sumHeight;
        }

        private float GetLongestWidth()
        {
            Control longestItem = _items.Aggregate((longest, next) =>
                next.width > longest.width + (longest as IHasIndent).indent.x ? next : longest);
            return longestItem.width + (longestItem as IHasIndent).indent.x;
        }

        private void FitToItems()
        {
            float longest = 0f;
            foreach (Control item in _items)
            {
                if (item is IAutoUpdate)
                {
                    IAutoUpdate updatable = item as IAutoUpdate;
                    updatable.Update();
                }
                if (item.width > longest)
                {
                    IHasIndent impl = item as IHasIndent;
                    longest = item.width + impl.indent.x;
                }
            }

            int count = (_items.Count / 24) + 1;
            width = count * longest + indent.x * 4;
            if (_items.Count > 24)
                width -= indent.x * count;

            // TODO: Optimize this code, probably, one day...
            float sumHeight = 0f;
            if (count > 1)
            {
                int index = 0;
                foreach (Control item in _items)
                {
                    if (index >= 24)
                        break;
                    sumHeight += item.height;
                    index++;
                }
                height = sumHeight + (inner.y * 24) + indent.y * 5;
            }
            else
            {
                foreach (Control item in _items)
                {
                    sumHeight += item.height;
                }
                height = sumHeight + (inner.y * _items.Count) + indent.y * 5;
            }
        }

        public virtual void Update()
        {
            panelPosition = position + indent * 2;
            panelSize = size - indent * 2;

            for (int i = 0; i < _items.Count; i++)
            {
                // 24 is the maximum controls count on a single stack
                int stack = (i % 24);
                int count = (i / 24);

                Control item = _items[i];
                item.x = panelPosition.x + inner.x + (GetLongestWidth() * count) - (inner.x * count);
                item.y = panelPosition.y + CalculateHeights(stack) + (inner.y * (stack + 1));

                IHasIndent impl = item as IHasIndent;
                item.width = GetLongestWidth() - impl.indent.x;
            }
        }

        public override void Draw()
        {
            if (!visible)
                return;

            // Draw borders
            Graphics.DrawRect(position, position + size, Color.Black);

            Vec2 end = panelPosition + panelSize - indent;
            end.y += indent.y / 2;
            Graphics.DrawRect(panelPosition - indent, end,
                Color.DarkSlateGray, 0.1f, false, 0.5f);

            foreach (Control item in _items)
            {
                item.Draw();
            }
        }

        public virtual void AddItem(Control item)
        {
            item.parent = this;
            _items.Add(item);
            if (sizeMode == SizeModes.Flexible)
                FitToItems();
        }

        public virtual void RemoveItem(Control item)
        {
            _items.Remove(item);
            if (sizeMode == SizeModes.Flexible)
                FitToItems();
        }

        public virtual void Clear()
        {
            _items.Clear();
        }

        public void Sort(Comparison<Control> comparison)
        {
            _items.Sort(comparison);
        }

        public virtual void Show()
        {
            Load();
            Azxc.GetCore().GetUI().AddUpdatable(this);
        }

        public virtual void Close()
        {
            Azxc.GetCore().GetUI().RemoveUpdatable(this);
        }
    }
}
