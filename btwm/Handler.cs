using System;
using System.Windows.Forms;
using System.Collections.Generic;
using btwm.Layouts;

namespace btwm
{

    class Handler
    {
        Dictionary<string, Workspace> Workspaces;
        Dictionary<IntPtr, Window> HandledWindows;
        string openedWorkspace;
        ShellHookHelper shellHookHelper;
        public bool running;
        List<Screen> screens;

        private class ShellHookHelper : Form
        {
            private int notificationMessage;
            private Handler parentHandler;
            public ShellHookHelper(Handler parentHandler) : base()
            {
                this.parentHandler = parentHandler;
                notificationMessage = user32.shellHook.RegisterWindowMessage("SHELLHOOK");
                user32.shellHook.RegisterShellHookWindow(this.Handle);
            }

            /// <summary>
            /// This function is triggered by shell hooks
            /// </summary>
            /// <param name="m"></param>
            protected override void WndProc(ref Message m)
            {
                if (m.Msg == notificationMessage)
                {
                    // Receive shell messages
                    parentHandler.eventReceiver(ref m);
                }
                base.WndProc(ref m);
            }

            /// <summary>
            /// This is executed when the form is destroyed
            /// </summary>
            /// <param name="disposing"></param>
            protected override void Dispose(bool disposing)
            {
                try { user32.shellHook.DeregisterShellHookWindow(this.Handle); }
                catch { }
                base.Dispose(disposing);
            }
        }

        Workspace CurrentWorkspace
        {
            get { return Workspaces[openedWorkspace]; }
        }

        /// <summary>
        /// Make sure "primary" screen is the first one in the list
        /// </summary>
        /// <param name="list"></param>
        private void SortScreenList(ref List<Screen> list)
        {
            if (!list[0].Primary)
            {
                Screen primaryScreen = list.Find(screen => screen.Primary);
                list.Remove(primaryScreen);
                list.Insert(0, primaryScreen);
            }
        }

        public Handler()
        {
            Workspaces = new Dictionary<string, Workspace>();
            HandledWindows = new Dictionary<IntPtr, Window>();
            shellHookHelper = new ShellHookHelper(this);
            running = true;
            screens = new List<Screen>(Screen.AllScreens);
            SortScreenList(ref screens);
            openedWorkspace = "0";
            Workspaces.Add(openedWorkspace, new Workspace());
        }

        public void eventReceiver(ref Message m)
        {
            // Interpret message
            switch ((user32.shellHook.ShellEvents)m.WParam.ToInt32())
            {
                case user32.shellHook.ShellEvents.HSHELL_WINDOWCREATED:
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("{0:x8}\tregistered", m.LParam);
                    HandledWindows.Add(m.LParam, new Window(new Split(new Workspace()), m.LParam));
                    Workspaces[openedWorkspace].InsertWindow(HandledWindows[m.LParam]);
                    break;

                case user32.shellHook.ShellEvents.HSHELL_WINDOWDESTROYED:
                    if (HandledWindows.ContainsKey(m.LParam))
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("{0:x8}\tremoved", m.LParam);
                        Workspaces[openedWorkspace].RemoveWindow(HandledWindows[m.LParam]);
                        HandledWindows.Remove(m.LParam);
                    }
                    break;

                default:
                    break;
            }
        }
    }
}
