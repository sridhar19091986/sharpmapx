//==============================================================================
// Copyright 2010-2014 - Fabrizio Vita (www.itacasoft.com)
// This file can be redistributed and/or modified under the terms of the GNU Lesser General Public License.
//===============================================================================

using System.IO;

namespace SharpMap.Layers
{
    public abstract class LayerPixel : LayerBaseDrawable
    {
        public abstract bool Draw();
        public abstract string ImageFormat { get; set; }
        public abstract Stream Image { get; }
    }
}
