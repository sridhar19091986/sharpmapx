//==============================================================================
// Copyright 2010-2014 - Fabrizio Vita (www.itacasoft.com)
// This file can be redistributed and/or modified under the terms of the GNU Lesser General Public License.
//===============================================================================

using SharpMap.Layers;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace SharpMap.Entities
{
    /// <summary>
    /// This class encapsulate a multipoint shape
    /// </summary>
    public class GisShapeMultiPoint: GisShapeBase
    {
        /// <summary>
        /// Creates a new instance of the <see cref="GisShapeMultiPoint"/>
        /// </summary>
        public GisShapeMultiPoint(LayerVector ll)
            :base(ll)
        {
#if SILVERLIGHT
            _points.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(_points_CollectionChanged);
#endif
        }

#if SILVERLIGHT
        void _points_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            NotifyPropertyChanged("Points");
        }
#endif

#if SILVERLIGHT
        private ObservableCollection<GIS_Point> _points = new ObservableCollection<GIS_Point>();
#else
        private Collection<GisPoint> _points = new Collection<GisPoint>();
#endif

        /// <summary>
        /// Points of the shape
        /// </summary>
        public IList<GisPoint> Points
        {
            get { return _points; }
        }

        /// <summary>
        /// The minimum extent for this Geometry.
        /// </summary>
        /// <returns></returns>
        public override Extent GetExtent()
        {
            if (_points == null || _points.Count == 0)
                return null;
            var bbox = new Extent(_points[0], _points[0]);
            for (int i = 1; i < _points.Count; i++)
            {
                bbox.Min.X = _points[i].X < bbox.Min.X ? _points[i].X : bbox.Min.X;
                bbox.Min.Y = _points[i].Y < bbox.Min.Y ? _points[i].Y : bbox.Min.Y;
                bbox.Max.X = _points[i].X > bbox.Max.X ? _points[i].X : bbox.Max.X;
                bbox.Max.Y = _points[i].Y > bbox.Max.Y ? _points[i].Y : bbox.Max.Y;
            }
            return bbox;
        }
    }
}
