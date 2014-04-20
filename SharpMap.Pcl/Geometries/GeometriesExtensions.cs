// Copyright 2011, 2014 - Fabrizio Vita (www.itacasoft.com)
// This file can be redistributed and/or modified under the terms of the GNU Lesser General Public License.

using GeoAPI.Geometries;
using NetTopologySuite.Geometries;

namespace SharpMap.Geometries
{
    public static class GeometriesExtensions
    {
        public static bool IsEmpty(this Coordinate value)
        {
// ReSharper disable CompareOfFloatsByEqualityOperator
            if ((value.X == 0) && (value.Y == 0))
// ReSharper restore CompareOfFloatsByEqualityOperator
            return true;
            return false;
        }

        public static bool IsEmpty(this Point value)
        {
            // ReSharper disable CompareOfFloatsByEqualityOperator
            if ((value.X == 0) && (value.Y == 0))
                // ReSharper restore CompareOfFloatsByEqualityOperator
                return true;
            return false;
        }
    }
}
