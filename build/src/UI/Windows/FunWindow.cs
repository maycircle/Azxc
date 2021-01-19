using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

using Harmony;
using DuckGame;

using Azxc.UI.Events;
using Azxc.UI.Controls;

namespace Azxc.UI
{
    // No need to hook it anywhere
    [ForceUpdate]
    class AssaultAura : IAutoUpdate
    {
        const float _radius = 30.0f;

        float _degrees;

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

    class FunWindow : Controls.Window
    {
        private CheckBox<FancyBitmapFont> _assaultAura;
        private AssaultAura _AssaultAura;

        public FunWindow(Vec2 position, SizeModes sizeMode = SizeModes.Static) :
            base(position, sizeMode)
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            _assaultAura = new CheckBox<FancyBitmapFont>("Assault Aura", Azxc.core.uiManager.font);
            _assaultAura.onChecked += WeaponAura_Checked;
            AddItem(_assaultAura);
        }

        private void WeaponAura_Checked(object sender, ControlEventArgs e)
        {
            CheckBox<FancyBitmapFont> checkBox = e.item as CheckBox<FancyBitmapFont>;
            if (checkBox.isChecked)
            {
                _AssaultAura = new AssaultAura();
                Azxc.core.uiManager.AddUpdatable(_AssaultAura);
            }
            else
            {
                Azxc.core.uiManager.RemoveUpdatable(_AssaultAura);
                foreach (Holdable holdable in Level.current.things.OfType<Holdable>())
                    holdable.active = true;
            }
        }
    }
}
