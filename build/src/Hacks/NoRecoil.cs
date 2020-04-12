using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Reflection;
using System.Reflection.Emit;

namespace Azxc.Hacks
{
    internal static class NoRecoil
    {
        public static bool enabled;

        static bool Prefix()
        {
            if (enabled)
                return false;
            return true;
        }
    }
}