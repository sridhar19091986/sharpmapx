//==============================================================================
// Copyright 2010-2014 - Fabrizio Vita (www.itacasoft.com)
// This file can be redistributed and/or modified under the terms of the GNU Lesser General Public License.
//===============================================================================

using System;
using SharpMap.Entities;
using SharpMap.Layers;

namespace SharpMap.WMS
{
    public class WmsLayerInfo: LayerBaseDrawable
    {
        private int _mercatorEpsg = 3857;
        private LegendIcons _Symbols = new LegendIcons();

        /// <summary>
        /// Gets or sets comments to the layer.
        /// </summary>
        public string Comments { get; set; }

        /// <summary>
        /// Title.
        /// </summary>
        public string Title { get; set; }


        public override Extent BoundingBox { get; set; }

        public Extent LatLonBoundingBox
        {
            get;
            set;
        }

        /// <summary>
        /// Symbols of the layer.
        /// </summary>
        public LegendIcons Symbols
        {
            get
            {
                return _Symbols;
            }
            set
            {
                _Symbols = value;
            }
        }

        /// <summary>
        /// Gets or sets the queryable status.
        /// </summary>
        public bool IsQueryable
        {
            get; set;
        }


        public int MercatorEpsg
        {
            get { return _mercatorEpsg; }
            set
            {
                if ((value == 3857) || (value == 900913))
                    _mercatorEpsg = value;
                else
                {
                    throw new ArgumentOutOfRangeException(string.Format("Invalid mercator EPSG {0}", value));
                }
            }
        }
    }
}
