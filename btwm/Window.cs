using System;

namespace btwm
{
    class Window : Container
    {
        /// <summary>
        /// The Window Handle of this window
        /// </summary>
		public IntPtr HWnd;

        private Layout.LayoutType nextLayout = Layout.LayoutType.unset;

        public override void WindowChangedTitle(IntPtr x) { }

        public override void FocusWindow(IntPtr x) { }

        public override void InsertContainer(Container x) { throw new InvalidOperationException(); }

        public override void InsertWindow(IntPtr x) { throw new InvalidOperationException(); }

        public override void RemoveContainer(Container x) { throw new InvalidOperationException(); }

        public override void RemoveWindow(IntPtr x) { throw new InvalidOperationException(); }

        public override void SetNextLayout(IntPtr focusedWindow, Layout.LayoutType nextLayout)
        {
            if (focusedWindow == HWnd)
                this.nextLayout = nextLayout;
        }

        public override bool ContainsWindow(IntPtr x)
        {
            return x == HWnd;
        }

        protected override void surfaceChanged()
        {
            placeWindow(surface);
        }

		public Window(IntPtr windowHandler) : base(ContainerTypes.Window)
        {
            CanContainContainers = false;
            CanContainWindows = false;
            HWnd = windowHandler;
        }

        private void placeWindow(RECT surface)
        {
            user32.MoveWindow(HWnd , surface.Left, surface.Top,
                surface.Width, surface.Height, true);
        }

        public override void Show()
        {
            user32.ShowWindow(HWnd, user32.ShowWindowCommands.Restore);
        }

        public override void Hide()
        {
            user32.ShowWindow(HWnd, user32.ShowWindowCommands.Minimize);
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

        public string Title { get { return user32.GetWindowTitle(HWnd); } }

        public override string ToString()
        {
            return "{HWnd=" + HWnd.ToString() + "}";
        }

        public override bool Equals(object obj)
        { return obj.GetType() == typeof(Window) ? (obj as Window).HWnd == HWnd : false; }

        public override int GetHashCode()
        { return HWnd.ToInt32(); }

        public static bool operator ==(Window w1, Window w2)
        {
            return w1.Equals(w2);
        }

        public static bool operator !=(Window w1, Window w2)
        {
            return !w1.Equals(w2);
        }
    }
}
