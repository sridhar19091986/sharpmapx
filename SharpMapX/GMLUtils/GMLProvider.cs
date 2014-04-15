// Originally created by Peter Robineau (peter.robineau@gmx.at) as WPF Provider
// has been modified by Fabrizio Vita (10-03-2011) to read a generic GML stream.
// This file can be redistributed and/or modified under the terms of the GNU Lesser General Public License.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Xml;
using System.IO;
using GeoAPI.Geometries;
using NetTopologySuite.Geometries;

namespace SharpMap.GMLUtils
{
    /// <summary>
    /// This class is the provider of GML maps file.
    /// </summary>
    public class GMLProvider : IDisposable
    {
        #region Fields

        private GMLShapeList _features;
        private IEnvelope _featuresBoundingBox;
        private GMLLayer _gmlLayer = null;

        #endregion

        #region Properties

        /// <summary>
        /// Extent of the geometric elements
        /// </summary>
        public IEnvelope Extent
        {
            get
            {
                return _featuresBoundingBox;
            }
        }

        public GMLLayer Layer
        {
            get
            {
                return _gmlLayer;
            }
        }

        /// <summary>
        /// Returns the list of geometries
        /// </summary>
        public GMLShapeList Features
        {
            get { return _features; }
        }
        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="GMLProvider"/>
        /// </summary>
        /// <param name="geometries">Set of geometries that this datasource should contain</param>
        public GMLProvider(IEnumerable<IGeometry> geometries)
        {
            _features = new GMLShapeList();
            foreach (IGeometry geometry in geometries)
            {
                GMLShape feature = _features.New();
                feature.Geometry = geometry;
                _features.Add(feature);
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GMLProvider"/>
        /// </summary>
        /// <param name="feature">Feature to be included in this datasource</param>
        public GMLProvider(GMLShape feature)
        {
            _features = new GMLShapeList();
            _features.Add(feature);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GMLProvider"/>
        /// </summary>
        /// <param name="features">Features to be included in this datasource</param>
        public GMLProvider(GMLShapeList features)
        {
            _features = features;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GMLProvider"/>
        /// </summary>
        /// <param name="geometry">Geometry to be in this datasource</param>
        public GMLProvider(Geometry geometry)
        {
            _features = new GMLShapeList();
            GMLShape feature = _features.New();
            feature.Geometry = geometry;
            _features.Add(feature);
        }

        /// <summary>
        /// Exttract the name of the layer from the GML string.
        /// </summary>
        /// <param name="gml">GML string.</param>
        /// <returns>Name of the layer.</returns>
        private string ExtractLayerName(string gml)
        {
            int ipos1 = gml.IndexOf("<gml:name>");
            int ipos2 = gml.IndexOf("</gml:name>");

            return gml.Substring(ipos1 + 10, ipos2 - ipos1 - 10);
        }


        /// <summary>
        /// Initializes a new instance of the <see cref="GMLProvider"/>
        /// </summary>
        /// <param name="gml">GML string.</param>
        /// <param name="layer">Layer of gml shapes are part</param>
        public GMLProvider(string gml, GMLLayer layer)
        {
            _features = new GMLShapeList();
            var _featTypeInfo = new FeatTypeInfo();
            _featTypeInfo.Name = ExtractLayerName(gml);

            _gmlLayer = layer;
            if (_gmlLayer == null)
                throw new Exception("Unable to find layer: " + _featTypeInfo.Name);

            string geometryTypeString = _gmlLayer.GetShapeTypeAsGML();
            _featTypeInfo.Geometry._GeometryType = geometryTypeString;

            GeometryFactory geomFactory = null;
            Collection<GMLShape> shapes = null;

            var xmlReaderSettings = new XmlReaderSettings();
            xmlReaderSettings.IgnoreComments = true;
            xmlReaderSettings.IgnoreProcessingInstructions = true;
            xmlReaderSettings.IgnoreWhitespace = true;
            _XmlReader = XmlReader.Create(new StringReader(gml), xmlReaderSettings);

            if (geometryTypeString == "")
                geometryTypeString = GMLProvider.DetectGeometryType(gml);

            try
            {
                switch (geometryTypeString)
                {
                    /* Primitive geometry elements */

                    // GML2
                    case "PointPropertyType":
                        geomFactory = new PointFactory(_XmlReader, _featTypeInfo, _gmlLayer);
                        break;

                    // GML2
                    case "LineStringPropertyType":
                        geomFactory = new LineStringFactory(_XmlReader, _featTypeInfo, _gmlLayer);
                        break;

                    // GML2
                    case "PolygonPropertyType":
                        geomFactory = new PolygonFactory(_XmlReader, _featTypeInfo, _gmlLayer);
                        break;

                    // GML3
                    case "CurvePropertyType":
                        geomFactory = new LineStringFactory(_XmlReader, _featTypeInfo, _gmlLayer);
                        break;

                    // GML3
                    case "SurfacePropertyType":
                        geomFactory = new PolygonFactory(_XmlReader, _featTypeInfo, _gmlLayer);
                        break;

                    /* Aggregate geometry elements */

                    // GML2
                    case "MultiPointPropertyType":
                        if (_MultiGeometries)
                            geomFactory = new MultiPointFactory(_XmlReader, _featTypeInfo, _gmlLayer);
                        else
                            geomFactory = new PointFactory(_XmlReader, _featTypeInfo, _gmlLayer);
                        break;

                    // GML2
                    case "MultiLineStringPropertyType":
                        if (_MultiGeometries)
                            geomFactory = new MultiLineStringFactory(_XmlReader, _featTypeInfo, _gmlLayer);
                        else
                            geomFactory = new LineStringFactory(_XmlReader, _featTypeInfo, _gmlLayer);
                        break;

                    // GML2
                    case "MultiPolygonPropertyType":
                        if (_MultiGeometries)
                            geomFactory = new MultiPolygonFactory(_XmlReader, _featTypeInfo, _gmlLayer);
                        else
                            geomFactory = new PolygonFactory(_XmlReader, _featTypeInfo, _gmlLayer);
                        break;

                    // GML3
                    case "MultiCurvePropertyType":
                        if (_MultiGeometries)
                            geomFactory = new MultiLineStringFactory(_XmlReader, _featTypeInfo, _gmlLayer);
                        else
                            geomFactory = new LineStringFactory(_XmlReader, _featTypeInfo, _gmlLayer);
                        break;

                    // GML3
                    case "MultiSurfacePropertyType":
                        if (_MultiGeometries)
                            geomFactory = new MultiPolygonFactory(_XmlReader, _featTypeInfo, _gmlLayer);
                        else
                            geomFactory = new PolygonFactory(_XmlReader, _featTypeInfo, _gmlLayer);
                        break;

                    default:
                        //TODO: non funziona
                        geomFactory = new UnspecifiedGeometryFactory_WFS1_0_0_GML2(_XmlReader, _featTypeInfo, _gmlLayer);
                        shapes = geomFactory.createGeometries();
                        break;
                }

                if (shapes == null)
                    shapes = geomFactory.createGeometries();

                _featuresBoundingBox = geomFactory.FeaturesBoundingBox;
                _features.Name = _gmlLayer.Name;

                foreach (GMLShape feature in shapes)
                {
                    _features.Add(feature);
                }
            }
            finally
            {
                geomFactory.Dispose();
            }
        }


        #endregion

        #region IProvider Members

        /// <summary>
        /// Returns the geometries contained in the box
        /// </summary>
        /// <param name="box">Bounding box</param>
        /// <returns>List of shapes</returns>
        public void PopulateFeaturesInView(Envelope box, GMLShapeList results)
        {
            foreach (GMLShape feature in _features)
            {
                if (feature.Geometry.EnvelopeInternal.Intersects(box))
                {
                    results.Add(feature);
                }
            }
        }

        /// <summary>
        /// Boundingbox of dataset
        /// </summary>
        /// <returns>boundingbox</returns>
        public IEnvelope GetExtents()
        {
            if (_features.Count == 0)
                return null;
            IEnvelope box = null; // _Geometries[0].GetBoundingBox();
            for (int i = 0; i < _features.Count; i++)
                if (!_features[i].Geometry.IsEmpty)
                    box = box == null ? _features[i].Geometry.EnvelopeInternal : box.Union(_features[i].Geometry.EnvelopeInternal);

            return box;
        }

        /// <summary>
        /// Gets the connection ID of the datasource
        /// </summary>
        /// <remarks>
        /// The ConnectionID is meant for Connection Pooling which doesn't apply to this datasource. Instead
        /// <c>String.Empty</c> is returned.
        /// </remarks>
        public string ConnectionID
        {
            get { return String.Empty; }
        }

        /// <summary>
        /// Opens the datasource
        /// </summary>
        public void Open()
        {
            //Do nothing;
        }

        /// <summary>
        /// Closes the datasource
        /// </summary>
        public void Close()
        {
            //Do nothing;
        }

        /// <summary>
        /// Disposes the object
        /// </summary>
        public void Dispose()
        {
            _features = null;
        }

        #endregion

        /// <summary>
        /// TODO: sistemare la lettura dei MultiPoint (adesso non funziona) e impostare Multigeometries a True
        /// </summary>
        private bool _MultiGeometries = false;
        private XmlReader _XmlReader = null;
        /// <summary>
        /// GML web site.
        /// </summary>
        protected const string _GMLNS = "http://www.opengis.net/gml";

        /// <summary>
        /// Detect the type of shape contained in the GML string.
        /// </summary>
        /// <param name="gml">GML string to be evaluated.</param>
        /// <returns>Type of the geometry</returns>
        public static string DetectGeometryType(string gml)
        {
            int ipos = -1;

            ipos = gml.IndexOf("<gml:Point>");
            if (ipos > 0) return "PointPropertyType";

            ipos = gml.IndexOf("<gml:LineString>");
            if (ipos > 0) return "LineStringPropertyType";

            ipos = gml.IndexOf("<gml:Polygon>");
            if (ipos > 0) return "PolygonPropertyType";

            ipos = gml.IndexOf("<gml:MultiPoint>");
            if (ipos > 0) return "MultiPointPropertyType";

            ipos = gml.IndexOf("<gml:MultiLineString>");
            if (ipos > 0) return "MultiLineStringPropertyType";

            ipos = gml.IndexOf("<gml:MultiPolygon>");
            if (ipos > 0) return "MultiPolygonPropertyType";

            ipos = gml.IndexOf("<gml:MultiCurve>");
            if (ipos > 0) return "MultiCurvePropertyType";

            ipos = gml.IndexOf("<gml:MultiSurface>");
            if (ipos > 0) return "MultiSurfacePropertyType";

            return "";
        }
    }
}
