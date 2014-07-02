//==============================================================================
// Copyright 2010-2014 - Fabrizio Vita (www.itacasoft.com)
// This file can be redistributed and/or modified under the terms of the GNU Lesser General Public License.
//===============================================================================

using System;
using System.Globalization;
using GeoAPI.Geometries;

namespace SharpMap.Entities
{
    /// <summary>
    /// This class manages an extent. An extent represents a box whose sides are parallel to the two axes of the coordinate system.
    /// </summary>
    public class Extent : IEquatable<Extent>
    {
        private GisPoint _max;
        private GisPoint _min;

        /// <summary>
        /// Min X
        /// </summary>
        public double MinX
        {
            get
            {
                return _min.X;
            }
        }

        /// <summary>
        /// Min Y
        /// </summary>
        public double MinY
        {
            get
            {
                return _min.Y;
            }
        }

        /// <summary>
        /// Max X
        /// </summary>
        public double MaxX
        {
            get
            {
                return _max.X;
            }
        }

        /// <summary>
        /// Max Y
        /// </summary>
        public double MaxY
        {
            get
            {
                return _max.Y;
            }
        }


        /// <summary>
        /// Initializes an <see cref="Extent"/>
        /// </summary>
        public Extent()
        {
            _min = new GisPoint(0, 0);
            _max = new GisPoint(0, 0);
            if (!IsValid())
                throw new ArgumentException("Min are not smaller than max");
        }

        /// <summary>
        /// Initializes an <see cref="Extent"/>
        /// </summary>
        /// <param name="minX">left</param>
        /// <param name="minY">bottom</param>
        /// <param name="maxX">right</param>
        /// <param name="maxY">top</param>
        public Extent(double minX, double minY, double maxX, double maxY)
        {
            _min = new GisPoint(minX, minY);
            _max = new GisPoint(maxX, maxY);
            if (!IsValid())
                throw new ArgumentException("Min are not smaller than max");
        }


        /// <summary>
        /// Initializes an <see cref="Extent"/>
        /// </summary>
        /// <param name="lowerLeft">Lower left corner</param>
        /// <param name="upperRight">Upper right corner</param>
        public Extent(GisPoint lowerLeft, GisPoint upperRight)
            : this(lowerLeft.X, lowerLeft.Y, upperRight.X, upperRight.Y)
        {
        }

        /// <summary>
        /// Initializes an <see cref="Extent"/>
        /// </summary>
        /// <param name="lowerLeft">Lower left corner</param>
        /// <param name="upperRight">Upper right corner</param>
#if SILVERLIGHT || WINDOWS_PHONE || PCL
        public Extent(IPoint lowerLeft, IPoint upperRight)
#else
        public Extent(System.Drawing.Point lowerLeft, System.Drawing.Point upperRight)
#endif
            : this(lowerLeft.X, lowerLeft.Y, upperRight.X, upperRight.Y)
        {
        }



        /// <summary>
        /// Gets or sets the lower left corner.
        /// </summary>
        public GisPoint Min
        {
            get { return _min; }
            set { _min = value; }
        }

        /// <summary>
        /// Gets or sets the upper right corner.
        /// </summary>
        public GisPoint Max
        {
            get { return _max; }
            set { _max = value; }
        }

        /// <summary>
        /// Gets the left boundary
        /// </summary>
        public Double Left
        {
            get { return _min.X; }
        }

        /// <summary>
        /// Gets the right boundary
        /// </summary>
        public Double Right
        {
            get { return _max.X; }
        }

        /// <summary>
        /// Gets the top boundary
        /// </summary>
        public Double Top
        {
            get { return _max.Y; }
        }

        /// <summary>
        /// Gets the bottom boundary
        /// </summary>
        public Double Bottom
        {
            get { return _min.Y; }
        }

        /// <summary>
        /// Gets the top left corner
        /// </summary>
        public GisPoint TopLeft
        {
            get { return new GisPoint(Left, Top); }
        }

        /// <summary>
        /// Gets the top right corner
        /// </summary>
        public GisPoint TopRight
        {
            get { return new GisPoint(Right, Top); }
        }

        /// <summary>
        /// Gets the bottom left corner
        /// </summary>
        public GisPoint BottomLeft
        {
            get { return new GisPoint(Left, Bottom); }
        }

        /// <summary>
        /// Gets the bottom right corner
        /// </summary>
        public GisPoint BottomRight
        {
            get { return new GisPoint(Right, Bottom); }
        }

        /// <summary>
        /// Returns the width of the <see cref="Extent"/>
        /// </summary>
        /// <returns>Width of <see cref="Extent"/></returns>
        public double Width
        {
            get { return Math.Abs(_max.X - _min.X); }
        }

        /// <summary>
        /// Returns the height of the <see cref="Extent"/>
        /// </summary>
        /// <returns>Height of <see cref="Extent"/></returns>
        public double Height
        {
            get { return Math.Abs(_max.Y - _min.Y); }
        }

       /// <summary>
        /// Checks whether the values of this instance is equal to the values of another instance.
        /// </summary>
        /// <param name="other"><see cref="Extent"/> to compare to.</param>
        /// <returns>True if equal</returns>
        public bool Equals(Extent other)
        {
            const double epsilon = 0.0000001;
            if (other == null) return false;
            return Math.Abs(MinX - other.MinX) < epsilon && Math.Abs(MaxX - other.MaxX) < epsilon && Math.Abs(MinY - other.MinY) < epsilon && Math.Abs(MaxX - other.MaxY) < epsilon;
        }

        /// <summary>
        /// Checks if min values are actually smaller than max values.
        /// </summary>
        public bool IsValid()
        {
            if (_min.X > _max.X)
            {
                return false;
            }
            if (_min.Y > _max.Y)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// Check if extent is void
        /// </summary>
        /// <param name="extent"></param>
        /// <returns></returns>
        public static bool IsVoid(Extent extent)
        {
            const double epsilon = 0.0000001;
            if (extent == null) return true;
            if ((Math.Abs(extent.MaxX - -1) < epsilon) && (Math.Abs(extent.MaxY - -1) < epsilon) && (Math.Abs(extent.MinX - -1) < epsilon))
                return true;

            return false;
        }


        /// <summary>
        /// Determines whether the extent intersects another extent
        /// </summary>
        /// <param name="extent">Extent</param>
        /// <returns></returns>
        public bool Intersects(Extent extent)
        {
            return !(extent.Min.X > Max.X ||
                     extent.Max.X < Min.X ||
                     extent.Min.Y > Max.Y ||
                     extent.Max.Y < Min.Y);
        }

        /// <summary>
        /// Returns true if this <see cref="Extent"/> intersects the geometry
        /// </summary>
        /// <param name="g">Geometry</param>
        /// <returns>True if intersects</returns>
        public bool Intersects(GisShapeBase g)
        {
            return Touches(g);
        }

        /// <summary>
        /// Returns true if this instance touches the <see cref="Extent"/>
        /// </summary>
        /// <param name="extent"><see cref="Extent"/></param>
        /// <returns>True it touches</returns>
        public bool Touches(Extent extent)
        {
            for (uint i = 0; i < 2; i++)
            {
                if ((Min[i] > extent.Min[i] && Min[i] < extent.Min[i]) || (Max[i] > extent.Max[i] && Max[i] < extent.Max[i]))
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Returns true if this <see cref="Extent"/> touches the geometry
        /// </summary>
        /// <param name="s">Geometry</param>
        /// <returns>True if touches</returns>
        public bool Touches(GisShapeBase s)
        {
            if (s is GisShapePoint) return Touches((s as GisShapePoint).Point);
            if (s is GisShapeArc)
            {
                foreach (var p in (s as GisShapeArc).Points)
                {
                    if (Touches(p)) return true;
                }
                return false;
            }

            if (s is GisShapePolygon)
            {
                foreach (var p in (s as GisShapePolygon).Points)
                {
                    if (Touches(p)) return true;
                }
                return false;
            }

            throw new NotImplementedException("Touches: Not implemented on this geometry type");
        }

        /// <summary>
        /// Returns true if this instance contains the <see cref="Extent"/>
        /// </summary>
        /// <param name="r"><see cref="Extent"/></param>
        /// <returns>True it contains</returns>
        public bool Contains(Extent r)
        {
            for (uint i = 0; i < 2; i++)
                if (Min[i] > r.Min[i] || Max[i] < r.Max[i])
                    return false;

            return true;
        }

        /// <summary>
        /// Returns true if this instance touches the <see cref="GisPoint"/>
        /// </summary>
        /// <param name="p">Geometry</param>
        /// <returns>True if touches</returns>
        public bool Touches(GisPoint p)
        {
            for (uint i = 0; i < 2; i++)
            {
                if ((Min[i] > p[i] && Min[i] < p[i]) || (Max[i] > p[i] && Max[i] < p[i]))
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Computes the joined extent of this instance and another extent
        /// </summary>
        /// <param name="extent">Extent to join with</param>
        /// <returns>Extent containing both extents</returns>
        public Extent Join(Extent extent)
        {
            if (extent == null)
                return Clone();
            return new Extent(Math.Min(Min.X, extent.Min.X), Math.Min(Min.Y, extent.Min.Y),
                              Math.Max(Max.X, extent.Max.X), Math.Max(Max.Y, extent.Max.Y));
        }


        /// <summary>
        /// Checks whether a point is contained by the extent
        /// </summary>
        /// <param name="p">Point</param>
        /// <returns>true if point is within</returns>
        public bool Contains(GisPoint p)
        {
            if (Max.X < p.X)
                return false;
            if (Min.X > p.X)
                return false;
            if (Max.Y < p.Y)
                return false;
            if (Min.Y > p.Y)
                return false;
            return true;
        }

        /// <summary>
        /// Calculates the minimum distance between this extent and a <see cref="GisPoint"/>
        /// </summary>
        /// <param name="p"><see cref="ScreenPoint"/> to calculate distance to.</param>
        /// <returns>Minimum distance.</returns>
        public virtual double Distance(GisPoint p)
        {
            double result = 0.0;

            for (uint i = 0; i < 2; i++)
            {
                if (p[i] < Min[i]) 
                    result += Math.Pow(Min[i] - p[i], 2.0);
                else if (p[i] > Max[i]) 
                    result += Math.Pow(p[i] - Max[i], 2.0);
            }

            return Math.Sqrt(result);
        }

        /// <summary>
        /// Creates a copy of the extent
        /// </summary>
        /// <returns></returns>
        public Extent Clone()
        {
            return new Extent(_min.X, _min.Y, _max.X, _max.Y);
        }

        /// <summary>
        /// Returns a string representation of the extent as LowerLeft + UpperRight formatted as "MinX,MinY MaxX,MaxY"
        /// </summary>
        /// <returns>MinX,MinY MaxX,MaxY</returns>
        public override string ToString()
        {
            return String.Format(CultureInfo.InvariantCulture, "{0},{1} {2},{3}", Min.X, Min.Y, Max.X, Max.Y);
        }

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            Extent box = obj as Extent;
            if (obj == null) return false;
            else return Equals(box);
        }

        /// <summary>
        /// Returns a hash code for the specified object
        /// </summary>
        /// <returns>A hash code for the specified object</returns>
        public override int GetHashCode()
        {
            return Min.GetHashCode() ^ Max.GetHashCode();
        }

        private bool _isMercatore = false;

        public bool IsMercatore
        {
            get
            {
                return _isMercatore;
            }
            set
            {
                _isMercatore = value;
            }
        }

        /// <summary>
        /// Returns the center of the extent
        /// </summary>
        public GisPoint Centroid
        {
            get { return (_min + _max)*.5f; }
        }
    }
}
