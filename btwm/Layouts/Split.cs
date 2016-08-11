using System;
using System.Collections.Generic;
using System.Linq;

//TODO: Fix vertical split glitch

namespace btwm.Layouts
{
    class WindowTree
    {
        public enum type
        {
            unset  = 0,
            splith = 1,
            splitv = 2,
            window = 3
        }

        public type TreeType;
        public type NextSplit;
        public WindowTree Focused;
        public bool IsFocused;
        public List<WindowTree> SubTrees;
        public WindowTree Parent;
        public Window Window;
        public List<double> Proportions;
        public RECT Surface;

        private WindowTree rootTree;

        /// <summary>
        /// Create a new empty window split tree
        /// </summary>
        /// <param name="Surface"></param>
        public WindowTree(RECT Surface)
        {
            this.Surface = Surface;
            Parent = this;
            Focused = this;
            IsFocused = true;
            SubTrees = new List<WindowTree>();
            Proportions = new List<double>();
            TreeType = (type)1;
            NextSplit = 0;
            rootTree = this;
        }

        private WindowTree(type Type, WindowTree ParentNode, RECT Surface, WindowTree root)
        {
            TreeType = Type;
            Parent = ParentNode;
            this.Surface = Surface;
            SubTrees = new List<WindowTree>();
            Proportions = new List<double>();
            NextSplit = 0;
            rootTree = root;
        }

        private WindowTree(Window Win, WindowTree ParentNode, WindowTree root)
        {
            TreeType = type.window;
            Parent = ParentNode;
            SubTrees = new List<WindowTree>();
            Proportions = new List<double>();
            Window = Win;
            NextSplit = 0;
            rootTree = root;
        }

        public void ReDraw()
        {
            if (Parent == this)
                updateSubSurfacesFromProportions();

            if (TreeType == type.window)
                Window.Surface = Surface;

            foreach (WindowTree tree in SubTrees)
                tree.ReDraw();
        }

        /// <summary>
        /// Rebuild the Proportions List when a window is added/removed
        /// </summary>
        /// <param name="add">Was a window added (true = added, false = removed)
        /// </param>
        public void CalculateNewProportions(bool add)
        {
            if (add)
            {
                double total = 0;
                for (int i = 0; i < Proportions.Count; i++)
                {
                    Proportions[i] = Proportions[i] * Proportions.Count
                        / (Proportions.Count + 1);
                    total += Proportions[i];
                }
                Proportions.Add(1 - total);
            }
            else
            {
                for (int i = 0; i < Proportions.Count; i++)
                {
                    Proportions[i] = Proportions[i] * (Proportions.Count +1)
                        / (Proportions.Count);
                }
            }
        }

        private void updateSubSurfacesFromProportions()
        {
            switch (TreeType)
            {
                case type.splith:
                    int posX = Surface.Left;
                    for (int i = 0; i < SubTrees.Count; i++)
                    {
                        int width = (int)(Surface.Width * Proportions[i]);
                        SubTrees[i].Surface = new RECT(posX, Surface.Top,
                            posX + width, Surface.Bottom);
                        posX += width;
                    }
                    break;
                case type.splitv:
                    int posY = Surface.Top;
                    for (int i = 0; i < SubTrees.Count; i++)
                    {
                        int height = (int)(Surface.Width * Proportions[i]);
                        SubTrees[i].Surface = new RECT(Surface.Left, posY,
                            Surface.Right, posY + height);
                        posY += height;
                    }
                    break;
            }

            foreach (WindowTree tree in SubTrees)
                tree.updateSubSurfacesFromProportions();

        }

        private void updateFocus(WindowTree NewFocused)
        {
            IsFocused = false;
            if (this == NewFocused)
                IsFocused = true;

            Focused = NewFocused;

            foreach (WindowTree tree in SubTrees)
                tree.updateFocus(NewFocused);
        }

        public bool RemoveWindow(Window toRemove)
        {
            if (TreeType == type.window)
            {
                if (Window == toRemove)
                {
                    int index = Parent.SubTrees.FindIndex(t => t == this);
                    Parent.Proportions.RemoveAt(index);
                    Parent.SubTrees.RemoveAt(index);
                    Parent.CalculateNewProportions(false);
                    return true;
                }
                return false;
            }
            else
            {
                foreach (WindowTree tree in SubTrees)
                    if (tree.RemoveWindow(toRemove))
                        return true;
                return false;
            }

        }

        /// <summary>
        /// Insert a new window in the tree. Don't forget to ReDraw() the root
        /// node afterwards.
        /// </summary>
        /// <param name="ToInsert"></param>
        public void InsertWindow(Window ToInsert)
        {
            if (!IsFocused)
                Focused.InsertWindow(ToInsert);
            else
            {
                switch (TreeType)
                {
                    case type.splith:
                        SubTrees.Add(new WindowTree(ToInsert, this, rootTree));
                        updateFocus(SubTrees.Last());
                        CalculateNewProportions(true);
                        break;
                    case type.splitv:
                        SubTrees.Add(new WindowTree(ToInsert, this, rootTree));
                        updateFocus(SubTrees.Last());
                        CalculateNewProportions(true);
                        break;
                    case type.window:
                        if (NextSplit == type.unset)
                        {
                            // NextSplit is unset, we need to insert window in
                            // parent node
                            Parent.SubTrees.Add(
                                new WindowTree(ToInsert, Parent, rootTree));
                            updateFocus(Parent.SubTrees.Last());
                            Parent.CalculateNewProportions(true);
                        }
                        else
                        {
                            TreeType = NextSplit;
                            Proportions.Add(50);
                            SubTrees.Add(new WindowTree(Window, this, rootTree));
                            SubTrees.Add(new WindowTree(ToInsert, this, rootTree));
                            CalculateNewProportions(true);
                            Window = null;
                        }
                        break;
                    default:
                        throw new Exception(
                            "Trying to insert a window in an invalid Tree.");
                }
            }
        }

        public void FocusWindow(Window win)
        {
            if (TreeType == type.window)
            {
                if (Window.Equals(win))
                    rootTree.updateFocus(this);
            }
            else
                foreach (WindowTree tree in SubTrees)
                    tree.FocusWindow(win);

        }
    }

    class Split : Layout
    {
        private WindowTree windowTree;
        public Split(Workspace ws) : base(ws, LayoutType.split)
        {
            windowTree = new WindowTree(ws.Surface);
        }

        public override void InsertWindow(Window newWin, bool floating = false)
        {
            if (floating)
                FloatingWindows.Add(newWin);
            else
            {
                ManagedWindows.Add(newWin);
                windowTree.InsertWindow(newWin);
            }
            redrawWindows();
        }

        public override void RemoveWindow(Window toRemove)
        {
            windowTree.RemoveWindow(toRemove);
            ManagedWindows.Remove(toRemove);
            redrawWindows();
        }

        public override void FocusWindow(Window win)
        {
            // if not a floating window, tell window tree to focus win
            if (!FloatingWindows.Contains(win))
                windowTree.FocusWindow(win);
        }

        private void redrawWindows()
        {
            windowTree.ReDraw();
        }

        public void NextSplit(WindowTree.type type)
        {
            windowTree.Focused.NextSplit = type;
        }
    }
}
