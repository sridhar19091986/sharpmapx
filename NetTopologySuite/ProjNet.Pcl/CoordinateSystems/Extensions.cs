using System;
using System.Collections.Generic;
using GeoAPI.CoordinateSystems;
using GeoAPI.CoordinateSystems.Transformations;
using GeoAPI.Geometries;

namespace ProjNet.CoordinateSystems
{
	/// <summary>
	/// Silverlight Extension methods
	/// </summary>
	public static class SilverlightExtensions
	{
		/// <summary>
		/// Transforms a coordinate point.
		/// </summary>
		/// <param name="point">Input point</param>
		/// <returns>Transformed point</returns>
#if !PCL
        public static Point Transform(this IMathTransform transform, Point point)
        {
            double[] p = transform.Transform(new double[] { point.X, point.Y });
            if (p == null || p.Length < 2) return new Point(double.NaN, double.NaN);
            return new Point(p[0], p[1]);
        }
#else
        public static void Transform(this IMathTransform transform, IPoint point)
        {
            double[] p = transform.Transform(new double[] { point.X, point.Y });
            if (p == null || p.Length < 2)
            {
                point.X = double.NaN;
                point.Y = double.NaN;
                return;
            }

            point.X = p[0];
            point.Y = p[1];
        }
#endif

		internal static Parameter Find(this List<Parameter> items, Predicate<Parameter> match)
		{
			foreach (Parameter item in items)
			{
				if (match(item))
					return item;
			}
			return null;
		}

		internal static ProjectionParameter Find(this List<ProjectionParameter> items, Predicate<ProjectionParameter> match)
		{
			foreach (ProjectionParameter item in items)
			{
				if (match(item))
					return item;
			}
			return null;
		}

		//public static T Find(this List<T> items, Predicate<T> match)
		//{
		//    foreach (T item in items)
		//    {
		//        if (match(item))
		//            return item;
		//    }
		//    return default(T);
		//}
	}
}
