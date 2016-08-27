using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace btwm
{
    class Container
    {
        /// <summary>
        /// This container's parent container
        /// </summary>
        public Container ParentContainer;
        /// <summary>
        /// Can this container contain sub-containers.
        /// </summary>
        public bool CanContainContainers;
        /// <summary>
        /// Can this container contain windows.
        /// </summary>
        public bool CanContainWindows;
        /// <summary>
        /// The main handler of this program.
        /// </summary>
        public Handler MainHandler;

        /// <summary>
        /// The different types of containers.
        /// </summary>
        public enum ContainerTypes : byte
        {
            Layout,
            Window,
            Workspace
        }

        protected RECT surface;

        /// <summary>
        /// Hide this container. This message should be transmitted to every
        /// sub-container.
        /// </summary>
        public virtual void Hide() { throw new NotImplementedException(); }
        /// <summary>
        /// Show this container. This message should be transmitted to every
        /// sub-container.
        /// </summary>
        public virtual void Show() { throw new NotImplementedException(); }
        /// <summary>
        /// Remove a sub-container from this container.
        /// </summary>
        /// <param name="x"></param>
        public virtual void RemoveContainer(Container x) { throw new NotImplementedException(); }
        /// <summary>
        /// Insert a sub-container in this container.
        /// </summary>
        /// <param name="x"></param>
        public virtual void InsertContainer(Container x) { throw new NotImplementedException(); }
        /// <summary>
        /// Remove a window from this container.
        /// </summary>
        /// <param name="x"></param>
        public virtual void RemoveWindow(IntPtr x) { throw new NotImplementedException(); }
        /// <summary>
        /// Insert a window in this container.
        /// </summary>
        /// <param name="x"></param>
        public virtual void InsertWindow(IntPtr x) { throw new NotImplementedException(); }
        /// <summary>
        /// Does this container contain a specific window.
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        public virtual bool ContainsWindow(IntPtr x) { throw new NotImplementedException(); }
        /// <summary>
        /// Tells container that a window needs to be focused. This message
        /// should be transmitted to very sub-container.
        /// </summary>
        /// <param name="x"></param>
        public virtual void FocusWindow(IntPtr x) { throw new NotImplementedException(); }
        /// <summary>
        /// Tells container that a window changed it's title. This message
        /// should be transmitted to the sub-container that contains this
        /// window.
        /// </summary>
        /// <param name="x"></param>
        public virtual void WindowChangedTitle(IntPtr x) { throw new NotImplementedException(); }
        /// <summary>
        /// Used to tell the last handled focused window that it should
        /// transform into the specified layout, next time a window is inserted
        /// and that this window is focused. This message should be transmitted
        /// to the sub-container handling this window.
        /// </summary>
        /// <param name="focusedWindow">The last handled focused window</param>
        /// <param name="nextLayout">The layout to transform into</param>
        public virtual void SetNextLayout(IntPtr focusedWindow, Layout.LayoutType nextLayout) { throw new NotImplementedException(); }
        /// <summary>
        /// Called when the container is resized.
        /// </summary>
        protected virtual void surfaceChanged() { throw new NotImplementedException(); }

        /// <summary>
        /// The type of container of this container.
        /// </summary>
        private ContainerTypes type;

        /// <summary>
        /// Gets or sets the surface of this container.
        /// </summary>
        public RECT Surface
        {
            get { return surface; }
            set
            {
                surface = value;
                surfaceChanged();
            }
        }

        /// <summary>
        /// Is this container a window.
        /// </summary>
        public bool IsWindow
            { get { return type == ContainerTypes.Window; } }

        /// <summary>
        /// Is this container a layout.
        /// </summary>
        public bool IsLayout
        { get { return type == ContainerTypes.Layout; } }

        /// <summary>
        /// Is this container a workspace.
        /// </summary>
        public bool IsWorkspace
        { get { return type == ContainerTypes.Workspace; } }

        /// <summary>
        /// This container's type.
        /// </summary>
        public ContainerTypes ContainerType
            { get { return type; } }

        /// <summary>
        /// Create a new container.
        /// </summary>
        /// <param name="type">The type of container</param>
        /// <param name="handler">The handler of the program</param>
        /// <param name="surface">The surface containing this container</param>
        /// <param name="parentContainer">This container's parent container</param>
        public Container(ContainerTypes type, Handler handler = null, RECT surface = new RECT(), Container parentContainer = null)
        {
            ParentContainer = parentContainer;
            MainHandler = handler;
            this.type = type;
            this.surface = surface;
        }
    }
}
