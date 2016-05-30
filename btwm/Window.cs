using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using HWND = System.IntPtr;

namespace btwm
{
    class Window : Layout
    {
        public HWND WindowHandler;
        public user32.ShowWindowCommands Status;
        public Layout parentLayout;

        public Window(Layout parentLayout, HWND windowHandler) : base(parentLayout.ParentWorkspace, LayoutType.window)
        {
            WindowHandler = windowHandler;
            //PlaceWindow(surface);
        }

        public void PlaceWindow(RECT surface)
        {
            Surface = surface;
            user32.MoveWindow(WindowHandler , surface.Left, surface.Top, surface.Right, surface.Bottom, true);
        }

        public new void Hide()
        {
            user32.ShowWindow(WindowHandler, user32.ShowWindowCommands.Hide);
            Status = user32.ShowWindowCommands.Hide;
        }

        public new void Show()
        {
            user32.ShowWindow(WindowHandler, user32.ShowWindowCommands.Show);
            Status = user32.ShowWindowCommands.Show;
        }

        public void FullscreenWindow(HWND windowHandler)
        {
            throw new NotImplementedException();
        }

        public void unFullscreenWindow(HWND windowHandler)
        {
            throw new NotImplementedException();
        }
    }
}
