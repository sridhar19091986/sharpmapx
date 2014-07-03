//==============================================================================
// Copyright 2010-2014 - Fabrizio Vita (www.itacasoft.com)
// This file can be redistributed and/or modified under the terms of the GNU Lesser General Public License.
//===============================================================================

using SharpMap.Entities;
using System;

namespace SharpMap.Entities
{
    /// <summary>
    /// EventArgs class for a list of geometrical entities.
    /// </summary>
    public class FeaturesEventArgs : EventArgs
    {
        /// <summary>
        /// Creates a new instance of the <see cref="SelectionsEventArgs"/>
        /// </summary>
        /// <param name="features">List of entities.</param>
        public FeaturesEventArgs(GisShapeCollection features)
        {
            _features = features;
        }

        GisShapeCollection _features;

        /// <summary>
        /// List of entities.
        /// </summary>
        public GisShapeCollection Features
        {
            get
            {
                return _features;
            }
            set
            {
                _features = value;
            }
        }
    }
}
