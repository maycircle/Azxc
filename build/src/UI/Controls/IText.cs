using System;

using DuckGame;

namespace Azxc.UI.Controls
{
    interface IText<T>
    {
        string text { get; set; }
        T font { get; }

        // Still i don't know how to get characters height
        int characterHeight { get; }
    }
}
