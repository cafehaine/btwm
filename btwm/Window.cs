using System;
using HWND = System.IntPtr;

namespace btwm
{
    class Window
    {
        /// <summary>
        /// The Window Handle of this window
        /// </summary>
        public HWND HWnd;
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
            HWnd = windowHandler;
        }

        private void placeWindow(RECT surface)
        {
            user32.MoveWindow(HWnd , surface.Left, surface.Top,
                surface.Width, surface.Height, true);
        }

        /// <summary>
        /// Hide this window
        /// </summary>
        public void Hide()
        {
            user32.ShowWindow(HWnd, user32.ShowWindowCommands.Hide);
            Status = user32.ShowWindowCommands.Hide;
        }

        /// <summary>
        /// Show this window
        /// </summary>
        public void Show()
        {
            user32.ShowWindow(HWnd, user32.ShowWindowCommands.Show);
            Status = user32.ShowWindowCommands.Show;
        }

        //TODO: Implement
        /// <summary>
        /// Set this window to fullscreen
        /// </summary>
        public void Fullscreen()
        {
            throw new NotImplementedException();
        }

        //TODO: Implement
        /// <summary>
        /// Set this window to non-fullscreen
        /// </summary>
        public void UnFullscreenWindow()
        {
            throw new NotImplementedException();
        }

        //TODO: Implement
        /// <summary>
        /// Strip this window's borders in case of no-border mode
        /// </summary>
        public void StripBorders()
        {
            throw new NotImplementedException();
        }

        public override string ToString()
        {
            return "{HWnd=" + HWnd.ToString() + "}";
        }

        public override bool Equals(object obj)
        { return obj.GetType() == typeof(Window) ? (obj as Window).HWnd == HWnd : false; }
    }
}
