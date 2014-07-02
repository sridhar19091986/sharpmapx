//==============================================================================
// Copyright 2010-2014 - Fabrizio Vita (www.itacasoft.com)
// This file can be redistributed and/or modified under the terms of the GNU Lesser General Public License.
//===============================================================================

using System;

namespace SharpMap.Layers
{
    public class LayerGml: LayerVector
    {
        public LayerGml()
            : base()
        {
            
        }

        public override Entities.Extent BoundingBox
        {
            get; set;
        }

        public override void Open()
        {
            throw new NotImplementedException();
        }

        public override void Close()
        {
            throw new NotImplementedException();
        }

        public override bool IsOpened
        {
            get { throw new NotImplementedException(); }
        }

        public override Entities.GisShapeBase FindFirst()
        {
            throw new NotImplementedException();
        }

        public override Entities.GisShapeBase FindFirst(Entities.Extent extent)
        {
            throw new NotImplementedException();
        }

        public override Entities.GisShapeBase FindNext()
        {
            throw new NotImplementedException();
        }
    }
}
