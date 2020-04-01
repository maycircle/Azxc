using System;

using DuckGame;

namespace Azxc.Bindings
{
    public class BindingAttribute : Attribute
    {
        public Keys key { get; }

        public BindingAttribute(Keys key)
        {
            this.key = key;
        }
    }
}
