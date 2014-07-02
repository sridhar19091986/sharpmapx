//==============================================================================
// Copyright 2010-2014 - Fabrizio Vita (www.itacasoft.com)
// This file can be redistributed and/or modified under the terms of the GNU Lesser General Public License.
//===============================================================================

namespace SharpMap.Geometries
{
    public class ScreenPointF
    {
        public ScreenPointF():
            this(0,0)
        {
        }

        
        public ScreenPointF(float x, float y)
        {
            X = x;
            Y = y;
        }
        public float X { get; set; }
        public float Y { get; set; }

        public static ScreenPointF Empty
        {
            get { return new ScreenPointF(0, 0); }
        }
    }
}
