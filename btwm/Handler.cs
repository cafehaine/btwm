using System;
using System.Windows.Forms;
using System.Collections.Generic;
using btwm.Layouts;
using static btwm.user32;
using System.Runtime.InteropServices;

namespace btwm
{

    class Handler
    {
        public bool Running;

        private Dictionary<string, Workspace> workspaces;
        private Dictionary<IntPtr, Window> handledWindows;
        private string openedWorkspace;
        private shellHookHelper shellHookHelp;
        private List<Screen> screens;
        private WinEventHook winHook;
        private IntPtr focusedWindow;

        private class shellHookHelper : Form
        {
            private int notificationMessage;
            private Handler parentHandler;

            public shellHookHelper(Handler parentHandler) : base()
            {
                // Minimized metrics. This is required in order to use
                // shellhooks without explorer.exe
                SetTaskmanWindow(Handle);
                this.parentHandler = parentHandler;
                notificationMessage = ShellHook.RegisterWindowMessage(
                    "SHELLHOOK");
                ShellHook.RegisterShellHookWindow(Handle);

                MinimizedMetrics mm = new MinimizedMetrics
                {
                    cbSize = (uint)Marshal.SizeOf(typeof(MinimizedMetrics))
                };

                IntPtr mmPtr = Marshal.AllocHGlobal(Marshal.SizeOf(
                    typeof(MinimizedMetrics)));

                try
                {
                    Marshal.StructureToPtr(mm, mmPtr, true);
                    SystemParametersInfo(SPI.SPI_GETMINIMIZEDMETRICS, mm.cbSize,
                        mmPtr, SPIF.None);

                    mm.iArrange |= MinimizedMetricsArrangement.Hide;
                    Marshal.StructureToPtr(mm, mmPtr, true);
                    SystemParametersInfo(SPI.SPI_SETMINIMIZEDMETRICS, mm.cbSize,
                        mmPtr, SPIF.None);
                }
                finally
                {
                    Marshal.DestroyStructure(mmPtr, typeof(MinimizedMetrics));
                    Marshal.FreeHGlobal(mmPtr);
                }

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
                try { ShellHook.DeregisterShellHookWindow(Handle); }
                catch { }
                base.Dispose(disposing);
            }
        }

        Workspace currentWorkspace
        {
            get { return workspaces[openedWorkspace]; }
        }

        /// <summary>
        /// Make sure "primary" screen is the first one in the list
        /// </summary>
        /// <param name="list"></param>
        private void sortScreenList(ref List<Screen> list)
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
            workspaces = new Dictionary<string, Workspace>();
            handledWindows = new Dictionary<IntPtr, Window>();
            shellHookHelp = new shellHookHelper(this);
            Running = true;
            screens = new List<Screen>(Screen.AllScreens);
            sortScreenList(ref screens);
            openedWorkspace = "0";
            workspaces.Add(openedWorkspace, new Workspace());

            winHook = new WinEventHook(WinEventHook.WinEvents.EVENT_SYSTEM_FOREGROUND, WinEventHook.WinEvents.EVENT_SYSTEM_FOREGROUND);
            winHook.Handle += eventReceiver;
        }

        public void CommandExecutor(string command)
        {
            switch (command)
            {
                case "splith":
                    if (workspaces[openedWorkspace].Layout.Type ==
                        Layout.LayoutType.split)
                        (workspaces[openedWorkspace].Layout as Split).NextSplit(
                            WindowTree.type.splith);
                    break;
                case "splitv":
                    if (workspaces[openedWorkspace].Layout.Type ==
                        Layout.LayoutType.split)
                        (workspaces[openedWorkspace].Layout as Split).NextSplit(
                            WindowTree.type.splitv);
                    break;
            }
        }

        /// <summary>
        /// Called whenever a shell event is received
        /// </summary>
        /// <param name="m"></param>
        void eventReceiver(ref Message m)
        {
            // Interpret message
            switch ((ShellHook.ShellEvents)m.WParam.ToInt32())
            {
                case ShellHook.ShellEvents.HSHELL_WINDOWCREATED:
                    handledWindows.Add(m.LParam, new Window(new Split(
                        new Workspace()), m.LParam));
                    workspaces[openedWorkspace].InsertWindow(
                        handledWindows[m.LParam]);
                    focusedWindow = m.LParam;
                    currentWorkspace.FocusWindow(handledWindows[m.LParam]);
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine(handledWindows[m.LParam].ToString() + " opened");
                    Console.ResetColor();
                    break;

                case ShellHook.ShellEvents.HSHELL_WINDOWDESTROYED:
                    if (handledWindows.ContainsKey(m.LParam))
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine(handledWindows[m.LParam].ToString() + " closed");
                        Console.ResetColor();
                        workspaces[openedWorkspace].RemoveWindow(
                            handledWindows[m.LParam]);
                        handledWindows.Remove(m.LParam);
                    }
                    break;

                default:
                    break;
            }
        }

        /// <summary>
        /// Called whenever a WinEvent is received
        /// </summary>
        /// <param name="hWinEventHook"></param>
        /// <param name="iEvent"></param>
        /// <param name="hWnd"></param>
        /// <param name="idObject"></param>
        /// <param name="idChild"></param>
        /// <param name="dwEventThread"></param>
        /// <param name="dwmsEventTime"></param>
        void eventReceiver(WinEventHook.WinEvents eventType, IntPtr hWnd)
        {
            if (handledWindows.ContainsKey(hWnd))
                if (focusedWindow != hWnd)
                {
                    currentWorkspace.FocusWindow(handledWindows[hWnd]);
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.WriteLine(handledWindows[hWnd].ToString() + " focused");
                    Console.ResetColor();
                    focusedWindow = hWnd;
                }
        }
    }
}
