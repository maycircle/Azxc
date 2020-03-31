using System;

using DuckGame;

namespace Azxc
{
    class BindingAttribute : Attribute
    {
        public Keys key { get; }

        public BindingAttribute(Keys key)
        {
            this.key = key;
        }
    }
}
