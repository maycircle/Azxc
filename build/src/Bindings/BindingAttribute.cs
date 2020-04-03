using System;

using DuckGame;

namespace Azxc.Bindings
{
    public class BindingAttribute : Attribute
    {
        public object bind { get; }
        public InputState state { get; }

        public BindingAttribute(Keys key, InputState state)
        {
            this.state = state;
            bind = key;
        }

        public BindingAttribute(MouseButtons button, InputState state)
        {
            this.state = state;
            bind = button;
        }

        private bool UsedKeyboard()
        {
            switch (state)
            {
                case InputState.Pressed:
                    return Keyboard.Pressed((Keys)bind);
                case InputState.Down:
                    return Keyboard.Down((Keys)bind);
                case InputState.Released:
                    return Keyboard.Released((Keys)bind);
                default:
                    return false;
            }
        }

        private bool UsedMouse()
        {
            switch ((MouseButtons)bind)
            {
                case MouseButtons.Left:
                    return Mouse.left == state;
                case MouseButtons.Right:
                    return Mouse.right == state;
                case MouseButtons.Middle:
                    return Mouse.left == state;
                default:
                    return false;
            }
        }

        public virtual bool UsedBinding()
        {
            if (bind is Keys)
                return UsedKeyboard();
            else
                return UsedMouse();
        }
    }
}
