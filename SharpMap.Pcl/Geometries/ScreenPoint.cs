//==============================================================================
// Copyright 2010-2014 - Fabrizio Vita (www.itacasoft.com)
// This file can be redistributed and/or modified under the terms of the GNU Lesser General Public License.
//===============================================================================

namespace SharpMap.Geometries
{
    public class ScreenPoint
    {
        public ScreenPoint():
            this(0,0)
        {
        }

        
        public ScreenPoint(int x, int y)
        {
            X = x;
            Y = y;
        }
        public int X { get; set; }
        public int Y { get; set; }

        public static ScreenPoint Empty
        {
            get { return new ScreenPoint(0, 0); }
        }
    }
}
