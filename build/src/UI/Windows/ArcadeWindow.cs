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
        private Button<FancyBitmapFont> _ticketsMax, _ticketsMin, _finishChallenge;
        private CheckBox<FancyBitmapFont> _pauseTimer;

        public ArcadeWindow(Vec2 position, SizeModes sizeMode = SizeModes.Static) :
            base(position, sizeMode)
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            _ticketsMax = new Button<FancyBitmapFont>("Tickets MAX", "Set tickets amount to 999.",
                Azxc.core.uiManager.font);
            _ticketsMax.onClicked += TicketsMax_Clicked; AddItem(_ticketsMax);

            _ticketsMin = new Button<FancyBitmapFont>("Tickets MIN", "Set tickets amount to 0.",
                Azxc.core.uiManager.font);
            _ticketsMin.onClicked += TicketsMin_Clicked; AddItem(_ticketsMin);

            AddItem(new Separator());

            _pauseTimer = new CheckBox<FancyBitmapFont>("Pause Timer", "Become incredibly fast, relative to time... :)",
                Azxc.core.uiManager.font);
            _pauseTimer.onChecked += PauseTimer_Checked; AddItem(_pauseTimer);

            _finishChallenge = new Button<FancyBitmapFont>("Finish Challenge", "Complete the challenge (Developer included).",
                Azxc.core.uiManager.font);
            _finishChallenge.onClicked += FinishChallenge_Clicked; AddItem(_finishChallenge);
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

            PauseTimer.Hook();
        }

        // This code right here, officer
        private void FinishChallenge_Clicked(object sender, ControlEventArgs e)
        {
            if (ChallengeLevel.running)
            {
                ChallengeLevel challengeLevel = ChallengeLevel.current as ChallengeLevel;
                ChallengeMode challenge = AccessTools.Field(typeof(ChallengeLevel),
                    "_challenge").GetValue(challengeLevel) as ChallengeMode;

                List<ChallengeTrophy> simulatedTrophies = new List<ChallengeTrophy>();
                simulatedTrophies.Add(new ChallengeTrophy(challenge.challenge));
                simulatedTrophies[0].type = TrophyType.Developer;

                ChallengeLevel.timer.Stop();
                AccessTools.Field(typeof(ChallengeMode), "_wonTrophies")
                    .SetValue(challenge, simulatedTrophies);
                challengeLevel.ChallengeEnded(challenge);
            }
        }
    }
}
