using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;

namespace MapApp.Helpers
{
    /// <summary>
    /// Contains methods for converting a hex string into <b>Windows.UI.Color</b> and reverse.
    /// </summary>
    public static class StringColorConverter
    {
        /// <summary>Gets a string of valid hexadecimal characters</summary>
        public static string ValidHexChars = "0123456789ABCDEF";

        /// <summary>
        /// Converts and ARGB hex string into <b>Windows.UI.Color</b>.
        /// </summary>
        /// <param name="hexString">Hexadecimal ARGB string.</param>
        /// <returns>Color.</returns>
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

        /// <summary>
        /// Convert a <b>Windows.UI.Color</b> into hexadecimal ARGB string.
        /// </summary>
        /// <param name="color">Color.</param>
        /// <returns>Hexadecimal ARGB string.</returns>
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
