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

        public bool visible { get; set; }

        public Control()
        {
            this.position = Vec2.Zero;
            this.size = Vec2.Zero;
            visible = true;
        }

        public Control(Vec2 position, Vec2 size)
        {
            this.position = position;
            this.size = size;
            visible = true;
        }

        public virtual void Draw()
        {
            
        }
    }
}
