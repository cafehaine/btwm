using System.Drawing;

namespace btwmbar
{
    public static class HexColor
    {
        private static int hexDigit(char chr)
        {
            if (char.IsDigit(chr))
                return int.Parse(chr.ToString());

            switch (char.ToLower(chr))
            {
                case 'a':
                    return 10;
                case 'b':
                    return 11;
                case 'c':
                    return 12;
                case 'd':
                    return 13;
                case 'e':
                    return 14;
                case 'f':
                    return 15;
            }
            return 0;
        }

        private static int hexToInt(char hex)
        {
            return 16 * hexDigit(hex) + hexDigit(hex);
        }

        private static int hexToInt(string hex)
        {
            return 16 * hexDigit(hex[0]) + hexDigit(hex[1]);
        }

        public static Color HexToColor(string HexColor, Color fallback, bool AllowAlpha = true)
        {
            if (HexColor[0] == '#')
                HexColor = HexColor.Substring(1);
            switch (HexColor.Length)
            {
                case 3: // RGB
                    return Color.FromArgb(
                        hexToInt(HexColor[0]),
                        hexToInt(HexColor[1]),
                        hexToInt(HexColor[2]));
                case 4: // ARGB
                    return Color.FromArgb(
                        AllowAlpha ? hexToInt(HexColor[0]) : 255,
                        hexToInt(HexColor[1]),
                        hexToInt(HexColor[2]),
                        hexToInt(HexColor[3]));
                case 6: // RRGGBB
                    return Color.FromArgb(
                        hexToInt(HexColor.Substring(0, 2)),
                        hexToInt(HexColor.Substring(2, 2)),
                        hexToInt(HexColor.Substring(4, 2)));
                case 8: // AARRGGBB
                    return Color.FromArgb(
                        AllowAlpha ? hexToInt(HexColor.Substring(0, 2)) : 255,
                        hexToInt(HexColor.Substring(2, 2)),
                        hexToInt(HexColor.Substring(4, 2)),
                        hexToInt(HexColor.Substring(6, 2)));
            }
            return fallback;
        }
    }
}
