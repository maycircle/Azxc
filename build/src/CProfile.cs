using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DuckGame;

namespace Azxc
{
    public class CProfile : CMD.Argument
    {
        public CProfile(string name, bool optional = false) : base(name, optional)
        { }

        public override object Parse(string name)
        {
            Profile profile = DevConsole.ProfileByName(name);
            if (profile == null)
            {
                return Error("No profile named " + name + ".");
            }

            return profile;
        }
    }
}
