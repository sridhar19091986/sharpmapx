//==============================================================================
// Copyright 2010-2014 - Fabrizio Vita (www.itacasoft.com)
// This file can be redistributed and/or modified under the terms of the GNU Lesser General Public License.
//===============================================================================

using System.Collections.Generic;
using System;
using System.Collections.ObjectModel;
using SharpMap.Layers;

namespace SharpMap.Entities
{
    /// <summary>
    /// This class encapsulate a polygon shape
    /// </summary>
    public class GisShapePolygon: GisShapeBase
    {
        /// <summary>
        /// Creates a new instance of the <see cref="GisShapePolygon"/>
        /// </summary>
        public GisShapePolygon(LayerVector ll)
            :base(ll)
        {
#if SILVERLIGHT
            _points.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(_exteriorRing_CollectionChanged);
#endif
        }

#if SILVERLIGHT
        void _exteriorRing_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            NotifyPropertyChanged("Points");
        }
#endif

#if SILVERLIGHT
        private ObservableCollection<GIS_Point> _points = new ObservableCollection<GIS_Point>();
#else
        private Collection<GisPoint> _points = new Collection<GisPoint>();
#endif

        private Collection<Collection<GisPoint>> _interiorRings = new Collection<Collection<GisPoint>>();
        
        /// <summary>
        /// List of vertices of the exterior ring of the polygon
        /// </summary>
        public IList<GisPoint> Points
        {
            get { return _points; }
        }

        /// <summary>
        /// List of vertices of the exterior ring of the polygon
        /// </summary>
        public IList<GisPoint> ExteriorRing
        {
            get { return _points; }
        }

        /// <summary>
        /// List of interior rings of the polygon if any
        /// </summary>
        public IList<Collection<GisPoint>> InteriorRings
        {
            get { return _interiorRings; }
        }
        
        /// <summary>
        /// Returns the extent of the object
        /// </summary>
        /// <returns>extent</returns>
        public override Extent GetExtent()
        {
            if (_points == null || _points.Count == 0) return null;
            var bbox = new Extent(_points[0], _points[0]);
            for (int i = 1; i < _points.Count; i++)
            {
                bbox.Min.X = Math.Min(_points[i].X, bbox.Min.X);
                bbox.Min.Y = Math.Min(_points[i].Y, bbox.Min.Y);
                bbox.Max.X = Math.Max(_points[i].X, bbox.Max.X);
                bbox.Max.Y = Math.Max(_points[i].Y, bbox.Max.Y);
            }
            return bbox;
        }

        public GisPoint Centroid
        {
            get
            {
                throw new NotImplementedException();
            }
        }
    }
}
