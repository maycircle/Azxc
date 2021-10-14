using Azxc.Patches;
using Azxc.UI.Controls;
using Azxc.UI.Events;
using DuckGame;
using Harmony;
using System.Collections.Generic;

namespace Azxc.UI
{
    class ArcadeWindow : Controls.Window
    {
        private Button _ticketsMax, _ticketsMin, _finishChallenge;
        private CheckBox _pauseTimer;

        public ArcadeWindow()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            _ticketsMax = new Button("Tickets MAX", "Set tickets amount to 999.");
            _ticketsMax.onClicked += TicketsMax_Clicked;
            AddItem(_ticketsMax);

            _ticketsMin = new Button("Tickets MIN", "Set tickets amount to 0.");
            _ticketsMin.onClicked += TicketsMin_Clicked;
            AddItem(_ticketsMin);

            AddItem(new Separator());

            _pauseTimer = new CheckBox("Pause Timer",
                "Become incredibly fast, relative to time... :)");
            _pauseTimer.onChecked += PauseTimer_Checked;
            AddItem(_pauseTimer);

            _finishChallenge = new Button("Finish Challenge",
                "Complete the challenge (Developer included).");
            _finishChallenge.onClicked += FinishChallenge_Clicked;
            AddItem(_finishChallenge);
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
            CheckBox checkBox = sender as CheckBox;

            PauseTimer.enabled = checkBox.isChecked;
            if (ChallengeLevel.timer != null && checkBox.isChecked)
                ChallengeLevel.timer.Stop();
            else if (ChallengeLevel.timer != null && !checkBox.isChecked)
                ChallengeLevel.timer.Start();

            PauseTimer.Hook();
        }

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
