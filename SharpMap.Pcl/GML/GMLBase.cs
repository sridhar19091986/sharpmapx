//==============================================================================
// Copyright 2010-2014 - Fabrizio Vita (www.itacasoft.com)
// This file can be redistributed and/or modified under the terms of the GNU Lesser General Public License.
//===============================================================================

namespace SharpMap.GML
{
    /// <summary>
    /// Base class for management of GML files.
    /// </summary>
    public class GMLBase
    {
        /// <summary>
        /// Costante.
        /// </summary>
        protected const string GML_FEATURE_MEMBER = "gml:featureMember";
        /// <summary>
        /// Costante.
        /// </summary>
        protected const string GML_GEOMETRY_MEMBER = "geometryMember";
        /// <summary>
        /// Costante.
        /// </summary>
        protected const string GML_POLYGON_MEMBER = "polygonMember";
        /// <summary>
        /// Costante.
        /// </summary>
        protected const string GML_MULTI_GEOMETRY = "gml:MultiGeometry";
        /// <summary>
        /// Costante.
        /// </summary>
        protected const string GML_FEATURE_PREFIX = "Member";
        /// <summary>
        /// Costante.
        /// </summary>
        protected const string GML_POINT = "gml:Point";
        /// <summary>
        /// Costante.
        /// </summary>
        protected const string GML_MULTI_POINT = "gml:MultiPoint";
        /// <summary>
        /// Costante.
        /// </summary>
        protected const string GML_LINE_STRING = "gml:LineString";
        /// <summary>
        /// Costante.
        /// </summary>
        protected const string GML_MULTI_LINE = "gml:MultiLineString";
        /// <summary>
        /// Costante.
        /// </summary>
        protected const string GML_LINE_MEMBER = "gml:lineStringMember";
        /// <summary>
        /// Costante.
        /// </summary>
        protected const string GML_POLYGON = "gml:Polygon";
        /// <summary>
        /// Costante.
        /// </summary>
        protected const string GML_MULTI_POLYGON = "gml:MultiPolygon";
        /// <summary>
        /// Costante.
        /// </summary>
        protected const string GML_CURVE = "gml:Curve";
        /// <summary>
        /// Costante.
        /// </summary>
        protected const string GML_CURVE_MEMBER = "curveMember";
        /// <summary>
        /// Costante.
        /// </summary>
        protected const string GML_SURFACE = "gml:Surface";
        /// <summary>
        /// Costante.
        /// </summary>
        protected const string GML_LINEAR_RING = "gml:LinearRing";
        /// <summary>
        /// Costante.
        /// </summary>
        protected const string GML_INNERBOUND = "gml:innerBoundaryIs";
        /// <summary>
        /// Costante.
        /// </summary>
        protected const string GML_OUTERBOUND = "gml:outerBoundaryIs";
        /// <summary>
        /// Costante.
        /// </summary>
        protected const string GML_INTERIOR = "gml:interior";
        /// <summary>
        /// Costante.
        /// </summary>
        protected const string GML_EXTERIOR = "gml:exterior";
        /// <summary>
        /// Costante.
        /// </summary>
        protected const string GML_COORDINATES = "gml:coordinates";
        /// <summary>
        /// Costante.
        /// </summary>
        protected const string GML_COORDINATES2 = "gml:pos";
        /// <summary>
        /// Costante.
        /// </summary>
        protected const string GML_COORDINATES3 = "gml:posList";
        /// <summary>
        /// Costante.
        /// </summary>
        protected const string GML_COORD = "gml:coord";
        /// <summary>
        /// Costante.
        /// </summary>
        protected const string GML_BOX = "gml:Box";
        /// <summary>
        /// Costante.
        /// </summary>
        protected const string GML_BOUNDED_BY = "gml:boundedBy";
        /// <summary>
        /// Costante.
        /// </summary>
        protected const string GML_ARC = "Arc";
        /// <summary>
        /// Costante.
        /// </summary>
        protected const string GML_CIRCLE_CENTER = "CircleByCenterPoint";
        /// <summary>
        /// Costante.
        /// </summary>
        protected const string GML_POINT_MEMBER = "pointMember";
        /// <summary>
        /// Costante.
        /// </summary>
        protected const string GML_RADIUS = "gml:radius";
        /// <summary>
        /// Costante.
        /// </summary>
        protected const string GML_COORD_X = "gml:X";
        /// <summary>
        /// Costante.
        /// </summary>
        protected const string GML_COORD_Y = "gml:Y";
        /// <summary>
        /// Costante.
        /// </summary>
        protected const string GML_COORD_Z = "gml:Z";
        /// <summary>
        /// Costante.
        /// </summary>
        protected const string GML_DESCRIPTION = "description";
        /// <summary>
        /// Costante.
        /// </summary>
        protected const string GML_NAME = "name";
        /// <summary>
        /// Costante.
        /// </summary>
        protected const string GML_SRS_DIMENSION = "srsDimension";
        /// <summary>
        /// Costante.
        /// </summary>
        protected const string GML_PROPERTY_MEMBER = "g:PROPERTIES";
        /// <summary>
        /// Costante.
        /// </summary>
        protected const string GML_PROPERTY = "Property";
        /// <summary>
        /// Costante.
        /// </summary>
        protected const string GML_GEOMETRY = "GEOMETRY";
        /// <summary>
        /// Costante.
        /// </summary>
        protected const string GML = "gml:";
        /// <summary>
        /// Costante.
        /// </summary>
        protected const string GML_G = "g:";
        /// <summary>
        /// Costante.
        /// </summary>
        protected const string GML_XMLNS_GML = "xmlns:gml";
        /// <summary>
        /// Costante.
        /// </summary>
        protected const string GML_XMLNS_GML_G = "xmlns:g";
        /// <summary>
        /// Costante.
        /// </summary>
        protected const string GML_XMLNS_GML_WWW = "http://www.opengis.net/gml";
        /// <summary>
        /// Costante.
        /// </summary>
        protected const string GML_XMLNS_GML_G_WWW = "http://www.itacasoft.com/GML";
        /// <summary>
        /// Costante.
        /// </summary>
        protected const string GML_XMLNS_XSI = "xmlns:xsi";
        /// <summary>
        /// Costante.
        /// </summary>
        protected const string GML_XMLNS_XSI_WWW = "http://www.w3.org/2001/XMLSchema-instance";
        /// <summary>
        /// Costante.
        /// </summary>
        protected const string GML_XMLNS_SCHEMA = "xsi:schemaLocation";
        /// <summary>
        /// Costante.
        /// </summary>
        protected const string GML_XMLNS_SCHEMA_L = "http://www.opengis.net/gml feature.xsd http://www.tatukgis.com/GML ";
        /// <summary>
        /// Costante.
        /// </summary>
        protected const string NOVAL = "";
        /// <summary>
        /// Costante.
        /// </summary>
        protected const string GML_ITACASOFT = "g:ITACASOFT";
        /// <summary>
        /// Costante.
        /// </summary>
        protected const string ITACASOFT = "ITACASOFT";
    }
}
