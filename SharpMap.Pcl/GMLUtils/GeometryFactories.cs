//==============================================================================
// WFS provider by Peter Robineau (peter.robineau@gmx.at)
// Modified by Fabrizio Vita (fabrizio.vita@itacasoft.it), 10-03-2010
// This file can be redistributed and/or modified under the terms of the GNU Lesser General Public License.
//===============================================================================

using System;
using System.Globalization;
using System.Xml;
using GeoAPI.Geometries;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using NetTopologySuite.Geometries;
using SharpMap.Entities;

namespace SharpMap.GmlUtils
{
    internal class FeatTypeInfo
    {
        #region Fields with Properties

        private BoundingBox _boundingBox = new BoundingBox();
        private string _Cs = ",";
        private string _DecimalDel = ".";
        private string _FeatureTypeNamespace = string.Empty;
        private GeometryInfo _Geometry = new GeometryInfo();
        private string _Name = string.Empty;

        private string _Prefix = string.Empty;
        private string _ServiceURI = string.Empty;
        private string _SRID = "4326";
        private string _Ts = " ";

        /// <summary>
        /// Nome del tipo di feature
        /// </summary>
        /// <value>Nome.</value>
        public string Name
        {
            get { return _Name; }
            set { _Name = value; }
        }

        /// <summary>
        /// Prefisso del tipo e il suo elemento annidato.
        /// </summary>
        public string Prefix
        {
            get { return _Prefix; }
            set { _Prefix = value; }
        }

        /// <summary>
        /// Namespace della caratteristica
        /// </summary>
        public string FeatureTypeNamespace
        {
            get { return _FeatureTypeNamespace; }
            set { _FeatureTypeNamespace = value; }
        }

        /// <summary>
        /// Gets the qualified name of the featuretype (with namespace URI).
        /// </summary>
        internal string QualifiedName
        {
            get { return _FeatureTypeNamespace + _Name; }
        }

        /// <summary>
        /// Gets or sets the service URI for WFS 'GetFeature' request.
        /// This argument is obligatory for data retrieving.
        /// </summary>
        public string ServiceURI
        {
            get { return _ServiceURI; }
            set { _ServiceURI = value; }
        }

        /// <summary>
        /// Gets or sets information about the geometry of the featuretype.
        /// Setting at least the geometry name is obligatory for data retrieving.
        /// </summary>
        public GeometryInfo Geometry
        {
            get { return _Geometry; }
            set { _Geometry = value; }
        }

        /// <summary>
        /// Gets or sets the spatial extent of the featuretype - defined as minimum bounding rectangle. 
        /// </summary>
        public BoundingBox BBox
        {
            get { return _boundingBox; }
            set { _boundingBox = value; }
        }

        /// <summary>
        /// Gets or sets the spatial reference ID
        /// </summary>
        public string SRID
        {
            get { return _SRID; }
            set { _SRID = value; }
        }

        //Coordinates can be included in a single string, but there is no 
        //facility for validating string content. The value of the 'cs' attribute 
        //is the separator for coordinate values, and the value of the 'ts' 
        //attribute gives the tuple separator (a single space by default); the 
        //default values may be changed to reflect local usage.

        /// <summary>
        /// Decimal separator (for gml:coordinates)
        /// </summary>
        public string DecimalDel
        {
            get { return _DecimalDel; }
            set { _DecimalDel = value; }
        }

        /// <summary>
        /// Separator for coordinate values (for gml:coordinates)
        /// </summary>
        public string Cs
        {
            get { return _Cs; }
            set { _Cs = value; }
        }

        /// <summary>
        /// Tuple separator (for gml:coordinates)
        /// </summary>
        public string Ts
        {
            get { return _Ts; }
            set { _Ts = value; }
        }

        #endregion

        #region Constructors


        /// <summary>
        /// Creates a new instance of the <see cref="FeatTypeInfo"/>
        /// </summary>
        public FeatTypeInfo()
        {
        }

        #endregion

        #region Nested Types

        #region BoundingBox

        /// <summary>
        /// The bounding box defines the spatial extension
        /// </summary>
        public class BoundingBox
        {
            public double _MaxLat = 0;
            public double _MaxLong = 0;
            public double _MinLat = 0;
            public double _MinLong = 0;
        }

        #endregion

        #region GeometryInfo

        /// <summary>
        /// The geometry info comprises the name of the geometry attribute (e.g. 'Shape" or 'geom')
        /// and the type of the featuretype's geometry.
        /// </summary>
        public class GeometryInfo
        {
            public string _GeometryName = string.Empty;
            public string _GeometryType = string.Empty;
        }

        #endregion

        #endregion
    }


    /// <summary>
    /// Base abstract class for production of geometries.
    /// Provides parsing functionalities compatible with GML2/GML3.
    /// </summary>
    internal abstract class GeometryFactory : IDisposable
    {
        #region Fields

        protected const string _GMLNS = "http://www.opengis.net/gml";
        private readonly NumberFormatInfo _format = new NumberFormatInfo();
        private readonly List<IPathNode> _PathNodes = new List<IPathNode>();
        protected AlternativePathNodesCollection _CoordinatesNode;
        private string _cs;
        protected IPathNode _featureNode;
        protected IPathNode _propertyNode;
        private IPathNode _boundedByNode;
        protected XmlReader _featureReader;
        protected FeatTypeInfo _featureTypeInfo;
        protected XmlReader _geomReader;

        protected Collection<SimpleGisShape> _shapes = new Collection<SimpleGisShape>();

        private IPathNode _labelNode = null;
        protected AlternativePathNodesCollection _serviceExceptionNode;
        private string _Ts;
        protected XmlReader _xmlReader;
        protected IList<string> _fieldNames;
        #endregion

        #region Constructors


        /// <summary>
        /// Protected constructor for the abstract class.
        /// </summary>
        /// <param name="xmlReader">An XmlReader instance</param>
        /// <param name="featureTypeInfo">A <see cref="FeatTypeInfo"/> instance providing metadata of the featuretype to query</param>
        /// <param name="fieldNames">Names of fields</param>
        protected GeometryFactory(XmlReader xmlReader, FeatTypeInfo featureTypeInfo, IList<string> fieldNames)
        {
            _featureTypeInfo = featureTypeInfo;
            _xmlReader = xmlReader;
            _fieldNames = fieldNames;
            initializePathNodes();
            initializeSeparators();
        }

        #endregion

        #region Internal Member

        /// <summary>
        /// Abstract method - overwritten by derived classes for producing instances
        /// derived from Geometry/>.
        /// </summary>
        internal abstract Collection<SimpleGisShape> createGeometries();


        #endregion

        #region Protected Member

        /// <summary>
        /// This method parses a coordinates or posList(from 'GetFeature' response). 
        /// </summary>
        /// <param name="reader">An XmlReader instance at the position of the coordinates to read</param>
        /// <returns>A point collection (the collected coordinates)</returns>
        protected Coordinate[] ParseCoordinates(XmlReader reader)
        {
            if (!reader.Read()) return null;

            string name = reader.LocalName;
            string coordinateString = reader.ReadElementContentAsString().Trim();
            string[] coordinateValues;
            int i = 0, length = 0;

            if (name.Equals("coordinates"))
                coordinateValues = coordinateString.Split(_cs[0], _Ts[0]);
            else
                coordinateValues = coordinateString.Split(' ');

            length = coordinateValues.Length;

            var vertices = new List<Coordinate>();
            while (i < length - 1)
            {
                double c1 = Convert.ToDouble(coordinateValues[i++], _format);
                double c2 = Convert.ToDouble(coordinateValues[i++], _format);

                if (name.Equals("coordinates"))
                    vertices.Add(new Coordinate(c1, c2));
                else
                    vertices.Add(new Coordinate(c2, c1));
            }

            return vertices.ToArray();
        }

        /// <summary>
        /// Extent of the entities
        /// </summary>
        public IEnvelope FeaturesBoundingBox = null;

        /// <summary>
        /// Extract the bounding box from the GML
        /// </summary>
        protected void ParseBoundingBox()
        {
            GetSubReaderOf(_xmlReader, null, _boundedByNode);
            _xmlReader.ReadToDescendant("gml:coordinates");
            string bound = _xmlReader.ReadElementContentAsString();

            string[] strList = bound.Split(',', ' ');
            if (strList.Length != 4) return;

            NumberFormatInfo provider = new NumberFormatInfo();
            provider.NumberDecimalSeparator = ".";

            Coordinate p1 = new Coordinate(Convert.ToDouble(strList[0], provider), Convert.ToDouble(strList[1], provider));
            Coordinate p2 = new Coordinate(Convert.ToDouble(strList[2], provider), Convert.ToDouble(strList[3], provider));

            FeaturesBoundingBox = new Envelope(p1, p2);
        }


        /// <summary>
        /// This method parses a properties of a member. 
        /// </summary>
        /// <param name="reader">An XmlReader instance at the position of the PROPERTIES to read</param>
        /// <param name="isSelected">True if the entity is selected</param>
        /// <param name="uid">ID if the entity</param>
        /// <returns>A list of values</returns>
        protected List<string> ParseProperties(XmlReader reader, out bool isSelected, out int uid)
        {
            var res = new List<string>();

            bool fidDone = false;
            bool getFieldNames = _fieldNames.Count == 0;

            uid = 0;
            isSelected = false;

            while (!reader.EOF)
            {
                if (reader.NodeType == XmlNodeType.Element)
                {
                    if (reader.LocalName == "PROPERTIES")
                    {
                        int c = reader.AttributeCount;
                        string value = reader.GetAttribute("fid");
                        uid = Convert.ToInt32(value.Substring(4));

                        fidDone = true;

                        //provo a recupare l'attributo di selezione (se c'è)
                        value = reader.GetAttribute("selected");
                        isSelected = (value == "1");

                        reader.Read();
                    }
                    else if (reader.LocalName == "GEOMETRY")
                    {
                        return res;
                    }
                    else
                    {
                        if (fidDone)
                        {
                            string value = reader.ReadElementContentAsString();
                            res.Add(value);

                            if (getFieldNames)
                            {
                                _fieldNames.Add(reader.LocalName);
                            }

                            if ((reader.EOF) || (reader.LocalName == "GEOMETRY"))
                                return res;
                        }
                        else
                        {
                            throw new Exception("Unknown case");
                        }
                    }
                }
                else
                {
                    reader.Read();
                }
            }

            return res;
        }


        /// <summary>
        /// This method retrieves an XmlReader within a specified context.
        /// </summary>
        /// <param name="reader">An XmlReader instance that is the origin of a created sub-reader</param>
        /// <param name="labelValue">A string array for recording a found label value. Pass 'null' to ignore searching for label values</param>
        /// <param name="pathNodes">A list of <see cref="IPathNode"/> instances defining the context of the retrieved reader</param>
        /// <returns>A sub-reader of the XmlReader given as argument</returns>
        protected XmlReader GetSubReaderOf(XmlReader reader, string[] labelValue, params IPathNode[] pathNodes)
        {
            _PathNodes.Clear();
            _PathNodes.AddRange(pathNodes);
            return GetSubReaderOf(reader, labelValue, _PathNodes);
        }

        /// <summary>
        /// This method retrieves an XmlReader within a specified context.
        /// Moreover it collects label values before or after a geometry could be found.
        /// </summary>
        /// <param name="reader">An XmlReader instance that is the origin of a created sub-reader</param>
        /// <param name="labelValue">A string array for recording a found label value. Pass 'null' to ignore searching for label values</param>
        /// <param name="pathNodes">A list of <see cref="IPathNode"/> instances defining the context of the retrieved reader</param>
        /// <returns>A sub-reader of the XmlReader given as argument</returns>
        protected XmlReader GetSubReaderOf(XmlReader reader, string[] labelValue, List<IPathNode> pathNodes)
        {
            string errorMessage = null;

            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element)
                {
                    if (pathNodes[0].Matches(reader))
                    {
                        pathNodes.RemoveAt(0);

                        if (pathNodes.Count > 0)
                            return GetSubReaderOf(reader.ReadSubtree(), null, pathNodes);

                        return reader.ReadSubtree();
                    }

                    if (labelValue != null)
                        if (_labelNode != null)
                            if (_labelNode.Matches(reader))
                                labelValue[0] = reader.ReadElementContentAsString();//.ReadElementString();


                    if (_serviceExceptionNode.Matches(reader))
                    {
                        errorMessage = reader.ReadInnerXml();
                        throw new Exception("A service exception occured: " + errorMessage);
                    }
                }
            }

            return null;
        }
        #endregion

        #region Private Member

        /// <summary>
        /// This method initializes path nodes needed by the derived classes.
        /// </summary>
        private void initializePathNodes()
        {
            IPathNode coordinatesNode = new PathNode("http://www.opengis.net/gml", "coordinates",
                                                        (NameTable)_xmlReader.NameTable);
            IPathNode posListNode = new PathNode("http://www.opengis.net/gml", "posList",
                                                    (NameTable)_xmlReader.NameTable);
            IPathNode ogcServiceExceptionNode = new PathNode("http://www.opengis.net/ogc", "ServiceException",
                                                                (NameTable)_xmlReader.NameTable);
            IPathNode serviceExceptionNode = new PathNode("", "ServiceException", (NameTable)_xmlReader.NameTable);
            //ServiceExceptions without ogc prefix are returned by deegree. PDD.
            IPathNode exceptionTextNode = new PathNode("http://www.opengis.net/ows", "ExceptionText",
                                                        (NameTable)_xmlReader.NameTable);
            _CoordinatesNode = new AlternativePathNodesCollection(coordinatesNode, posListNode);
            _serviceExceptionNode = new AlternativePathNodesCollection(ogcServiceExceptionNode, exceptionTextNode,
                                                                        serviceExceptionNode);
            _featureNode = new PathNode(_featureTypeInfo.FeatureTypeNamespace, _featureTypeInfo.Name,
                                        (NameTable)_xmlReader.NameTable);

            _propertyNode = new PathNode("http://www.itacasoft.com/GML", "PROPERTIES",
                                                        (NameTable)_xmlReader.NameTable);

            _boundedByNode = new PathNode("http://www.opengis.net/gml", "boundedBy",
                                                        (NameTable)_xmlReader.NameTable);
        }

        /// <summary>
        /// This method initializes separator variables for parsing coordinates.
        /// From GML specification: Coordinates can be included in a single string, but there is no 
        /// facility for validating string content. The value of the 'cs' attribute 
        /// is the separator for coordinate values, and the value of the 'ts' 
        /// attribute gives the tuple separator (a single space by default); the 
        /// default values may be changed to reflect local usage.
        /// </summary>
        private void initializeSeparators()
        {
            string decimalDel = string.IsNullOrEmpty(_featureTypeInfo.DecimalDel) ? ":" : _featureTypeInfo.DecimalDel;
            _cs = string.IsNullOrEmpty(_featureTypeInfo.Cs) ? "," : _featureTypeInfo.Cs;
            _Ts = string.IsNullOrEmpty(_featureTypeInfo.Ts) ? " " : _featureTypeInfo.Ts;
            _format.NumberDecimalSeparator = decimalDel;
        }

        #endregion

        #region IDisposable Member

        /// <summary>
        /// This method closes the XmlReader member.
        /// </summary>
        public void Dispose()
        {
            if (_xmlReader != null)
                _xmlReader.Dispose();
        }

        #endregion

        protected void FillShapeFields(SimpleGisShape shp, List<string> fieldvalues)
        {
            for (int i = 0; i < fieldvalues.Count; i++)
            {
                shp[_fieldNames[i]] = fieldvalues[i];
            }
        }
    }

    /// <summary>
    /// This class produces instances of type <see cref="Point"/>.
    /// The base class is <see cref="GeometryFactory"/>.
    /// </summary>
    internal class PointFactory : GeometryFactory
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="PointFactory"/> class.
        /// This constructor shall just be called from the MultiPoint factory. The feature node therefore is deactivated.
        /// </summary>
        /// <param name="xmlReader">An XmlReader instance</param>
        /// <param name="featureTypeInfo">A <see cref="FeatTypeInfo"/> instance providing metadata of the featuretype to query</param>
        public PointFactory(XmlReader xmlReader, FeatTypeInfo featureTypeInfo, IList<string> fieldnames)
            : base(xmlReader, featureTypeInfo, fieldnames)
        {
            _featureNode.IsActive = false;
        }

        #endregion

        #region Internal Member


        /// <summary>
        /// This method produces instances of type collection of <see cref="GIS_Shape"/>.
        /// </summary>
        /// <returns>The created geometries</returns>
        internal override Collection<SimpleGisShape> createGeometries()
        {
            IPathNode pointNode = new PathNode(_GMLNS, "Point", (NameTable)_xmlReader.NameTable);
            string[] labelValue = new string[1];

            try
            {
                ParseBoundingBox();

                // Reading the entire feature's node makes it possible to collect label values that may appear before or after the geometry property
                while ((_featureReader = GetSubReaderOf(_xmlReader, null, _featureNode)) != null)
                {
                    while ((_geomReader = GetSubReaderOf(_featureReader, null, _propertyNode)) != null)
                    {
                        bool isSelected;
                        int uid;
                        List<string> ll = ParseProperties(_geomReader, out isSelected, out uid);

                        _geomReader = GetSubReaderOf(_featureReader, labelValue, pointNode, _CoordinatesNode);
                        if (_geomReader == null)
                        {
                            throw new Exception("Geometry of type point not found (if this is a ttkls file, please check the shapetype field)");
                        }

                        Coordinate c = ParseCoordinates(_geomReader)[0];
                        var shp = new SimpleGisShape(new Point(c));
                        shp.IsSelected = isSelected;
                        shp.UID = uid;
                        _shapes.Add(shp);

                        FillShapeFields(shp, ll);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return _shapes;
        }

        #endregion
    }

    /// <summary>
    /// This class produces instances of type <see cref="LineString"/>.
    /// The base class is <see cref="GeometryFactory"/>.
    /// </summary>
    internal class LineStringFactory : GeometryFactory
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="LineStringFactory"/> class.
        /// This constructor shall just be called from the MultiLineString factory. The feature node therefore is deactivated.
        /// </summary>
        /// <param name="xmlReader">An XmlReader instance</param>
        /// <param name="featureTypeInfo">A <see cref="FeatTypeInfo"/> instance providing metadata of the featuretype to query</param>
        internal LineStringFactory(XmlReader xmlReader, FeatTypeInfo featureTypeInfo, IList<string> fieldnames)
            : base(xmlReader, featureTypeInfo, fieldnames)
        {
            _featureNode.IsActive = false;
        }

        #endregion

        #region Internal Member

        /// <summary>
        /// This method produces instances of type <see cref="LineString"/>.
        /// </summary>
        /// <returns>The created geometries</returns>
        internal override Collection<SimpleGisShape> createGeometries()
        {
            IPathNode lineStringNode = new PathNode(_GMLNS, "LineString", (NameTable)_xmlReader.NameTable);
            string[] labelValue = new string[1];

            try
            {
                ParseBoundingBox();

                // Reading the entire feature's node makes it possible to collect label values that may appear before or after the geometry property
                while ((_featureReader = GetSubReaderOf(_xmlReader, null, _featureNode)) != null)
                {
                    while ((_geomReader = GetSubReaderOf(_featureReader, null, _propertyNode)) != null)
                    {
                        bool isSelected;
                        int uid;
                        List<string> ll = ParseProperties(_geomReader, out isSelected, out uid);

                        _geomReader = GetSubReaderOf(_featureReader, labelValue, lineStringNode, _CoordinatesNode);

                        LineString l = new LineString(ParseCoordinates(_geomReader));
                        var shp = new SimpleGisShape(l);
                        shp.IsSelected = isSelected;
                        shp.UID = uid;
                        _shapes.Add(shp);
                        FillShapeFields(shp, ll);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return _shapes;
        }

        #endregion
    }

    /// <summary>
    /// This class produces instances of type <see cref="PolygonFactory"/>.
    /// The base class is <see cref="GeometryFactory"/>.
    /// </summary>
    internal class PolygonFactory : GeometryFactory
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="PolygonFactory"/> class.
        /// This constructor shall just be called from the MultiPolygon factory. The feature node therefore is deactivated.
        /// </summary>
        /// <param name="xmlReader">An XmlReader instance</param>
        /// <param name="featureTypeInfo">A <see cref="FeatTypeInfo"/> instance providing metadata of the featuretype to query</param>
        internal PolygonFactory(XmlReader xmlReader, FeatTypeInfo featureTypeInfo, IList<string> fieldnames)
            : base(xmlReader, featureTypeInfo, fieldnames)
        {
            _featureNode.IsActive = false;
        }

        #endregion

        #region Internal Member

        /// <summary>
        /// This method produces instances of type Polygon/>.
        /// </summary>
        /// <returns>The created geometries</returns>
        internal override Collection<SimpleGisShape> createGeometries()
        {
            SimpleGisShape shp = null;
            XmlReader outerBoundaryReader = null;
            XmlReader innerBoundariesReader = null;

            IPathNode polygonNode = new PathNode(_GMLNS, "Polygon", (NameTable)_xmlReader.NameTable);
            IPathNode outerBoundaryNode = new PathNode(_GMLNS, "outerBoundaryIs", (NameTable)_xmlReader.NameTable);
            IPathNode exteriorNode = new PathNode(_GMLNS, "exterior", (NameTable)_xmlReader.NameTable);
            IPathNode outerBoundaryNodeAlt = new AlternativePathNodesCollection(outerBoundaryNode, exteriorNode);
            IPathNode innerBoundaryNode = new PathNode(_GMLNS, "innerBoundaryIs", (NameTable)_xmlReader.NameTable);
            IPathNode interiorNode = new PathNode(_GMLNS, "interior", (NameTable)_xmlReader.NameTable);
            IPathNode innerBoundaryNodeAlt = new AlternativePathNodesCollection(innerBoundaryNode, interiorNode);
            IPathNode linearRingNode = new PathNode(_GMLNS, "LinearRing", (NameTable)_xmlReader.NameTable);
            string[] labelValue = new string[1];

            try
            {
                ParseBoundingBox();

                // Reading the entire feature's node makes it possible to collect label values that may appear before or after the geometry property
                while ((_featureReader = GetSubReaderOf(_xmlReader, null, _featureNode)) != null)
                {
                    while ((_geomReader = GetSubReaderOf(_featureReader, null, _propertyNode)) != null)
                    {
                        bool isSelected;
                        int uid;
                        List<string> ll = ParseProperties(_geomReader, out isSelected, out uid);

                        _geomReader = GetSubReaderOf(_featureReader, labelValue, polygonNode);

                        ILinearRing shell = null;
                        List<ILinearRing> holes = new List<ILinearRing>();

                        if (
                            (outerBoundaryReader =
                                GetSubReaderOf(_geomReader, null, outerBoundaryNodeAlt, linearRingNode, _CoordinatesNode)) !=
                            null)
                            shell = new LinearRing(ParseCoordinates(outerBoundaryReader));

                        while (
                            (innerBoundariesReader =
                                GetSubReaderOf(_geomReader, null, innerBoundaryNodeAlt, linearRingNode, _CoordinatesNode)) !=
                            null)
                            holes.Add(new LinearRing(ParseCoordinates(innerBoundariesReader)));

                        Polygon polygon = new Polygon(shell, holes.ToArray());
                        shp = new SimpleGisShape(polygon);
                        shp.IsSelected = isSelected;
                        shp.UID = uid;
                        
                        _shapes.Add(shp);
                        FillShapeFields(shp, ll);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return _shapes;
        }

        #endregion
    }

    /// <summary>
    /// This class produces instances of type <see cref="MultiPoint"/>.
    /// The base class is <see cref="GeometryFactory"/>.
    /// </summary>
    internal class MultiPointFactory : GeometryFactory
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="MultiPointFactory"/> class.
        /// </summary>
        /// <param name="xmlReader">An XmlReader instance</param>
        /// <param name="featureTypeInfo">A <see cref="FeatTypeInfo"/> instance providing metadata of the featuretype to query</param>
        internal MultiPointFactory(XmlReader xmlReader, FeatTypeInfo featureTypeInfo, IList<string> fieldnames)
            : base(xmlReader, featureTypeInfo, fieldnames)
        {
        }

        #endregion

        #region Internal Member

        /// <summary>
        /// This method produces instances of type <see cref="MultiPoint"/>.
        /// </summary>
        /// <returns>The created geometries</returns>
        internal override Collection<SimpleGisShape> createGeometries()
        {
            SimpleGisShape shp = null;

            IPathNode multiPointNode = new PathNode(_GMLNS, "MultiPoint", (NameTable)_xmlReader.NameTable);
            IPathNode pointMemberNode = new PathNode(_GMLNS, "pointMember", (NameTable)_xmlReader.NameTable);
            string[] labelValue = new string[1];

            try
            {
                ParseBoundingBox();

                // Reading the entire feature's node makes it possible to collect label values that may appear before or after the geometry property
                while ((_featureReader = GetSubReaderOf(_xmlReader, null, _featureNode)) != null)
                {
                    while ((_geomReader = GetSubReaderOf(_featureReader, null, _propertyNode)) != null)
                    {
                        bool isSelected;
                        int uid;
                        List<string> ll = ParseProperties(_geomReader, out isSelected, out uid);

                        _geomReader = GetSubReaderOf(_featureReader, labelValue, multiPointNode, pointMemberNode);

                        GeometryFactory geomFactory = new PointFactory(_geomReader, _featureTypeInfo, _fieldNames);
                        Collection<SimpleGisShape> shapePoints = geomFactory.createGeometries();

                        var points = new List<IPoint>();
                        foreach (var shp1 in shapePoints)
                        {
                            points.Add(shp1.Geometry as IPoint);
                        }

                        MultiPoint multiPoint = new MultiPoint(points.ToArray());
                        shp = new SimpleGisShape(multiPoint);
                        shp.IsSelected = isSelected;
                        shp.UID = uid;

                        _shapes.Add(shp);
                        FillShapeFields(shp, ll);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return _shapes;
        }

        #endregion
    }

    /// <summary>
    /// This class produces objects of type <see cref="MultiLineString"/>.
    /// The base class is <see cref="GeometryFactory"/>.
    /// </summary>
    internal class MultiLineStringFactory : GeometryFactory
    {
        #region Constructors


        /// <summary>
        /// Initializes a new instance of the <see cref="MultiLineStringFactory"/> class.
        /// </summary>
        /// <param name="xmlReader">An XmlReader instance</param>
        /// <param name="featureTypeInfo">A <see cref="FeatTypeInfo"/> instance providing metadata of the featuretype to query</param>
        internal MultiLineStringFactory(XmlReader xmlReader, FeatTypeInfo featureTypeInfo, IList<string> fieldnames)
            : base(xmlReader, featureTypeInfo, fieldnames)
        {
        }

        #endregion

        #region Internal Member

        /// <summary>
        /// This method produces instances of type <see cref="MultiLineString"/>.
        /// </summary>
        /// <returns>The created geometries</returns>
        internal override Collection<SimpleGisShape> createGeometries()
        {
            SimpleGisShape shp = null;

            IPathNode multiLineStringNode = new PathNode(_GMLNS, "MultiLineString", (NameTable)_xmlReader.NameTable);
            IPathNode multiCurveNode = new PathNode(_GMLNS, "MultiCurve", (NameTable)_xmlReader.NameTable);
            IPathNode multiLineStringNodeAlt = new AlternativePathNodesCollection(multiLineStringNode, multiCurveNode);
            IPathNode lineStringMemberNode = new PathNode(_GMLNS, "lineStringMember", (NameTable)_xmlReader.NameTable);
            IPathNode curveMemberNode = new PathNode(_GMLNS, "curveMember", (NameTable)_xmlReader.NameTable);
            IPathNode lineStringMemberNodeAlt = new AlternativePathNodesCollection(lineStringMemberNode, curveMemberNode);
            string[] labelValue = new string[1];

            try
            {
                ParseBoundingBox();

                // Reading the entire feature's node makes it possible to collect label values that may appear before or after the geometry property
                while ((_featureReader = GetSubReaderOf(_xmlReader, null, _featureNode)) != null)
                {
                    while ((_geomReader = GetSubReaderOf(_featureReader, null, _propertyNode)) != null)
                    {
                        bool isSelected;
                        int uid;
                        List<string> ll = ParseProperties(_geomReader, out isSelected, out uid);

                        _geomReader = GetSubReaderOf(_featureReader, labelValue, multiLineStringNodeAlt, lineStringMemberNodeAlt);

                        GeometryFactory geomFactory = new LineStringFactory(_geomReader, _featureTypeInfo, _fieldNames);
                        Collection<SimpleGisShape> shpLineStrings = geomFactory.createGeometries();

                        var lineStrings = new List<ILineString>();

                        foreach (var lineString in shpLineStrings)
                        {
                            lineStrings.Add(lineString.Geometry as ILineString);
                        }
                        
                        MultiLineString multiLineString = new MultiLineString(lineStrings.ToArray());
                        shp = new SimpleGisShape(multiLineString);
                        shp.IsSelected = isSelected;
                        shp.UID = uid;

                        _shapes.Add(shp);
                        FillShapeFields(shp, ll);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return _shapes;
        }

        #endregion
    }

    /// <summary>
    /// This class produces instances of type <see cref="MultiPolygon"/>.
    /// The base class is <see cref="GeometryFactory"/>.
    /// </summary>
    internal class MultiPolygonFactory : GeometryFactory
    {
        #region Constructors


        /// <summary>
        /// Initializes a new instance of the <see cref="MultiPolygonFactory"/> class.
        /// </summary>
        /// <param name="xmlReader">An XmlReader instance</param>
        /// <param name="featureTypeInfo">A <see cref="FeatTypeInfo"/> instance providing metadata of the featuretype to query</param>
        internal MultiPolygonFactory(XmlReader xmlReader, FeatTypeInfo featureTypeInfo, IList<string> fieldnames)
            : base(xmlReader, featureTypeInfo, fieldnames)
        {
        }

        #endregion

        #region Internal Member

        /// <summary>
        /// This method produces instances of type <see cref="MultiPolygon"/>.
        /// </summary>
        /// <returns>The created geometries</returns>
        internal override Collection<SimpleGisShape> createGeometries()
        {
            SimpleGisShape shp = null;

            IPathNode multiPolygonNode = new PathNode(_GMLNS, "MultiPolygon", (NameTable)_xmlReader.NameTable);
            IPathNode multiSurfaceNode = new PathNode(_GMLNS, "MultiSurface", (NameTable)_xmlReader.NameTable);
            IPathNode multiPolygonNodeAlt = new AlternativePathNodesCollection(multiPolygonNode, multiSurfaceNode);
            IPathNode polygonMemberNode = new PathNode(_GMLNS, "polygonMember", (NameTable)_xmlReader.NameTable);
            IPathNode surfaceMemberNode = new PathNode(_GMLNS, "surfaceMember", (NameTable)_xmlReader.NameTable);
            IPathNode polygonMemberNodeAlt = new AlternativePathNodesCollection(polygonMemberNode, surfaceMemberNode);
            IPathNode linearRingNode = new PathNode(_GMLNS, "LinearRing", (NameTable)_xmlReader.NameTable);
            string[] labelValue = new string[1];

            try
            {
                ParseBoundingBox();

                // Reading the entire feature's node makes it possible to collect label values that may appear before or after the geometry property
                while ((_featureReader = GetSubReaderOf(_xmlReader, null, _featureNode)) != null)
                {
                    while ((_geomReader = GetSubReaderOf(_featureReader, null, _propertyNode)) != null)
                    {
                        bool isSelected;
                        int uid;
                        List<string> ll = ParseProperties(_geomReader, out isSelected, out uid);

                        _geomReader = GetSubReaderOf(_featureReader, labelValue, multiPolygonNodeAlt, polygonMemberNodeAlt);

                        GeometryFactory geomFactory = new PolygonFactory(_geomReader, _featureTypeInfo, _fieldNames);
                        Collection<SimpleGisShape> shpPolygons = geomFactory.createGeometries();

                        var polygons = new List<IPolygon>();

                        foreach (var shp1 in shpPolygons)
                        {
                            polygons.Add(shp1.Geometry as IPolygon);                            
                        }

                        MultiPolygon multiPolygon = new MultiPolygon(polygons.ToArray());
                        shp = new SimpleGisShape(multiPolygon);
                        shp.IsSelected = isSelected;
                        shp.UID = uid;

                        _shapes.Add(shp);
                        FillShapeFields(shp, ll);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return _shapes;
        }

        #endregion
    }

    /// <summary>
    /// This class must detect the geometry type of the queried layer.
    /// Therefore it works a bit slower than the other factories. Specify the geometry type manually,
    /// if it isn't specified in 'DescribeFeatureType'.
    /// </summary>
    internal class UnspecifiedGeometryFactory_WFS1_0_0_GML2 : GeometryFactory
    {
        #region Fields

        private readonly bool _QuickGeometries = false;
        private bool _MultiGeometries;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="PolygonFactory"/> class.
        /// This constructor shall just be called from the MultiPolygon factory. The feature node therefore is deactivated.
        /// </summary>
        /// <param name="xmlReader">An XmlReader instance</param>
        /// <param name="featureTypeInfo">A <see cref="FeatTypeInfo"/> instance providing metadata of the featuretype to query</param>
        internal UnspecifiedGeometryFactory_WFS1_0_0_GML2(XmlReader xmlReader, FeatTypeInfo featureTypeInfo, IList<string> fieldnames)
            : base(xmlReader, featureTypeInfo, fieldnames)
        {
            _featureNode.IsActive = false;
        }

        #endregion

        #region Internal Member

        /// <summary>
        /// This method detects the geometry type from 'GetFeature' response and uses a geometry factory to create the 
        /// appropriate geometries.
        /// </summary>
        /// <returns>Collection of GIS elements</returns>
        internal override Collection<SimpleGisShape> createGeometries()
        {
            GeometryFactory geomFactory = null;

            string geometryTypeString = string.Empty;
            string serviceException = null;

            if (_QuickGeometries) _MultiGeometries = false;

            IPathNode pointNode = new PathNode(_GMLNS, "Point", (NameTable)_xmlReader.NameTable);
            IPathNode lineStringNode = new PathNode(_GMLNS, "LineString", (NameTable)_xmlReader.NameTable);
            IPathNode polygonNode = new PathNode(_GMLNS, "Polygon", (NameTable)_xmlReader.NameTable);
            IPathNode multiPointNode = new PathNode(_GMLNS, "MultiPoint", (NameTable)_xmlReader.NameTable);
            IPathNode multiLineStringNode = new PathNode(_GMLNS, "MultiLineString", (NameTable)_xmlReader.NameTable);
            IPathNode multiCurveNode = new PathNode(_GMLNS, "MultiCurve", (NameTable)_xmlReader.NameTable);
            IPathNode multiLineStringNodeAlt = new AlternativePathNodesCollection(multiLineStringNode, multiCurveNode);
            IPathNode multiPolygonNode = new PathNode(_GMLNS, "MultiPolygon", (NameTable)_xmlReader.NameTable);
            IPathNode multiSurfaceNode = new PathNode(_GMLNS, "MultiSurface", (NameTable)_xmlReader.NameTable);
            IPathNode multiPolygonNodeAlt = new AlternativePathNodesCollection(multiPolygonNode, multiSurfaceNode);

            while (_xmlReader.Read())
            {
                if (_xmlReader.NodeType == XmlNodeType.Element)
                {
                    if (_MultiGeometries)
                    {
                        if (multiPointNode.Matches(_xmlReader))
                        {
                            geomFactory = new MultiPointFactory(_xmlReader, _featureTypeInfo, _fieldNames);
                            geometryTypeString = "MultiPointPropertyType";
                            break;
                        }
                        if (multiLineStringNodeAlt.Matches(_xmlReader))
                        {
                            geomFactory = new MultiLineStringFactory(_xmlReader, _featureTypeInfo, _fieldNames);
                            geometryTypeString = "MultiLineStringPropertyType";
                            break;
                        }
                        if (multiPolygonNodeAlt.Matches(_xmlReader))
                        {
                            geomFactory = new MultiPolygonFactory(_xmlReader, _featureTypeInfo, _fieldNames);
                            geometryTypeString = "MultiPolygonPropertyType";
                            break;
                        }
                    }

                    if (pointNode.Matches(_xmlReader))
                    {
                        geomFactory = new PointFactory(_xmlReader, _featureTypeInfo, _fieldNames);
                        geometryTypeString = "PointPropertyType";
                        _featureTypeInfo.Geometry._GeometryType = "PointPropertyType";
                        break;
                    }
                    if (lineStringNode.Matches(_xmlReader))
                    {
                        geomFactory = new LineStringFactory(_xmlReader, _featureTypeInfo, _fieldNames);
                        geometryTypeString = "LineStringPropertyType";
                        break;
                    }
                    if (polygonNode.Matches(_xmlReader))
                    {
                        geomFactory = new PolygonFactory(_xmlReader, _featureTypeInfo, _fieldNames);
                        geometryTypeString = "PolygonPropertyType";
                        break;
                    }
                    if (_serviceExceptionNode.Matches(_xmlReader))
                    {
                        serviceException = _xmlReader.ReadInnerXml();
                        throw new Exception("A service exception occured: " + serviceException);
                    }
                }
            }

            _featureTypeInfo.Geometry._GeometryType = geometryTypeString;

            if (geomFactory != null)
                return geomFactory.createGeometries();
            return _shapes;
        }

        internal string DetectGeometryType()
        {
            string geometryTypeString = string.Empty;
            string serviceException = null;

            if (_QuickGeometries) _MultiGeometries = false;

            IPathNode pointNode = new PathNode(_GMLNS, "Point", (NameTable)_xmlReader.NameTable);
            IPathNode lineStringNode = new PathNode(_GMLNS, "LineString", (NameTable)_xmlReader.NameTable);
            IPathNode polygonNode = new PathNode(_GMLNS, "Polygon", (NameTable)_xmlReader.NameTable);
            IPathNode multiPointNode = new PathNode(_GMLNS, "MultiPoint", (NameTable)_xmlReader.NameTable);
            IPathNode multiLineStringNode = new PathNode(_GMLNS, "MultiLineString", (NameTable)_xmlReader.NameTable);
            IPathNode multiCurveNode = new PathNode(_GMLNS, "MultiCurve", (NameTable)_xmlReader.NameTable);
            IPathNode multiLineStringNodeAlt = new AlternativePathNodesCollection(multiLineStringNode, multiCurveNode);
            IPathNode multiPolygonNode = new PathNode(_GMLNS, "MultiPolygon", (NameTable)_xmlReader.NameTable);
            IPathNode multiSurfaceNode = new PathNode(_GMLNS, "MultiSurface", (NameTable)_xmlReader.NameTable);
            IPathNode multiPolygonNodeAlt = new AlternativePathNodesCollection(multiPolygonNode, multiSurfaceNode);

            while (_xmlReader.Read())
            {
                if (_xmlReader.NodeType == XmlNodeType.Element)
                {
                    if (_MultiGeometries)
                    {
                        if (multiPointNode.Matches(_xmlReader))
                        {
                            geometryTypeString = "MultiPointPropertyType";
                            break;
                        }
                        if (multiLineStringNodeAlt.Matches(_xmlReader))
                        {
                            geometryTypeString = "MultiLineStringPropertyType";
                            break;
                        }
                        if (multiPolygonNodeAlt.Matches(_xmlReader))
                        {
                            geometryTypeString = "MultiPolygonPropertyType";
                            break;
                        }
                    }

                    if (pointNode.Matches(_xmlReader))
                    {
                        geometryTypeString = "PointPropertyType";
                        break;
                    }
                    if (lineStringNode.Matches(_xmlReader))
                    {
                        geometryTypeString = "LineStringPropertyType";
                        break;
                    }
                    if (polygonNode.Matches(_xmlReader))
                    {
                        geometryTypeString = "PolygonPropertyType";
                        break;
                    }
                    if (_serviceExceptionNode.Matches(_xmlReader))
                    {
                        serviceException = _xmlReader.ReadInnerXml();
                        throw new Exception("A service exception occured: " + serviceException);
                    }
                }
            }

            return geometryTypeString;
        }



        #endregion
    }
}
