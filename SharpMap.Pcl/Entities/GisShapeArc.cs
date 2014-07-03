//==============================================================================
// Copyright 2010-2014 - Fabrizio Vita (www.itacasoft.com)
// This file can be redistributed and/or modified under the terms of the GNU Lesser General Public License.
//===============================================================================

using SharpMap.Layers;
using System.Collections.ObjectModel;

namespace SharpMap.Entities
{
    /// <summary>
    /// This class encapsulate an arc shape
    /// </summary>
    public class GisShapeArc: GisShapeBase
    {
        /// <summary>
        /// Creates a new instance of the <see cref="GisShapeArc"/>
        /// </summary>
        public GisShapeArc(LayerVector ll)
            :base(ll)
        {
#if SILVERLIGHT
            _points.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(_vertices_CollectionChanged);
#endif
        }

#if SILVERLIGHT
        void _vertices_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            NotifyPropertyChanged("Points");
        }
#endif

#if SILVERLIGHT
        private ObservableCollection<GIS_Point> _points = new ObservableCollection<GIS_Point>();

        /// <summary>
        /// Gets or sets the collection of vertices in this Geometry
        /// </summary>
        public virtual ObservableCollection<GIS_Point> Points
        {
            get { return _points; }
        }
#else
        private Collection<GisPoint> _points = new Collection<GisPoint>();

        /// <summary>
        /// Gets or sets the collection of vertices in this Geometry
        /// </summary>
        public virtual Collection<GisPoint> Points
        {
            get { return _points; }
        }
#endif

        /// <summary>
        /// The minimum extent for this shape.
        /// </summary>
        /// <returns>Extent for this shape</returns>
        public override Extent GetExtent()
        {
            if (Points == null || Points.Count == 0)
                return null;
            Extent bbox = new Extent(Points[0], Points[0]);
            for (int i = 1; i < Points.Count; i++)
            {
                bbox.Min.X = Points[i].X < bbox.Min.X ? Points[i].X : bbox.Min.X;
                bbox.Min.Y = Points[i].Y < bbox.Min.Y ? Points[i].Y : bbox.Min.Y;
                bbox.Max.X = Points[i].X > bbox.Max.X ? Points[i].X : bbox.Max.X;
                bbox.Max.Y = Points[i].Y > bbox.Max.Y ? Points[i].Y : bbox.Max.Y;
            }
            return bbox;
        }


    }
}
