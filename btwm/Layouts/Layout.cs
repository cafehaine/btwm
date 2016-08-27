using System;
using System.Collections.Generic;

namespace btwm
{
    class Layout : Container
    {
        /// <summary>
        /// A type of layout
        /// </summary>
        public enum LayoutType : uint
        {
            /// <summary>
            /// Windows will be horizontally tiled next to eachothers.
            /// </summary>
            splith,
            /// <summary>
            /// Windows will be vertically tiled next to eachothers.
            /// </summary>
            splitv,
            /// <summary>
            /// Only one window will be displayed. Windows' titles will be
            /// listed vertically a the top of the workspace.
            /// </summary>
            stacking,
            /// <summary>
            /// Only one window will be displayed. Windows' titles will be
            /// displayed as tabs like in web browsers.
            /// </summary>
            tabbed,
            /// <summary>
            /// Only one window will be displayed. Windows' titles will be
            /// displayed as a column on the left of the screen.
            /// </summary>
            vtabbed,
            /// <summary>
            /// Special value, used in handler to let layouts know that the
            /// user did not ask to change the layout type.
            /// </summary>
            unset
        }

        /// <summary>
        /// A list of all windows of this layout
        /// </summary>
        public List<Container> Containers;

        /// <summary>
        /// The type of the layout (split, stacking, tabbed, vtabbed)
        /// </summary>
        public LayoutType Type;

        public enum Direction : byte
        {
            left = 0,
            up = 1,
            right = 2,
            down = 3
        }

        public Layout(Handler handler, RECT surface, LayoutType type, Container parent) : base(ContainerTypes.Layout, handler, surface, parent)
        {
            CanContainWindows = true;
            Surface = surface;
            Type = type;
            Containers = new List<Container>();
        }

        public override string ToString()
		{
			string typeName = "unknown";
			switch (Type)
			{
				case LayoutType.splith:
					typeName = "splith";
					break;
                case LayoutType.splitv:
                    typeName = "splitv";
                    break;
                case LayoutType.tabbed:
					typeName = "tabbed";
					break;
				case LayoutType.stacking:
					typeName = "stacking";
					break;
                case LayoutType.vtabbed:
                    typeName = "vtabbed";
                    break;
			}
			return "{Type=" + typeName + "}";
		}

        public virtual void RepositionWindows() { throw new NotImplementedException(); }

        public virtual void Delete() { throw new NotImplementedException(); }
    }
}
