using btwm.Layouts;
using System;

namespace btwm
{
    /// <summary>
    /// A workspace is an equivalent to a virtual destop, it contains layout.
    /// </summary>
    class Workspace
    {
        public RECT Surface = new RECT(0, 0, 1920, 1080);
        public Layout Layout;

        public Workspace()
        {
            Surface = new RECT(0, 0, 1920, 1080);
            Layout = new Split(this);
        }

        /// <summary>
        /// Show the content of the workspace
        /// </summary>
        public void Show()
        {
            Layout.Show();
        }

        /// <summary>
        /// Hide the content of a workspace
        /// </summary>
        public void Hide()
        {
            Layout.Hide();
        }

        /// <summary>
        /// Insert a window in this worspace
        /// </summary>
        /// <param name="newWin"></param>
        public void InsertWindow(Window newWin)
        {
            Layout.InsertWindow(newWin);
        }

        /// <summary>
        /// Rewove a window from this worspace
        /// </summary>
        /// <param name="toRemove"></param>
        public void RemoveWindow(Window toRemove)
        {
            Layout.RemoveWindow(toRemove);
        }

        public void FocusWindow(Window hwnd)
        { Layout.FocusWindow(hwnd); }
    }
}
