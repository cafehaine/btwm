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

        Dictionary<string, Workspace> workspaces;
        Dictionary<IntPtr, Window> handledWindows;
        string openedWorkspace;
        shellHookHelper shellHookHelp;
        List<Screen> screens;

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
                notificationMessage = shellHook.RegisterWindowMessage(
                    "SHELLHOOK");
                shellHook.RegisterShellHookWindow(Handle);

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
                try { shellHook.DeregisterShellHookWindow(Handle); }
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
        }

        /// <summary>
        /// Called whenever an event is recieved
        /// </summary>
        /// <param name="m"></param>
        void eventReceiver(ref Message m)
        {
            // Interpret message
            switch ((shellHook.ShellEvents)m.WParam.ToInt32())
            {
                case shellHook.ShellEvents.HSHELL_WINDOWCREATED:
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("{0:x8}\tregistered", m.LParam);
                    handledWindows.Add(m.LParam, new Window(new Split(
                        new Workspace()), m.LParam));
                    workspaces[openedWorkspace].InsertWindow(
                        handledWindows[m.LParam]);
                    break;

                case shellHook.ShellEvents.HSHELL_WINDOWDESTROYED:
                    if (handledWindows.ContainsKey(m.LParam))
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("{0:x8}\tremoved", m.LParam);
                        workspaces[openedWorkspace].RemoveWindow(
                            handledWindows[m.LParam]);
                        handledWindows.Remove(m.LParam);
                    }
                    break;

                default:
                    break;
            }
        }
    }
}
