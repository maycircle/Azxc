using Azxc.UI.Events;
using DuckGame;
using System;

namespace Azxc.UI.Controls.Misc
{
    /// <summary>
    /// Unlike <c>Expander</c>, initializes window that needs to be expanded
    /// before the moment it gets clicked.
    /// </summary>
    /// <typeparam name="T">
    /// The unsealed class of window that will be initialized.
    /// </typeparam>
    public class AbstractExpander<T> : Button where T : Window
    {
        private readonly Vec2 ArrowOffset = new Vec2(4f, 1.6f);

        public Window window { get; private set; }

        public AbstractExpander(string text) : base(text) { }

        public AbstractExpander(string text, string toolTipText) :
            base(text, toolTipText)
        { }

        public AbstractExpander(string text, string toolTipText, Vec2 position) :
            base(text, toolTipText, position)
        { }

        public override void Update()
        {
            width = font.GetWidth(text) + indent.x * 2 + 5f;
            height = font.characterHeight * font.scale.y + indent.y * 2;
        }

        public override void Draw()
        {
            base.Draw();

            Sprite arrow = new Sprite("contextArrowRight");
            arrow.scale = new Vec2(0.5f);
            Vec2 arrowPosition = new Vec2(x + width - indent.x, y + height / 2) - ArrowOffset;
            Graphics.Draw(arrow, arrowPosition.x, arrowPosition.y, 1f);
        }

        public virtual void Expand()
        {
            float windowIndent = 2f;
            window = Activator.CreateInstance<T>();
            window.position = new Vec2(x + width + indent.x + windowIndent, y - 0.5f * 3);
            window.Show();
            OnExpanded(new ControlEventArgs(this));
        }

        public event EventHandler<ControlEventArgs> onExpanded;
        protected void OnExpanded(ControlEventArgs e)
        {
            onExpanded?.Invoke(this, e);
        }

        public override void Click()
        {
            base.Click();

            Expand();
        }
    }
}
