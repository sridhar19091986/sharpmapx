using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpMap.Styles;

namespace SharpMap
{
    public static class ColorUtils
    {
        #region -- Data Members --
        static char[] hexDigits = {
     '0', '1', '2', '3', '4', '5', '6', '7',
     '8', '9', 'A', 'B', 'C', 'D', 'E', 'F'};
        #endregion

        /// <summary>
        /// Convert a .NET Color to a hex string.
        /// </summary>
        /// <param name="color">Color</param>
        /// <returns>ex: "FFFFFF", "AB12E9"</returns>
        public static string ColorToHexString(Color color)
        {
            byte[] bytes = new byte[3];
            bytes[0] = (byte)color.R;
            bytes[1] = (byte)color.G;
            bytes[2] = (byte)color.B;
            char[] chars = new char[bytes.Length * 2];
            for (int i = 0; i < bytes.Length; i++)
            {
                int b = bytes[i];
                chars[i * 2] = hexDigits[b >> 4];
                chars[i * 2 + 1] = hexDigits[b & 0xF];
            }
            return new string(chars);
        }

        /// <summary>
        /// Convert a .NET Color to a hex string.
        /// </summary>
        /// <param name="color">Color</param>
        /// <param name="includeAlpha">Include Alpha</param>
        /// <returns>ex: "FFFFFF", "AB12E9"</returns>
        public static string ColorToHexString(Color color, bool includeAlpha)
        {
            return string.Concat(
                includeAlpha ? color.A.ToString("x2") : string.Empty,
                color.R.ToString("x2"),
                color.G.ToString("x2"),
                color.B.ToString("x2"));
        }

        /// <summary>
        /// Converts an exadecimal rappresentation in color.
        /// </summary>
        /// <param name="hex">Hexadecimal representation of the color</param>
        /// <param name="includeAlpha">If true, the color string includes the alpha value</param>
        /// <returns>Color</returns>
        public static Color HexToColor(string hex, bool includeAlpha)
        {
            hex.Replace("#", string.Empty);
            var alphaSpace = includeAlpha ? 2 : 0;
            var a = includeAlpha ? (byte)Convert.ToInt32(hex.Substring(0, 2), 16) : (byte)255;
            var r = (byte)Convert.ToInt32(hex.Substring(0 + alphaSpace, 2), 16);
            var g = (byte)Convert.ToInt32(hex.Substring(2 + alphaSpace, 2), 16);
            var b = (byte)Convert.ToInt32(hex.Substring(4 + alphaSpace, 2), 16);

            return Color.FromArgb(a, r, g, b);
        }

    }
}
