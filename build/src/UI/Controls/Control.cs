using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

using Harmony;
using DuckGame;

namespace Azxc
{
    class Control
    {
        public Vec2 position;

        public Control(Vec2 position)
        {
            this.position = position;
        }

        public virtual void Draw()
        {

        }
    }
}
