using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using DuckGame;

using Azxc.UI;

namespace Azxc.Hacks.Misc
{
    [ForceUpdate]
    class SpinMeRound : IAutoUpdate
    {
        private static SpinMeRound _instance;
        public static void Toggle(bool toggle)
        {
            if (toggle && _instance == null)
            {
                _instance = new SpinMeRound();
                Azxc.core.uiManager.AddUpdatable(_instance);
            }
            else if (!toggle && _instance != null)
                Azxc.core.uiManager.RemoveUpdatable(_instance);
        }

        public void Update()
        {
            if (Level.current == null)
                return;

            Duck localDuck = Profiles.activeNonSpectators.Find(x => x.duck?.isLocal ?? false)?.duck;
            if (localDuck == null)
                return;

            localDuck.angleDegrees += 20f;
            if (localDuck.angleDegrees >= 360.0f)
                localDuck.angleDegrees = 0f;
        }
    }
}
