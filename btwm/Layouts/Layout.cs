using System;
using System.Collections.Generic;

namespace btwm
{
    class Layout
    {
        /// <summary>
        /// A type of layout
        /// </summary>
        public enum LayoutType : uint
        {
            /// <summary>
            /// Windows will be tiled next to eachothers.
            /// </summary>
            split,
            /// <summary>
            /// Only one window will be displayed. Windows' titles will be
            /// listed vertically a the top of the workspace.
            /// </summary>
            stacking,
            /// <summary>
            /// Only one window will be displayed. Windows' titles will be
            /// displayed as tabs like in web browsers.
            /// </summary>
            tabbed
        }

        /// <summary>
        /// A list of all non-floating windows of this layout
        /// </summary>
        public List<Window> ManagedWindows;

        /// <summary>
        /// A list of all floating windows in this layout
        /// </summary>
        public List<Window> FloatingWindows;

        /// <summary>
        /// The type of the layout (split, stacking,tabbed)
        /// </summary>
        public LayoutType Type;

        /// <summary>
        /// This layout's parent workspace
        /// </summary>
        public Workspace ParentWorkspace;

        public Layout(Workspace ws, LayoutType type)
        {
            ParentWorkspace = ws;
            Type = type;
            ManagedWindows = new List<Window>();
            FloatingWindows = new List<Window>();
        }

        /// <summary>
        /// Insert a window in this layout
        /// </summary>
        /// <param name="newWin">Inserted window's handler</param>
        /// <param name="floating">Is the window floating</param>
        public virtual void InsertWindow(Window newWin, bool floating = false)
        { }

        /// <summary>
        /// Remove a window from this layout
        /// </summary>
        /// <param name="newWin">Removed window's handler</param>
        public virtual void RemoveWindow(Window toRemove)
        { }

        /// <summary>
        /// Show all windows of this layout
        /// </summary>
        public virtual void Show()
        { }

        /// <summary>
        /// Hide all windows of this layout
        /// </summary>
        public virtual void Hide()
        { }

        public virtual void FocusWindow(Window hwnd)
        { }
    }
}
