// This file can be redistributed and/or modified under the terms of the GNU Lesser General Public License.

namespace SharpMap.Styles
{
    //I think this class should be replaced by doubles for Width and Height. PDD
    public class Size
    {
        public Size() :
            this(0, 0)
        {
            
        }

        public Size(double width, double height)
        {
            this.Height = height;
            this.Width = width;
        }

        public double Width { get; set; }
        public double Height { get; set; }
    }
}
