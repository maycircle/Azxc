using DuckGame;
using System.Linq;

namespace Azxc.UI.Controls
{
    public class Label : Control, IAutoUpdate, IHasIndent
    {
        public string text { get; set; }
        public FancyBitmapFont font { get; set; }

        public Vec2 indent { get; set; }

        protected Label()
        {
            font = Azxc.GetCore().GetUI().font;
            indent = Vec2.One / 2;
        }

        public Label(string text)
        {
            this.text = text;
            font = Azxc.GetCore().GetUI().font;
            indent = Vec2.One / 2;
        }

        public Label(string text, Vec2 position)
        {
            this.text = text;
            this.position = position;
            font = Azxc.GetCore().GetUI().font;
            indent = Vec2.One / 2;
        }

        protected virtual Vec2 GetLinesSize(string text)
        {
            string[] lines = text.Split('\n');
            return new Vec2(font.GetWidth(lines.Aggregate((longest, next) =>
                next.Length > longest.Length ? next : longest)),
                font.characterHeight * font.scale.y * lines.Length);
        }

        public virtual void Update()
        {
            width = font.GetWidth(text) + indent.x * 2;
            height = font.scale.y * font.characterHeight + indent.y;
        }

        public override void Draw()
        {
            font.Draw(text, position, Color.White, new Depth(1.0f), true);
        }

        public void DrawOutline(Color color, float thickness)
        {
            font.DrawOutline(text, position, Color.White, color, new Depth(1.0f), thickness);
        }
    }
}
