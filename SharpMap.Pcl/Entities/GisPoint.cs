//==============================================================================
// Copyright 2010-2014 - Fabrizio Vita (www.itacasoft.com)
// This file can be redistributed and/or modified under the terms of the GNU Lesser General Public License.
//===============================================================================

using System;
using System.Globalization;
using System.ComponentModel;

namespace SharpMap.Entities
{
    /// <summary>
    /// A Point is a 0-dimensional geometry and represents a single location in 2D coordinate space. A Point has a x coordinate
    /// value and a y-coordinate value. The boundary of a Point is the empty set.
    /// </summary>
    public class GisPoint : IFormattable, INotifyPropertyChanged
    {
        private double _X;
        private double _Y;

        /// <summary>
        /// Initializes a new Point
        /// </summary>
        /// <param name="x">X coordinate</param>
        /// <param name="y">Y coordinate</param>
        public GisPoint(double x, double y)
        {
            _X = x;
            _Y = y;
        }

        /// <summary>
        /// Initializes a new empty Point
        /// </summary>
        public GisPoint()
            : this(0, 0)
        {

        }

        /// <summary>
        /// Create a new point by a douuble[] array
        /// </summary>
        /// <param name="point"></param>
        public GisPoint(double[] point)
        {
            if (point.Length != 2)
                throw new Exception("Only 2 dimensions are supported for points");

            _X = point[0];
            _Y = point[1];
        }

        /// <summary>
        /// Gets or sets the X coordinate of the point
        /// </summary>
        public double X
        {
            get
            {
                return _X;
            }
            set
            {
                if (_X != value)
                {
                    _X = value;
                    NotifyPropertyChanged("X");
                }
            }
        }

        /// <summary>
        /// Gets or sets the Y coordinate of the point
        /// </summary>
        public double Y
        {
            get
            {
                return _Y;
            }
            set
            {
                if (_Y != value)
                {
                    _Y = value;
                    NotifyPropertyChanged("Y");
                }
            }
        }

        /// <summary>
        /// Gets or sets the longitude of the point
        /// </summary>
        public double Longitude
        {
            get
            {
                return X;
            }
            set
            {
                X = value;
            }
        }

        /// <summary>
        /// Gets or sets the latitude of the point
        /// </summary>
        public double Latitude
        {
            get
            {
                return Y;
            }
            set
            {
                Y = value;
            }
        }

        /// <summary>
        /// Returns part of coordinate. Index 0 = X, Index 1 = Y
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public virtual double this[uint index]
        {
            get
            {
                if (index == 0)
                    return X;
                else if
                    (index == 1)
                    return Y;
                else
                    throw (new Exception("Point index out of bounds"));
            }
            set
            {
                if (index == 0)
                    X = value;
                else if (index == 1)
                    Y = value;
                else
                    throw (new Exception("Point index out of bounds"));
            }
        }

        #region IComparable<Point> Members

        /// <summary>
        /// Comparator used for ordering point first by ascending X, then by ascending Y.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public virtual int CompareTo(GisPoint other)
        {
            if (X < other.X || X == other.X && Y < other.Y)
                return -1;
            else if (X > other.X || X == other.X && Y > other.Y)
                return 1;
            else
                return 0;
        }

        #endregion

        /// <summary>
        /// exports a point into a 2-dimensional double array
        /// </summary>
        /// <returns></returns>
        public double[] ToDoubleArray()
        {
            return new double[2] { _X, _Y };
        }

        /// <summary>
        /// Returns a point based on degrees, minutes and seconds notation.
        /// For western or southern coordinates, add minus '-' in front of all longitude and/or latitude values
        /// </summary>
        /// <param name="longDegrees">Longitude degrees</param>
        /// <param name="longMinutes">Longitude minutes</param>
        /// <param name="longSeconds">Longitude seconds</param>
        /// <param name="latDegrees">Latitude degrees</param>
        /// <param name="latMinutes">Latitude minutes</param>
        /// <param name="latSeconds">Latitude seconds</param>
        /// <returns>Point</returns>
        public static GisPoint FromDMS(double longDegrees, double longMinutes, double longSeconds,
                                    double latDegrees, double latMinutes, double latSeconds)
        {
            return new GisPoint(longDegrees + longMinutes / 60 + longSeconds / 3600,
                                latDegrees + latMinutes / 60 + latSeconds / 3600);
        }

        /// <summary>
        /// Returns a 2D <see cref="ScreenPoint"/> instance from this <see cref="Point3D"/>
        /// </summary>
        /// <returns><see cref="ScreenPoint"/></returns>
        public GisPoint AsPoint()
        {
            return new GisPoint(_X, _Y);
        }

        /// <summary>
        /// This method must be overridden using 'public new [derived_data_type] Clone()'
        /// </summary>
        /// <returns>Clone</returns>
        public GisPoint Clone()
        {
            return new GisPoint(X, Y);
        }

        #region Operators

        /// <summary>
        /// Vector + Vector
        /// </summary>
        /// <param name="v1">Vector</param>
        /// <param name="v2">Vector</param>
        /// <returns>Point</returns>
        public static GisPoint operator +(GisPoint v1, GisPoint v2)
        {
            return new GisPoint(v1.X + v2.X, v1.Y + v2.Y);
        }


        /// <summary>
        /// Vector - Vector
        /// </summary>
        /// <param name="v1">Vector</param>
        /// <param name="v2">Vector</param>
        /// <returns>Cross product</returns>
        public static GisPoint operator -(GisPoint v1, GisPoint v2)
        {
            return new GisPoint(v1.X - v2.X, v1.Y - v2.Y);
        }

        /// <summary>
        /// Vector * Scalar
        /// </summary>
        /// <param name="m">Vector</param>
        /// <param name="d">Scalar (double)</param>
        /// <returns></returns>
        public static GisPoint operator *(GisPoint m, double d)
        {
            return new GisPoint(m.X * d, m.Y * d);
        }

        #endregion


        /// <summary>
        /// The inherent dimension of this Geometry object, which must be less than or equal to the coordinate dimension.
        /// </summary>
        public int Dimension
        {
            get { return 0; }
        }

        /// <summary>
        /// Checks whether this instance is spatially equal to the Point 'o'
        /// </summary>
        /// <param name="p">Point to compare to</param>
        /// <returns>True if equal, otherwise false</returns>
        public virtual bool Equals(GisPoint p)
        {
            return p != null && p.X == _X && p.Y == _Y;
        }

        /// <summary>
        /// Serves as a hash function for a particular type. <see cref="GetHashCode"/> is suitable for use 
        /// in hashing algorithms and data structures like a hash table.
        /// </summary>
        /// <returns>A hash code for the current <see cref="GetHashCode"/>.</returns>
        public override int GetHashCode()
        {
            return _X.GetHashCode() ^ _Y.GetHashCode();
        }

        /// <summary>
        /// Returns 'true' if this Geometry has no anomalous geometric points, such as self
        /// intersection or self tangency. The description of each instantiable geometric class will include the specific
        /// conditions that cause an instance of that class to be classified as not simple.
        /// </summary>
        /// <returns>true if the geometry is simple</returns>
        public bool IsSimple()
        {
            return true;
        }

        /// <summary>
        /// Returns the distance between this geometry instance and another geometry, as
        /// measured in the spatial reference system of this instance.
        /// </summary>
        /// <param name="p">Point</param>
        /// <returns>Distance</returns>
        public double Distance(GisPoint p)
        {
            return Math.Sqrt(Math.Pow(X - p.X, 2) + Math.Pow(Y - p.Y, 2));
        }

        /// <summary>
        /// Returns the distance between this point and a <see cref="Extent"/>
        /// </summary>
        /// <param name="extent"></param>
        /// <returns>Distance</returns>
        public double Distance(Extent extent)
        {
            return extent.Distance(new GisPoint(_X, _Y));
        }

        /// <summary>
        /// The minimum extent for this Geometry.
        /// </summary>
        /// <returns>Extent</returns>
        public Extent GetExtent()
        {
            return new Extent(X, Y, X, Y);
        }

        /// <summary>
        /// Checks whether this point touches a <see cref="Extent"/>
        /// </summary>
        /// <param name="extent">extent</param>
        /// <returns>true if they touch</returns>
        public bool Touches(Extent extent)
        {
            return extent.Touches(new GisPoint(_X, _Y));
        }

        /// <summary>
        /// Checks whether this point touches another <see cref="Geometry"/>
        /// </summary>
        /// <param name="geom">Geometry</param>
        /// <returns>true if they touch</returns>
        public bool Touches(GisPoint geom)
        {
            if (Equals(geom)) return true;
            else return false;
        }

        /// <summary>
        /// Checks whether this point intersects a <see cref="Extent"/>
        /// </summary>
        /// <param name="box">Box</param>
        /// <returns>True if they intersect</returns>
        public bool Intersects(Extent box)
        {
            return box.Contains(new GisPoint(_X, _Y));
        }


        #region IFormattable Members
        string IFormattable.ToString(string format, IFormatProvider provider)
        {
            return this.ConvertToString(format, provider);
        }

        /// <summary>
        /// Converts the GIS_Point to string
        /// </summary>
        /// <returns>String</returns>
        public override string ToString()
        {
            return this.ConvertToString(null, null);
        }

        /// <summary>
        /// Converts the GIS_Point to a string.
        /// </summary>
        /// <param name="provider">Provider</param>
        /// <returns>String</returns>
        public string ToString(IFormatProvider provider)
        {
            return this.ConvertToString(null, provider);
        }

        internal string ConvertToString(string format, IFormatProvider provider)
        {
            char numericListSeparator = StringUtils.GetNumericListSeparator(provider);
            return string.Format(provider, "{1:" + format + "}{0}{2:" + format + "}", new object[] { numericListSeparator, this._X, this._Y });
        }

        public static GisPoint ConvertFromString(string value)
        {
            string str = value as string;
            if (str == null)
            {
                throw new NotSupportedException("Invalid GisPoint format");
            }
            string[] strArray = str.Split(new char[] { ',' });
            switch (strArray.Length)
            {
                case 2:
                    double num;
                    double num2;
                    if (!double.TryParse(strArray[0], NumberStyles.Float, CultureInfo.InvariantCulture, out num) || !double.TryParse(strArray[1], NumberStyles.Float, CultureInfo.InvariantCulture, out num2))
                    {
                        break;
                    }
                    return new GisPoint(num, num2);
            }
            throw new FormatException("Invalid GisPoint format");

        }

        #endregion

        /// <summary>
        /// Triggers the property changed event.
        /// </summary>
        /// <param name="propertyName">The name of the changed property.</param>
        protected void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                var args = new PropertyChangedEventArgs(propertyName);
                PropertyChanged(this, args);
            }
        }

        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

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
    }
}

