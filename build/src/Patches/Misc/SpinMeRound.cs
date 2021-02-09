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
            {
                Azxc.core.uiManager.RemoveUpdatable(_instance);
                _instance = null;
            }
        }

        public void Update()
        {
            if (Level.current == null)
                return;

            foreach (Profile profile in Profiles.activeNonSpectators.Where(x => x.duck != null && x.localPlayer))
            {
                profile.duck.angleDegrees += 20f;
                if (profile.duck.angleDegrees >= 360.0f)
                    profile.duck.angleDegrees = 0f;
            }
        }
    }
}
