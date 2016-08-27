using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace btwm.Config
{
    class Configuration
    {
        public struct ColorsHolder
        {
            public Color Foreground;
            public Color Background;
            public Color FocusedForeground;
            public Color FocusedBackground;
        }

        public ColorsHolder Colors;
        public Font Font;
        public int BarHeight = 17;

        public Configuration()
        {
            Colors.Foreground = Color.White;
            Colors.Background = Color.Black;
            Colors.FocusedForeground = Color.White;
            Colors.FocusedBackground = Color.OrangeRed;
            Font = new Font("Courier new", 12, GraphicsUnit.Pixel);
        }

        public bool ShouldBeFloating(IntPtr win)
        { return false; }
    }
}
