//==============================================================================
// Copyright 2010-2014 - Fabrizio Vita (www.itacasoft.com)
// This file can be redistributed and/or modified under the terms of the GNU Lesser General Public License.
//===============================================================================

using SharpMap.Entities;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace SharpMap.Utilities
{
    public static class Parser
    {
        public static double StringAsDouble(string value, double defaultValue)
        {
            if (String.IsNullOrEmpty(value)) return defaultValue;
            double result;
            if (Double.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out result))
                return result;
            return defaultValue;
        }

        public static Int32 StringAsInteger(string value, Int32 defaultValue)
        {
            if (String.IsNullOrEmpty(value)) return defaultValue;
            Int32 result;
            if (Int32.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out result))
                return result;
            return defaultValue;
        }

        public static DateTime StringAsDateTime(string value)
        {
            DateTime result;
            if (DateTime.TryParse(value, out result))
                return result;
            return DateTime.MinValue;
        }
    }
}
