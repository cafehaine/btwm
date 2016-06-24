using System;
using HWND = System.IntPtr;

namespace btwm
{
    class Window
    {
        /// <summary>
        /// The Window Handle of this window
        /// </summary>
        public HWND WindowHandler;
        /// <summary>
        /// The status of this window
        /// </summary>
        public user32.ShowWindowCommands Status;
        /// <summary>
        /// The layout this window is contained in
        /// </summary>
        public Layout ParentLayout;

        /// <summary>
        /// This window's display surface
        /// </summary>
        private RECT surface;
        /// <summary>
        /// The window's display surface
        /// </summary>
        public RECT Surface
        {
            get
            {
                return surface;
            }

            set
            {
                surface = value;
                placeWindow(value);
            }
        }

        public Window(Layout parentLayout, HWND windowHandler)
        {
            WindowHandler = windowHandler;
        }

        private void placeWindow(RECT surface)
        {
            user32.MoveWindow(WindowHandler , surface.Left, surface.Top,
                surface.Right, surface.Bottom, true);
        }

        /// <summary>
        /// Hide this window
        /// </summary>
        public void Hide()
        {
            user32.ShowWindow(WindowHandler, user32.ShowWindowCommands.Hide);
            Status = user32.ShowWindowCommands.Hide;
        }

        /// <summary>
        /// Show this window
        /// </summary>
        public void Show()
        {
            user32.ShowWindow(WindowHandler, user32.ShowWindowCommands.Show);
            Status = user32.ShowWindowCommands.Show;
        }

        /// <summary>
        /// Set this window to fullscreen
        /// </summary>
        public void Fullscreen()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Set this window to non-fullscreen
        /// </summary>
        public void UnFullscreenWindow()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Strip this window's borders in case of no-border mode
        /// </summary>
        public void StripBorders()
        {
            throw new NotImplementedException();
        }
    }
}
