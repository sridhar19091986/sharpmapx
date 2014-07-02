//==============================================================================
// Copyright 2010-2014 - Fabrizio Vita (www.itacasoft.com)
// This file can be redistributed and/or modified under the terms of the GNU Lesser General Public License.
//===============================================================================

using SharpMap.Layers;
using System;

namespace SharpMap.Entities
{
    /// <summary>
    /// This class encapsulate a point shape
    /// </summary>
    public class GisShapePoint : GisShapeBase
    {
        /// <summary>
        /// Creates a new instance of the <see cref="GisShapePoint"/>
        /// </summary>
        public GisShapePoint(LayerVector ll)
            : base(ll)
        {
            _point = new GisPoint();
            _point.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(_point_PropertyChanged);
        }

        void _point_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            NotifyPropertyChanged(e.PropertyName);
        }

        private GisPoint _point = null;

        /// <summary>
        /// Gets the location of the shape
        /// </summary>
        public GisPoint Point
        {
            get
            {
                return _point;
            }
        }

        /// <summary>
        /// The minimum extent for this geometry.
        /// </summary>
        /// <returns></returns>
        public override Extent GetExtent()
        {
            return new Extent(_point.X, _point.Y, _point.X, _point.Y);
        }
    }
}
