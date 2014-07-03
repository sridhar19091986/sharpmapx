using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace SharpMap
{
    public static class StringUtils
    {
        /// <summary>
        /// Decodes an UTF8 array to string.
        /// </summary>
        /// <param name="characters">UTF8 array.</param>
        /// <returns>string.</returns>
        public static String UTF8ByteArrayToString(Byte[] characters)
        {
            UTF8Encoding encoding = new UTF8Encoding();
            String constructedString = "";// encoding.GetString(characters);
            return (constructedString.Substring(1));
        }

        /// <summary>
        /// Encodes a string to a UTF8 array.
        /// </summary>
        /// <param name="pXmlString">string to be encoded.</param>
        /// <returns>array of UTF8 characters.</returns>
        public static Byte[] StringToUTF8ByteArray(String pXmlString)
        {
            UTF8Encoding encoding = new UTF8Encoding();
            Byte[] byteArray = encoding.GetBytes(pXmlString);
            return byteArray;
        }

        /// <summary>
        /// Replaces character \r and \n with carriage return.
        /// </summary>
        /// <param name="input">input string</param>
        /// <returns>output string</returns>
        public static string ReplaceCarriageReturn(string input)
        {
            char r = '\r';
            char n = '\n';

            StringBuilder sb = new StringBuilder();
            sb.Append(r);
            sb.Append(n);

            input = input.Replace(@"\r\n", sb.ToString());
            //input = input.Replace(@"\n", Convert.ToString(n));
            return input;
        }

        /// <summary>
        /// Replaces not standard characters for sql
        /// </summary>
        /// <param name="value">SQL string to be normalized</param>
        /// <returns>Normalized string</returns>
        public static string NormalizeStringForSQL(string value)
        {
            return value.Replace("'", "''");
        }

        internal static char GetNumericListSeparator(IFormatProvider provider)
        {
            char ch = ',';
            NumberFormatInfo instance = NumberFormatInfo.GetInstance(provider);
            if ((instance.NumberDecimalSeparator.Length > 0) && (ch == instance.NumberDecimalSeparator[0]))
            {
                ch = ';';
            }
            return ch;
        }

    }
}
