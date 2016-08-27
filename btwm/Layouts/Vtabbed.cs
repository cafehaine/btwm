using btwm.Layouts.Forms;
using System;
using System.Collections.Generic;

namespace btwm.Layouts
{
    class Vtabbed : Layout
    {
        public override bool ContainsWindow(IntPtr x)
        {
            return handledWindows.Contains(x);
        }

        public override void Hide()
        {
            formWindow.Hide();
            windows[displayedWindow].Hide();
        }

        public override void Show()
        {
            formWindow.Show();
            windows[displayedWindow].Hide();
        }

        public override void InsertWindow(IntPtr x)
        {
            handledWindows.Add(x);
            windows.Insert(displayedWindow + 1, new Window(x));
            displayedWindow += 1;

            if (displayedWindow >= windows.Count)
                displayedWindow = windows.Count - 1;

            updateForm();
            RepositionWindows();
        }

        public override void RemoveWindow(IntPtr x)
        {
            int index = windows.FindIndex(w => w.HWnd == x);
            if (index != -1)
                windows.RemoveAt(index);
            else
                throw new Exception("Tried to remove a window from a container that did not contain it");

            if (index >= windows.Count)
                index = windows.Count - 1;

            displayedWindow--;

            updateForm();
            RepositionWindows();
        }

        public override void InsertContainer(Container x)
        {
            throw new InvalidOperationException("Cannot insert a container in a vtabbed layout.");
        }

        public override void RemoveContainer(Container x)
        {
            throw new InvalidOperationException("Cannot remove a container from a vtabbed layout.");
        }

        public override void FocusWindow(IntPtr x)
        {
            // If window isn't found, will return -1, which the form interprets
            // as "no window selected".
            int index = handledWindows.FindIndex(hwnd => hwnd == x);
            displayedWindow = index;
            updateForm();
            RepositionWindows();
        }

        public override void WindowChangedTitle(IntPtr x)
        {
            updateForm();
        }

        private List<IntPtr> handledWindows;
        private List<Window> windows;
        private int displayedWindow;
        private vtabbedForm form = null;
        private Window formWindow;
        private RECT formSurface;
        private RECT windowSurface;

        private void updateForm()
        {
            form.Index = displayedWindow;
            string[] titles = new string[windows.Count];
            for (int i = 0; i < windows.Count; i++)
                titles[i] = windows[i].Title;
            form.Titles = titles;
        }

        public override void RepositionWindows()
        {
            if (displayedWindow == -1 || windows.Count == 0)
                return;

            for (int i = 0; i < windows.Count; i++)
            {
                if (i != displayedWindow)
                    windows[i].Hide();
                else
                    windows[i].Show();
            }

            windows[displayedWindow].Surface = windowSurface;
        }

        public Vtabbed(Handler handler, RECT surface, Container parent) : base(handler, surface, LayoutType.vtabbed, parent)
        {
            handledWindows = new List<IntPtr>();
            windows = new List<Window>();
            displayedWindow = -1;

            CanContainContainers = false;
            CanContainWindows = true;
            formSurface = surface;
            formSurface.Width = formSurface.Width / 10;
            windowSurface = new RECT(formSurface.Right, formSurface.Top, surface.Right, formSurface.Bottom);
            form = new vtabbedForm(new Config.Configuration(), formSurface);
            MainHandler.HwndBlackList.Add(form.Handle);
            form.Show();
            formWindow = new Window(form.Handle);
        }

        protected override void surfaceChanged()
        {
            if (form != null)
            {
                formSurface = surface;
                formSurface.Width = formSurface.Width / 10;
                windowSurface = new RECT(formSurface.Right, formSurface.Top, surface.Right, formSurface.Bottom);
                form.Surface = formSurface;
                RepositionWindows();
            }
        }
    }
}
