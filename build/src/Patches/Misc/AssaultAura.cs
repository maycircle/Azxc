using Azxc.UI;
using DuckGame;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Azxc.Patches.Misc
{
    [ForceUpdate]
    class AssaultAura : IAutoUpdate
    {
        private const float _radius = 30.0f;
        private float _degrees;

        private static AssaultAura _instance;
        public static void Toggle(bool toggle)
        {
            if (toggle && _instance == null)
            {
                _instance = new AssaultAura();
                Azxc.GetCore().GetUI().AddUpdatable(_instance);
            }
            else if (!toggle && _instance != null)
            {
                foreach (Holdable holdable in Level.current.things.OfType<Holdable>())
                    holdable.active = true;
                Azxc.GetCore().GetUI().RemoveUpdatable(_instance);
                _instance = null;
            }
        }

        public void Update()
        {
            if (Level.current == null)
                return;

            Duck localDuck = Profiles.activeNonSpectators.Find(x => x.duck?.isLocal ?? false)?.duck;
            if (localDuck == null)
                return;

            IEnumerable<Holdable> holdableThings = Level.current.things.OfType<Holdable>();
            for (int i = 0; i < holdableThings.Count(); i++)
            {
                Holdable current = holdableThings.ElementAt(i);
                if (current is RagdollPart && ((RagdollPart)current).doll == localDuck.ragdoll)
                    continue;
                current.active = false; // Disable things usability
                // current.velocity = Vec2.Zero; // Disable things randomly clipping (keep usability)

                Vec2 position = localDuck.ragdoll == null ? localDuck.position : localDuck.ragdoll.position;
                position.x += (float)Math.Cos(_degrees + 360.0f / i) * _radius;
                position.y += (float)Math.Sin(_degrees + 360.0f / i) * _radius;

                current.position = Vec2.Lerp(current.position, position, 0.2f);
                current.angle = (float)Math.Sin(_degrees);
            }

            _degrees += 0.05f * localDuck.offDir;
            if (_degrees >= 360.0f)
                _degrees = 0f;
        }
    }
}
