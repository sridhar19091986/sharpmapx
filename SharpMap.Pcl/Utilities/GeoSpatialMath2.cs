//==============================================================================
// Copyright 2010-2014 - Fabrizio Vita (www.itacasoft.com)
// This file can be redistributed and/or modified under the terms of the GNU Lesser General Public License.
//===============================================================================

using System;
using GeoAPI.Geometries;
using NetTopologySuite.Geometries;
using SharpMap.Entities;

namespace SharpMap.Utilities
{
    internal enum CoordinateType
    {
        Latitude,
        Longitude
    }

    /// <summary>
    /// This static class exposes useful methods.
    /// </summary>
    public static partial class GeoSpatialMath
    {
        /// <summary>
        /// Converts zoom level to scale.
        /// </summary>
        /// <param name="latitude">Latitude</param>
        /// <param name="zoomlevel">zoom level</param>
        /// <returns>The scale as text</returns>
        public static string ScaleFromZoomLevelAsText(double latitude, double zoomlevel)
        {
            double scaleDown = 1 / ZoomLevelToScale(latitude, zoomlevel);
            return @"1:" + ((int)scaleDown).ToString();
        }

        /// <summary>
        /// Converts zoom level to scale.
        /// </summary>
        /// <param name="latitude">Latitude</param>
        /// <param name="zoomlevel">zoom level</param>
        /// <returns>The scale as double</returns>
        public static double ZoomLevelToScale(double latitude, double zoomlevel)
        {
            double resolution = ZoomLevelToResolution(latitude, zoomlevel);

            double scaleDown = (100 * 39.37 * resolution);
            return 1 / scaleDown;
        }


        /// <summary>
        /// Returns resolution from zoom level
        /// </summary>
        /// <param name="latitude">latitude</param>
        /// <param name="zoomlevel">zoom level</param>
        /// <returns>resolution in meters per pixel</returns>
        public static double ZoomLevelToResolution(double latitude, double zoomlevel)
        {
            /*
             * Pixel per metro all'equatore
                        switch ((int)zoomlevel)
                        {
                            case 1: metersPerPixel = 78271.52; break;
                            case 2: metersPerPixel =  39135.76; break;
                            case 3: metersPerPixel =  19567.88; break;
                            case 4: metersPerPixel =  9783.94; break;
                            case 5: metersPerPixel = 4891.97; break;
                            case 6: metersPerPixel = 2445.98; break;
                            case 7: metersPerPixel = 1222.99; break;
                            case 8: metersPerPixel = 611.50; break;
                            case 9: metersPerPixel = 305.75; break;
                            case 10: metersPerPixel = 152.87; break;
                            case 11: metersPerPixel = 76.44; break;
                            case 12: metersPerPixel = 38.22; break;
                            case 13: metersPerPixel = 19.11; break;
                            case 14: metersPerPixel = 9.55; break;
                            case 15: metersPerPixel = 4.78; break;
                            case 16: metersPerPixel = 2.39; break;
                            case 17: metersPerPixel = 1.19; break;
                            case 18: metersPerPixel = 0.60; break;
                            case 19: metersPerPixel = 0.30; break;
                        }

             */
            double resolution = 156543.04 * Math.Cos(latitude * Math.PI / 180) / (Math.Pow(2, zoomlevel));

            return resolution;
        }


        /// <summary>
        /// Transforms a couple of coordinates in standard coordinates.
        /// </summary>
        /// <param name="latitude">latitude</param>
        /// <param name="longitude">longitude</param>
        /// <returns>Coordinates expressed in standard fashion</returns>
        public static string DecimalToDMS(double latitude, double longitude)
        {
            string res = DDtoDMS(latitude, CoordinateType.Latitude);
            res += " " + DDtoDMS(longitude, CoordinateType.Longitude);
            return res;
        }

        /// <summary>
        /// Transforms a decimal coordinate in a standard coordinates
        /// </summary>
        /// <param name="coordinate">coordinate in decimal</param>
        /// <param name="type">latitude or longitude</param>
        /// <returns>coordinate expressed in standard fashion.</returns>
        private static string DDtoDMS(double coordinate, CoordinateType type)
        {
            // Set flag if number is negative
            bool neg = coordinate < 0d;

            // Work with a positive number
            coordinate = Math.Abs(coordinate);

            // Get d/m/s components
            double d = Math.Floor(coordinate);
            coordinate -= d;
            coordinate *= 60;
            double m = Math.Floor(coordinate);
            coordinate -= m;
            coordinate *= 60;
            double s = Math.Round(coordinate);

            // Create padding character
            char pad;
            char.TryParse("0", out pad);

            // Create d/m/s strings
            string dd = d.ToString();
            string mm = m.ToString().PadLeft(2, pad);
            string ss = s.ToString().PadLeft(2, pad);

            // Append d/m/s
            string dms = string.Format("{0}°{1}'{2}\"", dd, mm, ss);

            // Append compass heading
            switch (type)
            {
                case CoordinateType.Longitude:
                    dms += neg ? "W" : "E";
                    break;
                case CoordinateType.Latitude:
                    dms += neg ? "S" : "N";
                    break;
            }

            // Return formated string
            return dms;
        }


        /// <summary>
        /// Calculates center and zoom level given an extent.
        /// </summary>
        /// <param name="actualWidth">width of the map in screen coordinates</param>
        /// <param name="actualHeight">height of the map in screen coordinates</param>
        /// <param name="xmin">min X of the extent</param>
        /// <param name="ymin">min Y of the extent</param>
        /// <param name="xmax">max X of the extent</param>
        /// <param name="ymax">max Y of the extent</param>
        /// <param name="centerLocation">output center</param>
        /// <param name="zoomlevel">output zoom level</param>
        public static void CalculateCenterAndZoomByRectangle(double actualWidth, double actualHeight, double xmin, double ymin, double xmax, double ymax, ref IPoint centerLocation, out int zoomlevel)
        {
            Point topLeft = new Point(xmin, ymax);
            Point bottomRight = new Point(xmax, ymin);
            CalculateCenterAndZoomByRectangle(actualWidth, actualHeight, topLeft, bottomRight, ref centerLocation, out zoomlevel);
        }

        /// <summary>
        /// Calculates center and zoom level given an extent.
        /// </summary>
        /// <param name="actualWidth">width of the map in screen coordinates</param>
        /// <param name="actualHeight">height of the map in screen coordinates</param>
        /// <param name="leftTop">Left top corner of the extent.</param>
        /// <param name="rightBottom">Right bottom corner of the extent</param>
        /// <param name="centerLocation">output center.</param>
        /// <param name="zoomlevel">output zoom level.</param>
        public static void CalculateCenterAndZoomByRectangle(double actualWidth, double actualHeight, IPoint leftTop, IPoint rightBottom, ref IPoint centerLocation, out int zoomlevel)
        {
            centerLocation.Y = (leftTop.Y + rightBottom.Y) / 2;
            centerLocation.X = (leftTop.X + rightBottom.X) / 2;

            double tileSize = 256;
            double viewportWidth = (rightBottom.X - leftTop.X) / 360;
            double viewportHeight = (leftTop.Y - rightBottom.Y) / 180;
            if (viewportWidth / actualWidth < viewportHeight / actualHeight)
            {
                viewportWidth = viewportHeight;
            }
            zoomlevel = (int)Math.Log(actualWidth / tileSize / viewportWidth, 2d);
        }

        /// <summary>
        /// Calculates center and zoom level given an extent.
        /// </summary>
        /// <param name="actualWidth">width of the map in screen coordinates</param>
        /// <param name="actualHeight">height of the map in screen coordinates</param>
        /// <param name="leftTop">Left top corner of the extent.</param>
        /// <param name="rightBottom">Right bottom corner of the extent</param>
        /// <param name="centerLocation">output center.</param>
        /// <param name="zoomlevel">output zoom level.</param>
        public static void CalculateCenterAndZoomByRectangle(double actualWidth, double actualHeight, GisPoint leftTop, GisPoint rightBottom, ref GisPoint centerLocation, out int zoomlevel)
        {
            centerLocation.Y = (leftTop.Y + rightBottom.Y) / 2;
            centerLocation.X = (leftTop.X + rightBottom.X) / 2;

            double tileSize = 256;
            double viewportWidth = (rightBottom.X - leftTop.X) / 360;
            double viewportHeight = (leftTop.Y - rightBottom.Y) / 180;
            if (viewportWidth / actualWidth < viewportHeight / actualHeight)
            {
                viewportWidth = viewportHeight;
            }
            zoomlevel = (int)Math.Log(actualWidth / tileSize / viewportWidth, 2d);
        }

        /// <summary>
        /// Converts a degree value to radians
        /// </summary>
        /// <param name="degrees">Value in degrees</param>
        /// <returns>Value in radians</returns>
        public static double ToRadian(double degrees)
        {
            return degrees * (Math.PI / 180);
        }

        /// <summary>
        /// Convert a radians value to degree
        /// </summary>
        /// <param name="radians">Value in radians</param>
        /// <returns>Value in degress</returns>
        public static double ToDegrees(double radians)
        {
            return radians * (180 / Math.PI);
        }

        /// <summary>
        /// Returns the longitude given the tile number.
        /// </summary>
        /// <param name="tile_x"></param>
        /// <param name="zoom"></param>
        /// <returns></returns>
        public static double TileToWorldPosX(double tile_x, int zoom)
        {
            double f1 = (tile_x / Math.Pow(2.0, zoom) * 360.0) - 180;
            return f1;
        }

        /// <summary>
        /// Returns the latitude given the tile number.
        /// </summary>
        /// <param name="tile_y"></param>
        /// <param name="zoom"></param>
        /// <returns></returns>
        public static double TileToWorldPosY(double tile_y, int zoom)
        {
            double f1 = ((2.0 * Math.PI * tile_y) / Math.Pow(2.0, zoom));
            double n = Math.PI - f1;
            return (double)(180.0 / Math.PI * Math.Atan(Math.Sinh(n)));
        }

    }
}
