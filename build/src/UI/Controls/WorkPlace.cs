using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

using Harmony;
using DuckGame;

namespace Azxc.UI.Controls
{
    public class Workplace : Control, IEnumerable<Control>, IAutoUpdate
    {
        public Vec2 inner;

        protected List<Control> _items;

        public Workplace()
        {
            _items = new List<Control>();
            inner = Vec2.One / 2;
        }

        public Workplace(List<Control> items)
        {
            _items = items;
            inner = Vec2.One / 2;
        }

        public IEnumerator<Control> GetEnumerator()
        {
            return _items.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _items.GetEnumerator();
        }

        public void Add(Control item)
        {
            _items.Add(item);
        }

        public void Remove(Control item)
        {
            _items.Remove(item);
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

        public void Update()
        {
            for (int i = 0; i < _items.Count; i++)
            {
                Control item = _items[i] as Control;
                if (item is IAutoUpdate)
                {
                    IAutoUpdate updatable = item as IAutoUpdate;
                    updatable.Update();
                }
                item.x = x + inner.x;
                item.y = y + CalculateHeights(i) + (inner.y * (i + 1));
                IIndent impl = item as IIndent;
                if (item.width + impl.indent.x < width)
                    item.width = width - inner.x * 4;
            }
        }

        public override void Draw()
        {
            foreach (Control item in _items)
            {
                item.Draw();
            }
        }
    }
}
