//==============================================================================
// Copyright 2010-2014 - Fabrizio Vita (www.itacasoft.com)
// This file can be redistributed and/or modified under the terms of the GNU Lesser General Public License.
//===============================================================================

using SharpMap.Layers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace SharpMap.Entities
{
    public class GisShapeMultiPolygon: GisShapeBase
    {
        public class PolygonGeometry
        {
            private Collection<GisPoint> _points = new Collection<GisPoint>();
            private Collection<Collection<GisPoint>> _interiorRings = new Collection<Collection<GisPoint>>(); 

            public Collection<GisPoint> Points
            {
                get { return _points; }
            }

            public Collection<Collection<GisPoint>> InteriorRings
            {
                get { return _interiorRings; }
            }
        }

        private Collection<PolygonGeometry> _geometries = new Collection<PolygonGeometry>();

        public GisShapeMultiPolygon(LayerVector layer) : base(layer)
        {
        }

        public ICollection<PolygonGeometry> Geometries
        {
            get { return _geometries; }
        }

        public IList<GisPoint> Points
        {
            get
            {
                var result = new Collection<GisPoint>();
                foreach (var g in _geometries)
                {
                    foreach (var gisPoint in g.Points)
                    {
                        result.Add(gisPoint);
                    }
                    
                }
                return result;
            }
        }


        public override Extent GetExtent()
        {
            throw new NotImplementedException();
        }
    }
}
