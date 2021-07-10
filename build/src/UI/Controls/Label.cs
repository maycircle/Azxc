using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

using Harmony;
using DuckGame;

namespace Azxc.UI.Controls
{
    public class Label<T> : Control, IAutoUpdate, IHasIndent
    {
        public string text { get; set; }
        public T font { get; set; }
        public int characterHeight { get; private set; }

        public Vec2 indent { get; set; }

        public Label(string text, T font)
        {
            this.text = text;
            this.font = font;

            if (typeof(T) == typeof(FancyBitmapFont))
                characterHeight = (font as FancyBitmapFont).characterHeight;
            indent = Vec2.One / 2;
        }

        public Label(string text, T font, Vec2 position)
        {
            this.text = text;
            this.font = font;
            this.position = position;

            if (typeof(T) == typeof(FancyBitmapFont))
                characterHeight = (font as FancyBitmapFont).characterHeight;
            indent = Vec2.One / 2;
        }

        protected virtual float GetWidth(string text)
        {
            MethodInfo getWidth = AccessTools.Method(typeof(T), "GetWidth");
            return (float)getWidth.Invoke(font, new object[] { text, false });
        }

        protected virtual Vec2 GetLinesSize(string text)
        {
            string[] lines = text.Split('\n');
            return new Vec2(GetWidth(lines.Aggregate((longest, next) =>
                next.Length > longest.Length ? next : longest)),
                characterHeight * GetScale().y * lines.Length);
        }

        public bool SetCharacterHeight(int characterHeight)
        {
            if (this.characterHeight <= 0)
            {
                this.characterHeight = characterHeight;
                return true;
            }
            return false;
        }

        public float GetWidth()
        {
            return GetWidth(text);
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

        protected void DrawText(string text, Vec2 position, Color color, Depth depth, bool colorSymbols)
        {
            // Using AccessTools.Method because i'm too lazy to use basic typeof ;)
            MethodInfo draw = AccessTools.Method(typeof(T), "Draw", new Type[] { typeof(string),
                typeof(Vec2), typeof(Color), typeof(Depth), typeof(bool) });
            draw.Invoke(font, new object[] { text, position, color, depth, colorSymbols });
        }

        public override void Draw()
        {
            DrawText(text, position, Color.White, new Depth(1.0f), true);
        }
    }
}
