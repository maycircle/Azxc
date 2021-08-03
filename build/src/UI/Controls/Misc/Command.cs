using System.Collections.Generic;

namespace Azxc.UI.Controls.Misc
{
    abstract class Command : Window
    {
        protected List<ISelector> _selectors;

        public Command()
        {
            _selectors = new List<ISelector>();
        }

        public virtual void AddSelector(ISelector selector)
        {
            _selectors.Add(selector);
        }
    }
}
