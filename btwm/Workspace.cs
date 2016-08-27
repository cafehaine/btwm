using btwm.Layouts;
using System;
using System.Collections.Generic;

namespace btwm
{
    /// <summary>
    /// A workspace is an equivalent to a virtual destop, it contains layouts.
    /// </summary>
    class Workspace : Container
    {
        private Layout BaseContainer;
        private Layout.LayoutType nextLayout = Layout.LayoutType.tabbed;
        private List<IntPtr> handledWindows;
        private List<Window> Floating;

        public Workspace(Handler handler, RECT surface) : base(ContainerTypes.Workspace, handler, surface)
        {
            CanContainContainers = true;
            CanContainWindows = true;
            Floating = new List<Window>();
            handledWindows = new List<IntPtr>();
            BaseContainer = null;
        }

        public override void SetNextLayout(IntPtr focusedWindow, Layout.LayoutType nextLayout)
        {
            if (BaseContainer == null)
                this.nextLayout = nextLayout;
            else if (BaseContainer.ContainsWindow(focusedWindow))
                BaseContainer.SetNextLayout(focusedWindow, nextLayout);
        }

        public override void WindowChangedTitle(IntPtr x)
        {
            if (BaseContainer.ContainsWindow(x))
                BaseContainer.WindowChangedTitle(x);
        }

        public override void Show()
        {
            BaseContainer.Show();
            Floating.ForEach(w => w.Show());
        }

        public override void Hide()
        {
            Floating.ForEach(w => w.Hide());
            BaseContainer.Hide();
        }

        public override bool ContainsWindow(IntPtr x)
        {
            return handledWindows.Contains(x);
        }

        public override void InsertWindow(IntPtr win)
        {
            handledWindows.Add(win);
            if (MainHandler.Configuration.ShouldBeFloating(win))
                Floating.Add(new Window(win));
            else
            {
                if (BaseContainer != null)
                    BaseContainer.InsertWindow(win);
                else
                {
                    switch (nextLayout)
                    {
                        case Layout.LayoutType.splith:
                            BaseContainer = new Split(MainHandler, surface, this, true);
                            break;
                        case Layout.LayoutType.splitv:
                            BaseContainer = new Split(MainHandler, surface, this, false);
                            break;
                        case Layout.LayoutType.stacking:
                            throw new NotImplementedException();
                        case Layout.LayoutType.tabbed:
                            BaseContainer = new Htabbed(MainHandler, surface, this);
                            break;
                        case Layout.LayoutType.vtabbed:
                            BaseContainer = new Vtabbed(MainHandler, surface, this);
                            break;
                        case Layout.LayoutType.unset:
                            throw new Exception("Unset layout is impossibru, wtf man.");
                    }
                    BaseContainer.InsertWindow(win);
                }
            }
        }

        public override void RemoveWindow(IntPtr toRemove)
        {
            int index = Floating.FindIndex(w => w.HWnd == toRemove);

            if (index == -1)
                BaseContainer.RemoveWindow(toRemove);
            else
                Floating.RemoveAt(index);

            handledWindows.Remove(toRemove);

            if (handledWindows.Count == 0)
            {
                BaseContainer.Delete();
                BaseContainer = null;
            }
        }
    }
}
