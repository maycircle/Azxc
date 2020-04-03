using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

using Harmony;
using DuckGame;

namespace Azxc.UI.Controls
{
    public class Control
    {
        public bool visible { get; set; }

        public Control()
        {
            visible = true;
        }

        public virtual void Draw()
        {
            
        }
    }
}
