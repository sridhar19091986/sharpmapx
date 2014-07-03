//==============================================================================
// Copyright 2010-2014 - Fabrizio Vita (www.itacasoft.com)
// This file can be redistributed and/or modified under the terms of the GNU Lesser General Public License.
//===============================================================================

namespace SharpMap.Layers
{
    public abstract class LayerBaseDrawable: LayerBase
    {
        private IMapViewer _viewer;

        public IMapViewer Viewer
        {
            get { return _viewer; }
                
            set
            {
                if (value == null)
                {
                    _viewer = null;
                    return;
                }

                if (value != _viewer)
                {
                    _viewer = value;
                    if (!_viewer.Layers.Contains(this))
                        _viewer.Layers.Add(this);
                }
            }
        }
    }
}
