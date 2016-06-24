using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace btwm.Layouts
{
    class Split : Layout
    {
        public Split(Workspace ws) : base(ws, LayoutType.split)
        { }

        public override void InsertWindow(Window newWin)
        {
            ManagedWindows.Add(newWin);
            redrawWindows();
        }

        public override void RemoveWindow(Window toRemove)
        {
            ManagedWindows.Remove(toRemove);
            redrawWindows();
        }

        private void redrawWindows()
        {
            if (ManagedWindows.Count == 0)
                return;
            int counter = 0;
            int width = Surface.Width / ManagedWindows.Count;
            foreach (Window win in ManagedWindows)
            {
                RECT surface = new RECT(
                    width * counter + Surface.Left,
                    Surface.Top,
                    width,
                    Surface.Bottom);
                win.PlaceWindow(surface);
                counter++;
            }
        }
    }
}
