using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

using Harmony;
using DuckGame;

namespace Azxc.UI.Controls
{
    class Cursor : Control, IAutoUpdate, IPosition
    {
        public Vec2 position { get; set; }

        private SpriteMap _sprite;

        public Vec2 scale
        {
            get { return _sprite.scale; }
            set { _sprite.scale = value; }
        }

        public Cursor(Depth depth, Vec2 scale)
        {
            _sprite = new SpriteMap("cursors", 16, 16, false);
            _sprite.depth = depth;
            _sprite.scale = scale;
        }

        public void Update()
        {
            _sprite.position = position;
        }

        public override void Draw()
        {
            _sprite.Draw();
        }
    }
}
