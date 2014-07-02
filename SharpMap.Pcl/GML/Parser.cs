//==============================================================================
// Copyright 2010-2014 - Fabrizio Vita (www.itacasoft.com)
// This file can be redistributed and/or modified under the terms of the GNU Lesser General Public License.
//===============================================================================

using GeoAPI.Geometries;
using SharpMap.Entities;
using SharpMap.Utilities;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Xml.Linq;

namespace SharpMap.GML
{
    public static class Parser
    {
        /// <summary>
        /// Returns the value of a child element of an XML element
        /// </summary>
        /// <param name="xparent"></param>
        /// <param name="elementName"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static string ElementAsString(XElement xparent, string elementName, string defaultValue = null)
        {
            var xelement = ChildElement(xparent, elementName);
            if (xelement != null)
                return xelement.Value;

            return defaultValue;
        }

        /// <summary>
        /// Returns the child element of an XML element
        /// </summary>
        /// <param name="xparent"></param>
        /// <param name="elementName"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static XElement ChildElement(XElement xparent, string elementName)
        {
            if (xparent.Element(elementName) != null)
                return xparent.Element(elementName);

            foreach (var xelement in xparent.Elements())
            {
                if (string.Equals(xelement.Name.LocalName, elementName, StringComparison.OrdinalIgnoreCase))
                    return xelement;
            }

            return null;
        }


        /// <summary>
        /// Returns the value of an attribute of an XML element
        /// </summary>
        /// <param name="xparent"></param>
        /// <param name="elementName"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static string AttributeAsString(XElement xparent, string attributeName, string defaultValue = null)
        {
            if (xparent.Attribute(attributeName) != null)
                return xparent.Attribute(attributeName).Value;

            foreach (var xattr in xparent.Attributes())
            {
                if (string.Equals(xattr.Name.LocalName, attributeName, StringComparison.OrdinalIgnoreCase))
                    return xattr.Value;
            }

            return defaultValue;
        }

        private static XNamespace gml = "http://www.opengis.net/gml";

        public static double StringAsDouble(string value, double defaultValue)
        {
            if (String.IsNullOrEmpty(value)) return defaultValue;
            double result;
            if (Double.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out result))
                return result;
            return defaultValue;
        }

        public static Extent ElementAsPolygon(XElement xparent)
        {
            string srsSname = AttributeAsString(xparent, "srsName");

            var xcoordinates = xparent.Element(gml + "coordinates");
            if (xcoordinates != null)
            {
                string value = xcoordinates.Value;

                var list = new List<GisPoint>();
                string[] scoordses = value.Split(' ');

                foreach (var scoords in scoordses)
                {
                    string[] coords = scoords.Split(',');

                    double lon = StringAsDouble(coords[0], -1);
                    double lat = StringAsDouble(coords[1], -1);

                    var p = new GisPoint(lon, lat);

                    list.Add(p);
                }

                double minX = Math.Min(list[0].X, list[1].X);
                double miny = Math.Min(list[0].Y, list[1].Y);
                double maxX = Math.Max(list[0].X, list[1].X);
                double maxy = Math.Max(list[0].Y, list[1].Y);

                return new Extent(minX, miny, maxX, maxy);
            }

            return null;
        }


        public static Extent ElementAsExtent(XElement xparent)
        {
            string srsSname = AttributeAsString(xparent, "srsName");

            var xcoordinates = xparent.Element(gml + "coordinates");
            if (xcoordinates != null)
            {
                string value = xcoordinates.Value;
                
                var list = new List<GisPoint>();
                string[] scoordses = value.Split(' ');

                foreach (var scoords in scoordses)
                {
                    string[] coords = scoords.Split(',');

                    double lon = StringAsDouble(coords[0], -1);
                    double lat = StringAsDouble(coords[1], -1);

                    var p = new GisPoint(lon, lat);

                    list.Add(p);
                }

                double minX = Math.Min(list[0].X, list[1].X);
                double miny = Math.Min(list[0].Y, list[1].Y);
                double maxX = Math.Max(list[0].X, list[1].X);
                double maxy = Math.Max(list[0].Y, list[1].Y);

                return new Extent(minX, miny, maxX, maxy);
            }

            return null;
        }

        public static List<GisPoint> StringAsCoordinates(string value)
        {
            var result = new List<GisPoint>();
            string[] scoordses = value.Split(' ');

            foreach (var scoords in scoordses)
            {
                string[] coords = scoords.Split(',');

                double lon = Utilities.Parser.StringAsDouble(coords[0], -1);
                double lat = Utilities.Parser.StringAsDouble(coords[1], -1);

                var p = new GisPoint(lon, lat);

                p = ProjectionConversion.ConvertToWgs84(p);

                result.Add(p);
            }

            return result;
        }

    }
}
