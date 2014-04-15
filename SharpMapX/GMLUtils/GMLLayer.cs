// Copyright 2011, 2014 - Fabrizio Vita (www.itacasoft.com)
// This file can be redistributed and/or modified under the terms of the GNU Lesser General Public License.

using System.Collections.Generic;
using GeoAPI.Geometries;

namespace SharpMap.GMLUtils
{
    public class GMLLayer
    {
        public GMLLayer()
        {

        }

        GMLLayerFields _fields = new GMLLayerFields();

        public string Name { get; set; }
        
        public GMLLayerFields Fields {
            get
            {
                return _fields;
            }
            set
            {
                _fields = value;
            }
        }
        
        public GMLShape CreateShape(IGeometry geometry)
        {
            GMLShape shp = new GMLShape();
            shp.Geometry = geometry;
            return shp;
        }

        public GMLShapeType DefaultShapeType { get; set; }

        public string GetShapeTypeAsGML()
        {
            switch (DefaultShapeType)
            {
                case GMLShapeType.gisShapeTypePoint:
                    return "PointPropertyType";
                case GMLShapeType.gisShapeTypeMultiPoint:
                    return "MultiPointPropertyType";
                case GMLShapeType.gisShapeTypeArc:
                    return "LineStringPropertyType";
                case GMLShapeType.gisShapeTypePolygon:
                    return "PolygonPropertyType";
                default:
                    return "";
            }
        }
    }

    public class GMLLayerField
    {
        public string Name { get; set; }
    }

    public class GMLLayerFields : List<GMLLayerField>
    {

    }
}
