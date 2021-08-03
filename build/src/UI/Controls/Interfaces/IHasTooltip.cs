namespace Azxc.UI.Controls
{
    public interface IHasTooltip
    {
        bool showToolTip { get; set; }
        string toolTipText { get; set; }

        void DrawTooltip();
    }
}
