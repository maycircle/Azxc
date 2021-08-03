using Azxc.UI;
using DuckGame;
using System.Linq;

namespace Azxc.Patches.Misc
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
                Azxc.GetCore().GetUI().AddUpdatable(_instance);
            }
            else if (!toggle && _instance != null)
            {
                Azxc.GetCore().GetUI().RemoveUpdatable(_instance);
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
