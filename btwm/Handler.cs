using System;
using System.Windows.Forms;
using System.Collections.Generic;
using static btwm.user32;
using static btwm.user32.WinEventHook;
using System.Runtime.InteropServices;
using System.IO;
using JsonStructures;

namespace btwm
{

    class Handler
    {
        public bool Running;
        public List<IntPtr> HwndBlackList;
        public Config.Configuration Configuration;
        public IntPtr FocusedWindow;
        public IntPtr LastHandledFocused;
        public Layout.LayoutType NextLayout = Layout.LayoutType.unset;

        private Dictionary<string, Workspace> workspaces;
        private List<IntPtr> handledWindows;
        private string openedWorkspace;
        private shellHookHelper shellHookHelp;
        private List<Screen> screens;
        private WinEventHook winHook;

        private Dictionary<IntPtr, string> titles;

        private StreamWriter barInput;
        private StreamReader barOutput;

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

        public Handler(StreamWriter BarInput, StreamReader BarOutput)
        {
            HwndBlackList = new List<IntPtr>();
            workspaces = new Dictionary<string, Workspace>();
            handledWindows = new List<IntPtr>();
            shellHookHelp = new shellHookHelper(this);
            Running = true;
            screens = new List<Screen>(Screen.AllScreens);
            sortScreenList(ref screens);
            openedWorkspace = "0";

            RECT surface = screens[0].Bounds;
            //TODO: read from config
            int barHeight = 17;
            surface.Top = surface.Top + barHeight;
            workspaces.Add(openedWorkspace, new Workspace(this, surface));

            Configuration = new Config.Configuration();

            barInput = BarInput;
            barOutput = BarOutput;

            /*
            JsonStructures.Workspace[] testws = new JsonStructures.Workspace[1];
            testws[0] = new JsonStructures.Workspace("lol");
            Workspaces test = new Workspaces(testws,1);
            */

            titles = new Dictionary<IntPtr, string>();

            winHook = new WinEventHook(WinEvents.EVENT_SYSTEM_FOREGROUND, WinEvents.EVENT_OBJECT_NAMECHANGE);
            winHook.Handle += eventReceiver;
        }

        public void CommandExecutor(string command)
        {
            /*
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
            */
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
                    if (!HwndBlackList.Contains(m.LParam) &&
                        !GetWindowTitle(m.LParam).StartsWith("BTWM-EXCLUDED"))
                    {
                        handledWindows.Add(m.LParam);
                        workspaces[openedWorkspace].InsertWindow(m.LParam);
                        titles.Add(m.LParam, GetWindowTitle(m.LParam));
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine('{' + m.LParam.ToString() + "} opened (\"" + titles[m.LParam] + "\")");
                        Console.ResetColor();
                        LastHandledFocused = m.LParam;
                    }
                    else
                        Console.WriteLine("Blacklisted window opened");
                    break;

                case ShellHook.ShellEvents.HSHELL_WINDOWDESTROYED:
                    if (handledWindows.Contains(m.LParam))
                    {
                        foreach (Workspace ws in workspaces.Values)
                            if (ws.ContainsWindow(m.LParam))
                                ws.RemoveWindow(m.LParam);

                        handledWindows.Remove(m.LParam);
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine('{' + m.LParam.ToString() + "} closed (\"" + titles[m.LParam] + "\")");
                        Console.ResetColor();
                        titles.Remove(m.LParam);
                    }
                    break;

                default:
                    break;
            }
        }

        /// <summary>
        /// Called whenever a WinEvent is received
        /// </summary>
        /// <param name="eventType">type of event received</param>
        /// <param name="hWnd">window concerned by event</param>
        void eventReceiver(WinEvents eventType, IntPtr hWnd)
        {
            switch (eventType)
            {
                case WinEvents.EVENT_SYSTEM_FOREGROUND:
                    if (handledWindows.Contains(hWnd))
                    {
                        if (FocusedWindow != hWnd)
                        {
                            //currentWorkspace.FocusWindow(handledWindows[hWnd]);
                            Console.ForegroundColor = ConsoleColor.Cyan;
                            Console.WriteLine('{' + hWnd.ToString() + "} focused (\"" + titles[hWnd] + "\")");
                            Console.ResetColor();
                            FocusedWindow = hWnd;
                            LastHandledFocused = hWnd;
                        }
                    }
                    else
                        FocusedWindow = (IntPtr)(-1);
                    break;
                case WinEvents.EVENT_OBJECT_NAMECHANGE:
                    if (handledWindows.Contains(hWnd))
                    {
                        string newTitle = GetWindowTitle(hWnd);
                        if (newTitle != titles[hWnd])
                        {
                            foreach (Workspace ws in workspaces.Values)
                                if (ws.ContainsWindow(hWnd))
                                    ws.WindowChangedTitle(hWnd);

                            Console.ForegroundColor = ConsoleColor.Magenta;
                            Console.WriteLine('{' + hWnd.ToString() + "} changed title from \"" + titles[hWnd] + "\" to \"" + newTitle + "\"");
                            Console.ResetColor();
                            titles[hWnd] = newTitle;
                        }
                    }
                    break;
            }
        }
    }
}
