using System;
using System.Collections.Generic;

namespace btwm.Layouts
{
    class Split : Layout
    {
        private class sizeContainerPair
        {
            public int Size;
            public Container Container;

            public sizeContainerPair(int size, Container container)
            {
                Size = size;
                Container = container;
            }
        }

        List<sizeContainerPair> subContainers;

        List<IntPtr> handledWindows;
        bool horizontal;

        public Split(Handler handler, RECT surface, Container parent, bool horizontal) : base(handler, surface, horizontal ? LayoutType.splith : LayoutType.splitv, parent)
        {
            this.horizontal = horizontal;
            CanContainContainers = true;
            subContainers = new List<sizeContainerPair>();
            handledWindows = new List<IntPtr>();
        }

        public override bool ContainsWindow(IntPtr x)
        {
            return handledWindows.Contains(x);
        }

        public override void Hide()
        {
            subContainers.ForEach(p => p.Container.Hide());
        }

        public override void Show()
        {
            subContainers.ForEach(p => p.Container.Show());
        }

        public override void WindowChangedTitle(IntPtr x)
        {
            // for now only vtabbed layouts need this
            foreach (sizeContainerPair p in subContainers)
            {
                if (p.Container.ContainerType == ContainerTypes.Layout &&
                    p.Container.ContainsWindow(x))
                    p.Container.WindowChangedTitle(x);
            }
        }

        public override void FocusWindow(IntPtr x)
        {
            // for now this is only usefull in vtabbed layouts
            foreach (sizeContainerPair p in subContainers)
            {
                if (p.Container.ContainerType != ContainerTypes.Layout)
                    p.Container.FocusWindow(x);
            }
        }

        public override void InsertWindow(IntPtr toInsert)
        {
            handledWindows.Add(toInsert);

            IntPtr focused = MainHandler.LastHandledFocused;

            if (subContainers.Count == 0)
                InsertContainer(new Window(toInsert));
            else
            {
                for (int i = 0; i < subContainers.Count; i++)
                {
                    Container c = subContainers[i].Container;
                    if (c.ContainerType == ContainerTypes.Window)
                    {
                        if ((c as Window).HWnd == focused)
                        {
                            //if NextLayout is set, transform c into new layout
                            //else insert window inthis layout
                            if (MainHandler.NextLayout != LayoutType.unset)
                            {
                                Container newContainer;
                                switch (MainHandler.NextLayout)
                                {
                                    case LayoutType.splith:
                                        newContainer = new Split(MainHandler, c.Surface, this, true);
                                        newContainer.InsertContainer(c);
                                        newContainer.InsertContainer(new Window(toInsert));
                                        c = newContainer;
                                        break;
                                    case LayoutType.splitv:
                                        newContainer = new Split(MainHandler, c.Surface, this, false);
                                        newContainer.InsertContainer(c);
                                        newContainer.InsertContainer(new Window(toInsert));
                                        c = newContainer;
                                        break;
                                    case LayoutType.stacking:
                                        throw new NotImplementedException();
                                    case LayoutType.tabbed:
                                        throw new NotImplementedException();
                                    case LayoutType.vtabbed:
                                        newContainer = new Vtabbed(MainHandler, c.Surface, this);
                                        newContainer.InsertWindow((c as Window).HWnd);
                                        newContainer.InsertWindow(toInsert);
                                        c = newContainer;
                                        break;
                                }
                            }
                            else
                            {
                                InsertContainer(new Window(toInsert));
                            }
                        }
                    }
                    else if (c.ContainsWindow(focused))
                        c.InsertWindow(toInsert);
                }
            }
        }

        public override void RepositionWindows()
        {
            if (horizontal)
            {
                int leftPos = surface.Left;
                for (int i = 0; i < subContainers.Count; i++)
                {
                    RECT newSurface = surface;
                    newSurface.Left = leftPos;
                    newSurface.Width = subContainers[i].Size;
                    leftPos += subContainers[i].Size;
                    subContainers[i].Container.Surface = newSurface;
                }
            }
            else
            {
                int topPos = surface.Top;
                for (int i = 0; i < subContainers.Count; i++)
                {
                    RECT newSurface = surface;
                    newSurface.Top = topPos;
                    newSurface.Height = subContainers[i].Size;
                    topPos += subContainers[i].Size;
                    subContainers[i].Container.Surface = newSurface;
                }
            }
        }

        public override void InsertContainer(Container x)
        {
            int totalSize = horizontal ? surface.Width : surface.Height;
            if (subContainers.Count == 0)
                subContainers.Add(new sizeContainerPair(totalSize, x));
            else
            {
                int remainingSize = totalSize;
                for (int i = 0; i < subContainers.Count; i++)
                {
                    subContainers[i].Size = subContainers[i].Size / (subContainers.Count + 1) * subContainers.Count;
                    remainingSize -= subContainers[i].Size;
                }
                subContainers.Add(new sizeContainerPair(remainingSize, x));
            }
            RepositionWindows();
        }

        public override void RemoveWindow(IntPtr x)
        {
            handledWindows.Remove(x);
            int toRemove = -1;
            for (int i = 0; i < subContainers.Count; i++)
            {
                if (subContainers[i].Container.IsWindow)
                {
                    if ((subContainers[i].Container as Window).HWnd == x)
                        toRemove = i;
                }
                else if (subContainers[i].Container.ContainsWindow(x))
                    subContainers[i].Container.RemoveWindow(x);
            }

            if (toRemove != -1)
            {
                if (subContainers.Count > 1)
                {
                    int toAdd = subContainers[toRemove].Size / (subContainers.Count - 1);
                    subContainers.RemoveAt(toRemove);
                    int totalSize = horizontal ? surface.Width : surface.Height;

                    int remainingSize = totalSize;
                    for (int i = 0; i < subContainers.Count - 1; i++)
                    {
                        subContainers[i].Size += toAdd;
                        remainingSize -= subContainers[i].Size;
                    }
                    subContainers[subContainers.Count - 1].Size = remainingSize;
                    RepositionWindows();
                }
                else
                {
                    subContainers.Clear();
                }
            }
        }

        protected override void surfaceChanged()
        {
            if (subContainers != null)
            {
                if (subContainers.Count == 1)
                    subContainers[0].Size = horizontal ? surface.Width : surface.Height;
                else if (subContainers.Count >= 2)
                {
                    int oldSize = 0;
                    subContainers.ForEach(p => oldSize += p.Size);
                    int remaining = (horizontal ? surface.Width : surface.Height);
                    float factor = oldSize / remaining;

                    for (int i = 0; i < subContainers.Count - 1; i++)
                    {
                        subContainers[i].Size = (int)(subContainers[i].Size / factor);
                        remaining -= subContainers[i].Size;
                    }
                    subContainers[subContainers.Count - 1].Size = remaining;
                }
            }
        }

        ~Split()
        {
            handledWindows.Clear();
            subContainers.Clear();
        }
    }
}
