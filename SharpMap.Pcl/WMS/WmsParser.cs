//==============================================================================
// Copyright 2010-2014 - Fabrizio Vita (www.itacasoft.com)
// This file can be redistributed and/or modified under the terms of the GNU Lesser General Public License.
//===============================================================================

using System;
using System.Globalization;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using SharpMap.Entities;

namespace SharpMap.WMS
{
    public static class WmsParser
    {
        private static WmsLayerInfo ReadLayer(XElement xlayer)
        {
            var layer = new WmsLayerInfo();

            layer.IsQueryable = xlayer.Attribute("queryable") != null ? xlayer.Attribute("queryable").Value == "1" : false;
            layer.Name = xlayer.Element("Name") != null ? xlayer.Element("Name").Value : "";
            layer.Title = xlayer.Element("Title") != null ? xlayer.Element("Title").Value : "";
            layer.Comments = xlayer.Element("Abstract") != null ? xlayer.Element("Abstract").Value : "";

            //LatLonBoundingBox is specific for WMS 1.1.1 servers    
            if (xlayer.Element("LatLonBoundingBox") != null)
                layer.LatLonBoundingBox = ParseElementAsExtent(xlayer.Element("LatLonBoundingBox"));

            if (xlayer.Element("BoundingBox") != null)
                layer.BoundingBox = ParseElementAsExtent(xlayer.Element("BoundingBox"));

            var xstyles = xlayer.Descendants("Style");
            foreach (XElement xstyle in xstyles)
            {
                var ls = new LegendIcon();

                if (xstyle.Element("LegendURL") != null)
                {
                    ls.Width = Convert.ToInt32(xstyle.Element("LegendURL").Attribute("width").Value);
                    ls.Height = Convert.ToInt32(xstyle.Element("LegendURL").Attribute("height").Value);
                    ls.SymbolUri = xstyle.Element("LegendURL").Element("OnlineResource").Attribute(xlink + "href").Value;
                }
                ls.Name = xstyle.Element("Name").Value;
                layer.Symbols.Add(ls);
            }

            return layer;
        }

        private static XNamespace xlink = "http://www.w3.org/1999/xlink";

        public static WmsProjectInfo Read(string capabilities)
        {
            var result = new WmsProjectInfo();

            XDocument doc;
            var settings = new XmlReaderSettings();
            settings.DtdProcessing = DtdProcessing.Ignore;

            using (XmlReader reader = XmlReader.Create(new StringReader(capabilities), settings))
            {
                doc = XDocument.Load(reader);
            }

            var docRoot = doc.Element("WMT_MS_Capabilities");
            result.Name = docRoot.Element("Service").Element("Name").Value;
            result.Title = docRoot.Element("Service").Element("Title").Value;
            result.Abstract = docRoot.Element("Service").Element("Abstract").Value;

            var supportedProjections = docRoot.Element("Capability").Element("Layer").Descendants("SRS");
            foreach (XElement xsupportedProjection in supportedProjections)
            {
                result.Projections.Add(xsupportedProjection.Value);
            }

            //LatLonBoundingBox is specific for WMS 1.1.1 servers    
            if (docRoot.Element("Capability").Element("Layer").Element("LatLonBoundingBox") != null)
                result.LatLonBoundingBox = ParseElementAsExtent(docRoot.Element("Capability").Element("Layer").Element("LatLonBoundingBox"));

            //Layers
            var layers = docRoot.Element("Capability").Element("Layer").Descendants("Layer");
            foreach (XElement xlayer in layers)
            {
                result.Layers.Add(ReadLayer(xlayer));
            }

            //GetFeatureInfo
            var oocap = docRoot.Element("Capability")
                             .Element("Request")
                             .Element("GetCapabilities")
                             .Element("DCPType")
                             .Element("HTTP")
                             .Element("Get")
                             .Element("OnlineResource");
            result.GetCapabilities.Url = oocap.Attribute(xlink + "href").Value;

            var formats = docRoot.Element("Capability").Element("Request").Element("GetCapabilities").Descendants("Format");
            foreach (XElement format in formats)
            {
                result.GetCapabilities.Formats.Add(format.Value);
            }

            //GetMap
            var oor = docRoot.Element("Capability")
                             .Element("Request")
                             .Element("GetMap")
                             .Element("DCPType")
                             .Element("HTTP")
                             .Element("Get")
                             .Element("OnlineResource");

            result.GetMapInfo.Url = oor.Attribute(xlink + "href").Value;

            formats = docRoot.Element("Capability").Element("Request").Element("GetMap").Descendants("Format");
            foreach (XElement format in formats)
            {
                result.GetMapInfo.Formats.Add(format.Value);
            }

            //GetFeatureInfo
            oor = docRoot.Element("Capability")
                             .Element("Request")
                             .Element("GetFeatureInfo")
                             .Element("DCPType")
                             .Element("HTTP")
                             .Element("Get")
                             .Element("OnlineResource");

            result.GetFeatureInfo.Url = oor.Attribute(xlink + "href").Value;

            formats = docRoot.Element("Capability").Element("Request").Element("GetFeatureInfo").Descendants("Format");
            foreach (XElement format in formats)
            {
                result.GetFeatureInfo.Formats.Add(format.Value);
            }

            //GetLegendGraphic
            if (docRoot.Element("Capability").Element("Request").Element("GetLegendGraphic") != null)
            {
                result.GetLegendGraphic = new WmsGetLegendGraphic();

                oor = docRoot.Element("Capability")
                             .Element("Request")
                             .Element("GetLegendGraphic")
                             .Element("DCPType")
                             .Element("HTTP")
                             .Element("Get")
                             .Element("OnlineResource");

                result.GetLegendGraphic.Url = oor.Attribute(xlink + "href").Value;

                formats =
                    docRoot.Element("Capability").Element("Request").Element("GetLegendGraphic").Descendants("Format");
                foreach (XElement format in formats)
                {
                    result.GetLegendGraphic.Formats.Add(format.Value);
                }
            }

            formats =
                docRoot.Element("Capability").Element("Exception").Descendants("Format");
            foreach (XElement format in formats)
            {
                result.ExceptionFormats.Add(format.Value);
            }
            
            result.CalculateExtent();
            return result;
        }

        private static Extent ParseElementAsExtent(XElement boundingBox)
        {
            double minx = ParseNodeAsDouble(boundingBox.Attribute("minx"), -180.0);
            double miny = ParseNodeAsDouble(boundingBox.Attribute("miny"), -90.0);
            double maxx = ParseNodeAsDouble(boundingBox.Attribute("maxx"), 180.0);
            double maxy = ParseNodeAsDouble(boundingBox.Attribute("maxy"), 90.0);

            return new Extent(minx, miny, maxx, maxy);
        }

        public static XAttribute FindEpsgNode(XElement bbox)
        {
            if (bbox == null)
                throw new ArgumentNullException("bbox");

            var epsgNode = bbox.Attribute("srs") ?? bbox.Attribute("crs");
            if (epsgNode == null)
                epsgNode = bbox.Attribute("CRS") ?? bbox.Attribute("SRS");

            return epsgNode;
        }

        public static bool TryParseNodeAsEpsg(XAttribute node, out int epsg)
        {
            epsg = default(int);
            if (node == null) return false;
            string epsgString = node.Value;
            if (String.IsNullOrEmpty(epsgString)) return false;
            const string prefix = "EPSG:";
#if PCL
            int index = epsgString.IndexOf(prefix, StringComparison.OrdinalIgnoreCase);
#else
            int index = epsgString.IndexOf(prefix, StringComparison.InvariantCulture);
#endif
            if (index < 0) return false;
            return (Int32.TryParse(epsgString.Substring(index + prefix.Length), NumberStyles.Any, CultureInfo.InvariantCulture, out epsg));
        }

        public static double ParseNodeAsDouble(XAttribute node, double defaultValue)
        {
            if (node == null) return defaultValue;
            if (String.IsNullOrEmpty(node.Value)) return defaultValue;
            double value;
            if (Double.TryParse(node.Value, NumberStyles.Any, CultureInfo.InvariantCulture, out value))
                return value;
            return defaultValue;
        }

        public static bool TryParseNodeAsDouble(XAttribute node, out double value)
        {
            value = default(double);
            if (node == null) return false;
            if (String.IsNullOrEmpty(node.Value)) return false;
            return Double.TryParse(node.Value, NumberStyles.Any, CultureInfo.InvariantCulture, out value);
        }

    }
}
