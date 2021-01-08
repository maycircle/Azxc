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

        public void Clear()
        {
            _items.Clear();
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
            float longest = 0f;
            foreach (Control item in _items)
            {
                if (item.width > longest)
                {
                    IIndent impl = item as IIndent;
                    longest = item.width + impl.indent.x;
                }
            }
            return longest;
        }

        public void Update()
        {
            for (int i = 0; i < _items.Count; i++)
            {
                // 24 is the maximum controls count on a single stack
                int stack = (i % 24);
                int count = (i / 24);

                Control item = _items[i] as Control;
                item.x = x + inner.x + (GetLongestWidth() * count);
                item.y = y + CalculateHeights(stack) + (inner.y * (stack + 1));

                IIndent impl = item as IIndent;
                item.width = GetLongestWidth() - impl.indent.x;
            }
        }

        // TODO: maybe, I should floor (round) all those Graphics draw calls, so the GUI won't look
        // kind of broken sometimes
        public override void Draw()
        {
            foreach (Control item in _items)
            {
                item.Draw();
            }
        }
    }
}
