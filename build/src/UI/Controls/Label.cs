using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

using Harmony;
using DuckGame;

namespace Azxc.UI.Controls
{
    public class Label<T> : Control, IAutoUpdate, IIndent
    {
        public string text { get; set; }
        public T font { get; set; }
        public int characterHeight { get; }

        public Vec2 indent { get; set; }

        public Label(string text, T font)
        {
            this.text = text;
            this.font = font;

            characterHeight = 7;
            indent = Vec2.One / 2;
        }

        public Label(string text, T font, Vec2 position)
        {
            this.text = text;
            this.font = font;
            this.position = position;

            characterHeight = 7;
            indent = Vec2.One / 2;
        }

        public virtual float GetWidth()
        {
            MethodInfo getWidth = AccessTools.Method(typeof(T), "GetWidth");
            return (float)getWidth.Invoke(font, new object[] { text, false });
        }

        public virtual Vec2 GetScale()
        {
            PropertyInfo scale = AccessTools.Property(typeof(T), "scale");
            return (Vec2)scale.GetValue(font, null);
        }

        public virtual void Update()
        {
            width = GetWidth() + indent.x * 2;
            height = characterHeight * GetScale().y + indent.y;
        }

        public override void Draw()
        {
            // Using AccessTools.Method because i'm too lazy to use basic typeof ;)
            MethodInfo draw = AccessTools.Method(typeof(T), "Draw",new Type[] { typeof(string),
                typeof(Vec2), typeof(Color), typeof(Depth), typeof(bool) });
            draw.Invoke(font, new object[] { text, position, Color.White, new Depth(1f), true });
        }
    }
}
