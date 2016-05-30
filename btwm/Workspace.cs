using btwm.Layouts;
using System;

namespace btwm
{
    class Workspace
    {
        public RECT surface = new RECT(0, 0, 1920, 1080);
        private Layout layout;

        public Workspace()
        {
            surface = new RECT(0, 0, 1920, 1080);
            layout = new Split(this);
        }

        public void Show()
        {
            layout.Show();
        }

        public void Hide()
        {
            layout.Hide();
        }

        public Layout Layout
        {
            get { return layout; }
            set { layout = value; }
        }

        public void InsertWindow(Window newWin)
        {
            layout.InsertWindow(newWin);
        }

        public void RemoveWindow(Window toRemove)
        {
            layout.RemoveWindow(toRemove);
        }
    }
}
