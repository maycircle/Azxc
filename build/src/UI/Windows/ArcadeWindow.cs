using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

using Harmony;
using DuckGame;

using Azxc.UI.Events;
using Azxc.UI.Controls;
using Azxc.Hacks;

namespace Azxc.UI
{
    class ArcadeWindow : Controls.Window
    {
        public CheckBox<FancyBitmapFont> pauseTimer;

        public ArcadeWindow(Vec2 position, SizeModes sizeMode = SizeModes.Static) : base(position, sizeMode)
        {
            ticketsMax = new Button<FancyBitmapFont>("Tickets MAX", "Set tickets count to 999.",
                Azxc.core.uiManager.font);
            ticketsMax.onClicked += TicketsMax_Clicked;

            ticketsMin = new Button<FancyBitmapFont>("Tickets MIN", "Set tickets count to 0.",
                Azxc.core.uiManager.font);
            ticketsMin.onClicked += TicketsMin_Clicked;

            pauseTimer = new CheckBox<FancyBitmapFont>("Pause Timer", "Become incredibly fast, relative to time...",
                Azxc.core.uiManager.font);
            pauseTimer.onChecked += PauseTimer_Checked;

            Prepare();
        }

        public void Prepare()
        {
            AddItem(ticketsMax);
            AddItem(ticketsMin);
            AddItem(new Separator());
            AddItem(pauseTimer);
        }

        private void TicketsMax_Clicked(object sender, ControlEventArgs e)
        {
            Profiles.active[0].ticketCount = 999;
        }

        private void TicketsMin_Clicked(object sender, ControlEventArgs e)
        {
            Profiles.active[0].ticketCount = 0;
        }

        private void PauseTimer_Checked(object sender, ControlEventArgs e)
        {
            CheckBox<FancyBitmapFont> checkBox = e.item as CheckBox<FancyBitmapFont>;

            PauseTimer.enabled = checkBox.isChecked;
            if (ChallengeLevel.timer != null && checkBox.isChecked)
                ChallengeLevel.timer.Stop();
            else if (ChallengeLevel.timer != null && !checkBox.isChecked)
                ChallengeLevel.timer.Start();

            MethodInfo original = typeof(ChallengeLevel).GetMethod("Update");

            if (Azxc.core.harmony.GetPatchInfo(original) == null)
            {
                Azxc.core.harmony.Patch(original,
                    prefix: new HarmonyMethod(typeof(Hacks.PauseTimer), "Prefix"));
            }
        }
    }
}
