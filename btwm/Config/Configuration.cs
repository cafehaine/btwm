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

        public struct BarSettings
        {
            public struct BarColors
            {
                public string BarBack;
                public string BarFore;
                public string IndicatorBackNormal;
                public string IndicatorForeNormal;
                public string IndicatorBackFocused;
                public string IndicatorForeFocused;

                public string ToCommand()
                {
                    return BarBack + ':' + BarFore + ':' +
                        IndicatorBackNormal + ':' + IndicatorForeNormal + ':' +
                        IndicatorBackFocused + ':' + IndicatorForeFocused;
                }
            }
            public string Position;
            public string Command;
            public string Font;
            public BarColors Colors;
            public int Height { get { return int.Parse(Font.Split(':')[1]) + 2; } }

            public string ToCommand()
            {
                return "pos=" + Position + " command=\"" + Command +
                    "\" font=\"" + Font + "\" colors=" + Colors.ToCommand();
            }
        }

        public ColorsHolder Colors;
        public Font Font;
        public BarSettings Bar;

        public Configuration()
        {
            Colors.Foreground = Color.White;
            Colors.Background = Color.Black;
            Colors.FocusedForeground = Color.White;
            Colors.FocusedBackground = Color.OrangeRed;
            Font = new Font("Courier new", 12, GraphicsUnit.Pixel);
            Bar.Font = "Courier new:15";
            Bar.Position = "bottom";
            Bar.Command = "NotSet";
            Bar.Colors.BarBack = "000";
            Bar.Colors.BarFore = "FFF";
            Bar.Colors.IndicatorBackNormal = "222";
            Bar.Colors.IndicatorForeNormal = "FFF";
            Bar.Colors.IndicatorBackFocused = "F90";
            Bar.Colors.IndicatorForeFocused = "FFF";
        }

        public bool ShouldBeFloating(IntPtr win)
        { return false; }
    }
}
