// This file is part of SharpMap.
// SharpMap is free software; you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation; either version 2 of the License, or
// (at your option) any later version.
// 
// SharpMap is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Lesser General Public License for more details.

// You should have received a copy of the GNU Lesser General Public License
// along with SharpMap; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA 

// SOURCECODE IS MODIFIED FROM SharpMap-SL-80096 BY ITACASOFT DI VITA FABRIZIO
// 22/02/2011: excluded what does not compile under Windows Phone

using System;
using System.Net;
using ProjNet.CoordinateSystems.Transformations;
using System.Collections;
using System.Collections.Generic;
using GeoAPI.CoordinateSystems.Transformations;
using GeoAPI.Geometries;
using NetTopologySuite.Geometries;

namespace SharpMap.Projection
{
    public static class ProjectionHelper
    {
        public static IGeometry Transform(IGeometry geometry, ICoordinateTransformation CoordinateTransformation)
        {
            if (geometry is Point)
            {
                double[] point = CoordinateTransformation.MathTransform.Transform(new double[] { ((Point)geometry).X, ((Point)geometry).Y });
                return new Point(point[0], point[1]);
            }
            else
            {
                throw new NotImplementedException("todo implement for other geometries");
            }
        }

        public static IGeometry InverseTransform(IGeometry geometry, ICoordinateTransformation CoordinateTransformation)
        {
            if (geometry is Point)
            {
                CoordinateTransformation.MathTransform.Inverse();
                double[] point = CoordinateTransformation.MathTransform.Transform(new double[] { ((Point)geometry).X, ((Point)geometry).Y });
                CoordinateTransformation.MathTransform.Inverse();
                return new Point(point[0], point[1]);
            }
            else
            {
                throw new NotImplementedException("todo implement for other geometries");
            }
        }

        public static Envelope Transform(Envelope box, ICoordinateTransformation CoordinateTransformation)
        {
            double[] point1 = CoordinateTransformation.MathTransform.Transform(new double[] { box.MinX, box.MinY });
            double[] point2 = CoordinateTransformation.MathTransform.Transform(new double[] { box.MaxX, box.MaxY });
            return new Envelope(point1[0], point1[1], point2[0], point2[1]);
        }

        public static Envelope InverseTransform(Envelope box, ICoordinateTransformation CoordinateTransformation)
        {
            CoordinateTransformation.MathTransform.Invert();
            double[] point1 = CoordinateTransformation.MathTransform.Transform(new double[] { box.MinX, box.MinY });
            double[] point2 = CoordinateTransformation.MathTransform.Transform(new double[] { box.MaxX, box.MaxY });
            CoordinateTransformation.MathTransform.Invert();
            return new Envelope(point1[0], point1[1], point2[0], point2[1]);
        }
    }
}
