using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace btwm
{
    class Layout
    {
        public enum LayoutType : uint
        {
            split,
            stacking,
            tabbed,
            window
        }

        public List<Window> Windows;
        public LayoutType Type;
        public RECT Surface;
        public Workspace ParentWorkspace;

        public Layout(Workspace ws, LayoutType type)
        {
            ParentWorkspace = ws;
            Surface = ws.surface;
            Type = type;
            Windows = new List<Window>();
        }

        public virtual void InsertWindow(Window newWin)
        { }

        public virtual void RemoveWindow(Window toRemove)
        { }

        public virtual void Show()
        { }

        public virtual void Hide()
        { }
    }
}
