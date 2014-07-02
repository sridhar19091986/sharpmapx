//==============================================================================
// Copyright 2010-2014 - Fabrizio Vita (www.itacasoft.com)
// This file can be redistributed and/or modified under the terms of the GNU Lesser General Public License.
//===============================================================================

using System;
using SharpMap.Entities;
using System.Collections.Generic;

namespace SharpMap.Geocode
{
    /// <summary>
    /// This enumeration indicates if measure values should be expressed in miles or kilometers
    /// </summary>
    public enum GeoCalcMeasurement
    {
        /// <summary>
        /// Kilometers
        /// </summary>
        Kilometers,
        /// <summary>
        /// Miles
        /// </summary>
        Miles,
        /// <summary>
        /// Degrees
        /// </summary>
        Degrees
    }


    /// <summary>
    /// This class exposes methods to calculate distances on earth
    /// </summary>
    public static class GeoCalc
    {
        private const double EarthRadiusInMiles = 3959.0;
        private const double EarthRadiusInKilometers = 6371.0;
        private static double ToRadian(double val) { return val * (Math.PI / 180); }
        private static double ToDegree(double val) { return val * 180 / Math.PI; }
        private static double DiffRadian(double val1, double val2) { return ToRadian(val2) - ToRadian(val1); }


        /// <summary> 
        /// Calculate the distance between two coordinates. Defaults to using Km. 
        /// </summary> 
        public static double CalcDistance(GisPoint coord1, GisPoint coord2)
        {
            return CalcDistance(coord1, coord2, GeoCalcMeasurement.Kilometers);
        }
        /// <summary> 
        /// Calculate the distance between two coordinates. 
        /// </summary> 
        public static double CalcDistance(GisPoint coord1, GisPoint coord2, GeoCalcMeasurement m)
        {
            double radius = EarthRadiusInKilometers;

            if (m == GeoCalcMeasurement.Degrees)
            {
                double Dx = coord2.X - coord1.X;
                double Dy = coord2.Y - coord1.Y;
                double realLength = Math.Sqrt((Dx * Dx) + (Dy * Dy));

                realLength = Convert.ToDouble(Math.Round(Convert.ToDecimal(realLength), 2));
                return realLength;
            }
            else if (m == GeoCalcMeasurement.Miles)
            {
                radius = EarthRadiusInMiles;
            }

            return radius * 2 * Math.Asin(Math.Min(1, Math.Sqrt((Math.Pow(Math.Sin((DiffRadian(coord1.Y, coord2.Y)) / 2.0), 2.0) + Math.Cos(ToRadian(coord1.Y)) * Math.Cos(ToRadian(coord2.Y)) * Math.Pow(Math.Sin((DiffRadian(coord1.X, coord2.X)) / 2.0), 2.0)))));
        }

        /// <summary>
        /// Calculate the area of the given polygon
        /// </summary>
        /// <param name="points">Points of the polygon</param>
        /// <returns>Area</returns>
        public static double CalcArea(List<GisPoint> points)
        {
            // result is absolute value of sum of the area of all trapezoids
            // made by each polygon line
            double darea = 0;
            double lowerX = 1e30, lowerY = 1e30;

            for (int i = 0; i < points.Count; i++)
            {
                if (points[i].X < lowerX)
                    lowerX = points[i].X;

                if (points[i].Y < lowerY)
                    lowerY = points[i].Y;
            }

            //origin is the SW corner of the polygon
            GisPoint origin = new GisPoint(lowerX, lowerY);

            for (int i=0; i<points.Count; i++) // all points
            {
                int next_pt = i+1;
                if (next_pt >= points.Count)
                    next_pt = 0;

                GisPoint line_a = points[i];
                
                if ((line_a.X > 1e30) || (line_a.Y > 1e30))
                    continue;

                //calculate x1, y1 coordinate relative to origin
                double x1 = CalcDistance(line_a, new GisPoint(origin.X, line_a.Y));
                double y1 = CalcDistance(line_a, new GisPoint(line_a.X, origin.Y)); 

                GisPoint line_b = points[next_pt];
                
                if ((line_b.X > 1e30) || (line_b.Y > 1e30))
                    continue ;

                double x2 = CalcDistance(line_b, new GisPoint(origin.X, line_b.Y));
                double y2 = CalcDistance(line_b, new GisPoint(line_b.X, origin.Y)); 

                darea = darea + (x1 * y2 - x2 * y1) / 2;
            }

            return  Math.Abs( darea );
        }


        /// <summary>
        /// Calculate the perimeter of the given polygon
        /// </summary>
        /// <param name="points">Points of the polygon</param>
        /// <returns>Perimeter</returns>
        public static double CalcPerimeter(List<GisPoint> points)
        {
            double res = 0;
            
            // Make sure to add the first element to the end to close the polygon
            for (int i=0; i<points.Count; i++)
            {
                int nextp = i + 1;
                if (nextp >= points.Count)
                    nextp = 0;
                res += CalcDistance(points[i], points[nextp], GeoCalcMeasurement.Kilometers);
            }

            return res;
        }

    }
}
