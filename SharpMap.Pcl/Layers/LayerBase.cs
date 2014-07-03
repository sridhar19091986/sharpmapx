//==============================================================================
// Copyright 2010-2014 - Fabrizio Vita (www.itacasoft.com)
// This file can be redistributed and/or modified under the terms of the GNU Lesser General Public License.
//===============================================================================

using SharpMap.Entities;

namespace SharpMap.Layers
{
    public abstract class LayerBase: INamedEntity
    {
        public virtual bool Visible { get; set; }

        public abstract Extent BoundingBox { get; set; }

        /// <summary>
        /// Name of the Layer.
        /// </summary>
        protected string name;
        /// <summary>
        /// Name of the layer
        /// </summary>
        public string Name
        {
            get
            {
                return name;
            }
            set
            {
                name = value;
            }
        }

    }
}
