using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;

namespace MapApp.Helpers
{
    public static class StringColorConverter
    {
        public static string ValidHexChars = "0123456789ABCDEF";

        public static Color ArgbStringToColor(string hexString)
        {
            if (hexString.Length > 10)
            {
                throw new ArgumentException("Error: String is too long");
            }
            var substring = hexString;
            if (hexString[0] == '#')
            {
                substring = hexString.Substring(1);
            }
            else if (hexString[1] == 'x')
            {
                substring = hexString.Substring(2);
            }

            foreach (char c in substring)
            {
                if (!ValidHexChars.Contains(c))
                {
                    throw new ArgumentException("Error: String contains invalid character");
                }
            }

            byte a = 0;

            if (substring.Length == 8)
            {
                a = Convert.ToByte(substring.Substring(0, 2), 16);
                substring = substring.Substring(2);
            }

            byte r = Convert.ToByte(substring.Substring(0, 2), 16);
            byte g = Convert.ToByte(substring.Substring(2, 2), 16);
            byte b = Convert.ToByte(substring.Substring(4, 2), 16);

            return Color.FromArgb(a, r, g, b);
        }

        public static string ArgbColorToString(Color color)
        {
            string result = "";
            result += color.A.ToString("X2");
            result += color.R.ToString("X2");
            result += color.G.ToString("X2");
            result += color.B.ToString("X2");
            return result;
        }
    }
}
