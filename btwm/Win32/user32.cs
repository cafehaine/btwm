using System;
using System.Runtime.InteropServices;
using System.Text;

namespace btwm
{
    static class user32
    {
        /// <summary>
        /// Set the window used as TaskMan (explorer.exe)
        /// </summary>
        /// <param name="hWnd"></param>
        /// <returns></returns>
        [DllImport("user32.dll")]
        public static extern bool SetTaskmanWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        public static extern bool SystemParametersInfo(SPI uiAction,
            uint uiParam, IntPtr pvParam, SPIF fWinIni);

        [Flags]
        public enum SPIF
        {
            None = 0x00,
            SPIF_UPDATEINIFILE = 0x01,
            SPIF_SENDCHANGE = 0x02,
            SPIF_SENDWININICHANGE = 0x02
        }

        public enum SPI : uint
        {
            SPI_GETMINIMIZEDMETRICS = 0x002B,
            SPI_SETMINIMIZEDMETRICS = 0x002C
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct MinimizedMetrics
        {
            public uint cbSize;
            public int iWidth;
            public int iHorzGap;
            public int iVertGap;
            public MinimizedMetricsArrangement iArrange;
        }

        [Flags]
        public enum MinimizedMetricsArrangement
        {
            BottomLeft = 0,
            BottomRight = 1,
            TopLeft = 2,
            TopRight = 3,
            Left = 0,
            Right = 0,
            Up = 4,
            Down = 4,
            Hide = 8
        }

        public static class shellHook
        {
            /// <summary>
            /// The different kinds of shell hooks
            /// </summary>
            public enum ShellEvents : int
            {
                /// <summary>
                /// A window was created
                /// </summary>
                HSHELL_WINDOWCREATED = 1,
                /// <summary>
                /// A window was destroyed
                /// </summary>
                HSHELL_WINDOWDESTROYED = 2,
                HSHELL_ACTIVATESHELLWINDOW = 3,
                HSHELL_WINDOWACTIVATED = 4,
                HSHELL_GETMINRECT = 5,
                HSHELL_REDRAW = 6,
                HSHELL_TASKMAN = 7,
                HSHELL_LANGUAGE = 8,
                HSHELL_ACCESSIBILITYSTATE = 11,
                HSHELL_APPCOMMAND = 12
            }
            [DllImport("user32.dll", EntryPoint = "RegisterWindowMessageA",
                CharSet = CharSet.Ansi, SetLastError = true,
                ExactSpelling = true)]
            public static extern int RegisterWindowMessage(string lpString);

            [DllImport("user32", CharSet = CharSet.Ansi, SetLastError = true,
                ExactSpelling = true)]
            public static extern int DeregisterShellHookWindow(IntPtr hWnd);

            [DllImport("user32", CharSet = CharSet.Ansi, SetLastError = true,
                ExactSpelling = true)]
            public static extern int RegisterShellHookWindow(IntPtr hWnd);
        }

        #region Hooks

        #region Hooks codes
        /// <summary>
        /// Events are ASYNC
        /// </summary>
        public const uint WINEVENT_OUTOFCONTEXT = 0x0000;
        /// <summary>
        /// Don't call back for events on installer's thread
        /// </summary>
        public const uint WINEVENT_SKIPOWNTHREAD = 0x0001;
        /// <summary>
        /// Don't call back for events on installer's process
        /// </summary>
        public const uint WINEVENT_SKIPOWNPROCESS = 0x0002;
        /// <summary>
        /// Events are SYNC, this causes your dll to be injected into every process
        /// </summary>
        public const uint WINEVENT_INCONTEXT = 0x0004;
        /// <summary>
        /// Smallest event possible
        /// </summary>
        public const uint EVENT_MIN = 0x00000001;
        /// <summary>
        /// Biggest event possible
        /// </summary>
        public const uint EVENT_MAX = 0x7FFFFFFF;

        public const uint EVENT_SYSTEM_SOUND = 0x0001;

        public const uint EVENT_SYSTEM_ALERT = 0x0002;

        public const uint EVENT_SYSTEM_FOREGROUND = 0x0003;

        public const uint EVENT_SYSTEM_MENUSTART = 0x0004;

        public const uint EVENT_SYSTEM_MENUEND = 0x0005;

        public const uint EVENT_SYSTEM_MENUPOPUPSTART = 0x0006;

        public const uint EVENT_SYSTEM_MENUPOPUPEND = 0x0007;

        public const uint EVENT_SYSTEM_CAPTURESTART = 0x0008;

        public const uint EVENT_SYSTEM_CAPTUREEND = 0x0009;

        public const uint EVENT_SYSTEM_MOVESIZESTART = 0x000A;

        public const uint EVENT_SYSTEM_MOVESIZEEND = 0x000B;

        public const uint EVENT_SYSTEM_CONTEXTHELPSTART = 0x000C;

        public const uint EVENT_SYSTEM_CONTEXTHELPEND = 0x000D;

        public const uint EVENT_SYSTEM_DRAGDROPSTART = 0x000E;

        public const uint EVENT_SYSTEM_DRAGDROPEND = 0x000F;

        public const uint EVENT_SYSTEM_DIALOGSTART = 0x0010;

        public const uint EVENT_SYSTEM_DIALOGEND = 0x0011;

        public const uint EVENT_SYSTEM_SCROLLINGSTART = 0x0012;

        public const uint EVENT_SYSTEM_SCROLLINGEND = 0x0013;

        public const uint EVENT_SYSTEM_SWITCHSTART = 0x0014;

        public const uint EVENT_SYSTEM_SWITCHEND = 0x0015;

        public const uint EVENT_SYSTEM_MINIMIZESTART = 0x0016;

        public const uint EVENT_SYSTEM_MINIMIZEEND = 0x0017;

        public const uint EVENT_SYSTEM_DESKTOPSWITCH = 0x0020;

        public const uint EVENT_SYSTEM_END = 0x00FF;

        public const uint EVENT_OEM_DEFINED_START = 0x0101;

        public const uint EVENT_OEM_DEFINED_END = 0x01FF;
        /// <summary>
        /// Related to UI automation
        /// </summary>
        public const uint EVENT_UIA_EVENTID_START = 0x4E00;
        /// <summary>
        /// Related to UI automation
        /// </summary>
        public const uint EVENT_UIA_EVENTID_END = 0x4EFF;
        /// <summary>
        /// Related to UI automation
        /// </summary>
        public const uint EVENT_UIA_PROPID_START = 0x7500;
        /// <summary>
        /// Related to UI automation
        /// </summary>
        public const uint EVENT_UIA_PROPID_END = 0x75FF;

        public const uint EVENT_CONSOLE_CARET = 0x4001;

        public const uint EVENT_CONSOLE_UPDATE_REGION = 0x4002;

        public const uint EVENT_CONSOLE_UPDATE_SIMPLE = 0x4003;

        public const uint EVENT_CONSOLE_UPDATE_SCROLL = 0x4004;

        public const uint EVENT_CONSOLE_LAYOUT = 0x4005;

        public const uint EVENT_CONSOLE_START_APPLICATION = 0x4006;

        public const uint EVENT_CONSOLE_END_APPLICATION = 0x4007;

        public const uint EVENT_CONSOLE_END = 0x40FF;
        /// <summary>
        /// hwnd ID idChild is created item
        /// </summary>
        public const uint EVENT_OBJECT_CREATE = 0x8000;
        /// <summary>
        /// hwnd ID idChild is destroyed item
        /// </summary>
        public const uint EVENT_OBJECT_DESTROY = 0x8001;
        /// <summary>
        /// hwnd ID idChild is shown item
        /// </summary>
        public const uint EVENT_OBJECT_SHOW = 0x8002;
        /// <summary>
        /// hwnd ID idChild is hidden item
        /// </summary>
        public const uint EVENT_OBJECT_HIDE = 0x8003;
        /// <summary>
        /// hwnd ID idChild is parent of zordering children
        /// </summary>
        public const uint EVENT_OBJECT_REORDER = 0x8004;
        /// <summary>
        /// hwnd ID idChild is focused item
        /// </summary>
        public const uint EVENT_OBJECT_FOCUS = 0x8005;
        /// <summary>
        /// hwnd ID idChild is selected item (if only one), or idChild is OBJID_WINDOW if complex
        /// </summary>
        public const uint EVENT_OBJECT_SELECTION = 0x8006;
        /// <summary>
        /// hwnd ID idChild is item added
        /// </summary>
        public const uint EVENT_OBJECT_SELECTIONADD = 0x8007;
        /// <summary>
        /// hwnd ID idChild is item removed
        /// </summary>
        public const uint EVENT_OBJECT_SELECTIONREMOVE = 0x8008;
        /// <summary>
        /// hwnd ID idChild is parent of changed selected items
        /// </summary>
        public const uint EVENT_OBJECT_SELECTIONWITHIN = 0x8009;
        /// <summary>
        /// hwnd ID idChild is item w/ state change
        /// </summary>
        public const uint EVENT_OBJECT_STATECHANGE = 0x800A;
        /// <summary>
        /// hwnd ID idChild is moved/sized item
        /// </summary>
        public const uint EVENT_OBJECT_LOCATIONCHANGE = 0x800B;
        /// <summary>
        /// hwnd ID idChild is item w/ name change
        /// </summary>
        public const uint EVENT_OBJECT_NAMECHANGE = 0x800C;
        /// <summary>
        /// hwnd ID idChild is item w/ desc change
        /// </summary>
        public const uint EVENT_OBJECT_DESCRIPTIONCHANGE = 0x800D;
        /// <summary>
        /// hwnd ID idChild is item w/ value change
        /// </summary>
        public const uint EVENT_OBJECT_VALUECHANGE = 0x800E;
        /// <summary>
        /// hwnd ID idChild is item w/ new parent
        /// </summary>
        public const uint EVENT_OBJECT_PARENTCHANGE = 0x800F;
        /// <summary>
        /// hwnd ID idChild is item w/ help change
        /// </summary>
        public const uint EVENT_OBJECT_HELPCHANGE = 0x8010;
        /// <summary>
        /// hwnd ID idChild is item w/ def action change
        /// </summary>
        public const uint EVENT_OBJECT_DEFACTIONCHANGE = 0x8011;
        /// <summary>
        /// hwnd ID idChild is item w/ keybd accel change
        /// </summary>
        public const uint EVENT_OBJECT_ACCELERATORCHANGE = 0x8012;
        /// <summary>
        /// hwnd ID idChild is item invoked
        /// </summary>
        public const uint EVENT_OBJECT_INVOKED = 0x8013;
        /// <summary>
        /// hwnd ID idChild is item w? test selection change
        /// </summary>
        public const uint EVENT_OBJECT_TEXTSELECTIONCHANGED = 0x8014;

        public const uint EVENT_OBJECT_CONTENTSCROLLED = 0x8015;

        public const uint EVENT_SYSTEM_ARRANGMENTPREVIEW = 0x8016;

        public const uint EVENT_OBJECT_END = 0x80FF;

        public const uint EVENT_AIA_START = 0xA000;

        public const uint EVENT_AIA_END = 0xAFFF;
        #endregion

        public delegate void WinEventDelegate(IntPtr hWinEventHook,
            uint eventType, IntPtr hwnd, int idObject, int idChild,
            uint dwEventThread, uint dwmsEventTime);

        [DllImport("user32.dll")]
        public static extern IntPtr SetWinEventHook(uint eventMin,
            uint eventMax, IntPtr hmodWinEventProc,
            WinEventDelegate lpfnWinEventProc, uint idProcess, uint idThread,
            uint dwFlags);

        [DllImport("user32.dll")]
        static extern bool UnhookWinEvent(IntPtr hWinEventHook);

        #endregion

        #region Window operations

        [StructLayout(LayoutKind.Sequential)]
        public struct WINDOWINFO
        {
            public uint cbSize;
            public RECT rcWindow;
            public RECT rcClient;
            public uint dwStyle;
            public uint dwExStyle;
            public uint dwWindowStatus;
            public uint cxWindowBorders;
            public uint cyWindowBorders;
            public ushort atomWindowType;
            public ushort wCreatorVersion;

            // Allows automatic initialization of "cbSize" with
            // "new WINDOWINFO(null/true/false)".
            public WINDOWINFO(bool? filler) : this()
            {
                cbSize = (uint)(Marshal.SizeOf(typeof(WINDOWINFO)));
            }

        }

        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool GetWindowInfo(IntPtr hwnd,
            ref WINDOWINFO pwi);

        /// <summary>
        /// Send the window's title to the StringBuilder
        /// </summary>
        /// <param name="hWnd"></param>
        /// <param name="lpString"></param>
        /// <param name="nMaxCount"></param>
        /// <returns></returns>
        [DllImport("USER32.DLL")]
        public static extern int GetWindowText(IntPtr hWnd,
            StringBuilder lpString, int nMaxCount);

        /// <summary>
        /// Get the lenght of a window's title
        /// </summary>
        /// <param name="hWnd"></param>
        /// <returns></returns>
        [DllImport("USER32.DLL")]
        public static extern int GetWindowTextLength(IntPtr hWnd);

        /// <summary>
        /// Helper function. Return a string containing the handled window's
        /// title.
        /// </summary>
        /// <param name="hWnd">The window handler</param>
        /// <returns>Handled window's title.</returns>
        public static string GetWindowTitle(IntPtr hWnd)
        {
            int length = GetWindowTextLength(hWnd);
            StringBuilder builder = new StringBuilder(length);
            GetWindowText(hWnd, builder, length + 1);
            return builder.ToString();
        }

        /// <summary>
        /// Returns true if the window is displayed
        /// </summary>
        /// <param name="hWnd"></param>
        /// <returns></returns>
        [DllImport("USER32.DLL")]
        public static extern bool IsWindowVisible(IntPtr hWnd);

        /// <summary>
        /// Return the windowHandle of the desktop's window
        /// (usually explorer.exe)
        /// </summary>
        /// <returns></returns>
        [DllImport("USER32.DLL")]
        public static extern IntPtr GetShellWindow();

        /// <summary>
        /// Command to send to a window
        /// </summary>
        public enum ShowWindowCommands
        {
            /// <summary>
            /// Hides the window and activates another window.
            /// </summary>
            Hide = 0,
            /// <summary>
            /// Activates and displays a window. If the window is minimized or
            /// maximized, the system restores it to its original size and
            /// position. An application should specify this flag when
            /// displaying the window for the first time.
            /// </summary>
            Normal = 1,
            /// <summary>
            /// Activates the window and displays it as a minimized window.
            /// </summary>
            ShowMinimized = 2,
            /// <summary>
            /// Maximizes the specified window.
            /// </summary>
            Maximize = 3, // is this the right value?
            /// <summary>
            /// Activates the window and displays it as a maximized window.
            /// </summary>
            ShowMaximized = 3,
            /// <summary>
            /// Displays a window in its most recent size and position. This
            /// value is similar to
            /// <see cref="Win32.ShowWindowCommand.Normal"/>, except the window
            /// is not activated.
            /// </summary>
            ShowNoActivate = 4,
            /// <summary>
            /// Activates the window and displays it in its current size and
            /// position.
            /// </summary>
            Show = 5,
            /// <summary>
            /// Minimizes the specified window and activates the next top-level
            /// window in the Z order.
            /// </summary>
            Minimize = 6,
            /// <summary>
            /// Displays the window as a minimized window. This value is similar
            /// to <see cref="Win32.ShowWindowCommand.ShowMinimized"/>, except
            /// the window is not activated.
            /// </summary>
            ShowMinNoActive = 7,
            /// <summary>
            /// Displays the window in its current size and position. This value
            /// is similar to <see cref="Win32.ShowWindowCommand.Show"/>, except
            /// the window is not activated.
            /// </summary>
            ShowNA = 8,
            /// <summary>
            /// Activates and displays the window. If the window is minimized or
            /// maximized, the system restores it to its original size and
            /// position. An application should specify this flag when restoring
            /// a minimized window.
            /// </summary>
            Restore = 9,
            /// <summary>
            /// Sets the show state based on the SW_* value specified in the
            /// STARTUPINFO structure passed to the CreateProcess function by
            /// the program that started the application.
            /// </summary>
            ShowDefault = 10,
            /// <summary>
            ///  <b>Windows 2000/XP:</b> Minimizes a window, even if the thread
            /// that owns the window is not responding. This flag should only be
            /// used when minimizing windows from a different thread.
            /// </summary>
            ForceMinimize = 11
        }

        /// <summary>
        /// Move a window to a new specified surface
        /// </summary>
        /// <param name="hWnd"></param>
        /// <param name="X"></param>
        /// <param name="Y"></param>
        /// <param name="nWidth"></param>
        /// <param name="nHeight"></param>
        /// <param name="bRepaint">Should the window be repainted after</param>
        /// <returns></returns>
        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool MoveWindow(IntPtr hWnd, int X, int Y,
            int nWidth, int nHeight, bool bRepaint);

        /// <summary>
        /// Send a command to a Window
        /// </summary>
        /// <param name="hWnd"></param>
        /// <param name="nCmdShow"></param>
        /// <returns></returns>
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool ShowWindow(IntPtr hWnd,
            ShowWindowCommands nCmdShow);

        #endregion
    }
}
