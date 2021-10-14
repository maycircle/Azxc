using DuckGame;

namespace Azxc.UI.Controls
{
    public class Control
    {
        public Window parent;

        public float x, y;
        public virtual Vec2 position
        {
            get
            {
                return new Vec2(x, y);
            }
            set
            {
                x = value.x;
                y = value.y;
            }
        }

        public float width, height;
        public virtual Vec2 size
        {
            get
            {
                return new Vec2(width, height);
            }
            set
            {
                width = value.x;
                height = value.y;
            }
        }

        public bool isVisible { get; set; }

        public Control()
        {
            position = Vec2.Zero;
            size = Vec2.Zero;
            isVisible = true;
        }

        public Control(Vec2 position, Vec2 size)
        {
            this.position = position;
            this.size = size;
            isVisible = true;
        }

        public virtual void Draw()
        { }

        public virtual void Appear()
        { }
    }
}
