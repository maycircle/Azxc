using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using DuckGame;

using Azxc.UI.Events;
using Azxc.UI.Controls;

namespace Azxc.UI
{
    class ArcadeWindow : Controls.Window
    {
        public Button<FancyBitmapFont> ticketsMax, ticketsMin;

        public ArcadeWindow(Vec2 position, SizeModes sizeMode = SizeModes.Static) : base(position, sizeMode)
        {
            ticketsMax = new Button<FancyBitmapFont>("Tickets MAX", "Set tickets count to 999.",
                Azxc.core.uiManager.font);
            ticketsMax.onClicked += TicketsMax_Clicked;

            ticketsMin = new Button<FancyBitmapFont>("Tickets MIN", "Set tickets count to 0.",
                Azxc.core.uiManager.font);
            ticketsMin.onClicked += TicketsMin_Clicked;

            Prepare();
        }

        public void Prepare()
        {
            AddItem(ticketsMax);
            AddItem(ticketsMin);
        }

        private void TicketsMax_Clicked(object sender, ControlEventArgs e)
        {
            Profiles.active[0].ticketCount = 999;
        }

        private void TicketsMin_Clicked(object sender, ControlEventArgs e)
        {
            Profiles.active[0].ticketCount = 0;
        }
    }
}
