//==============================================================================
// Copyright 2010-2014 - Fabrizio Vita (www.itacasoft.com)
// This file can be redistributed and/or modified under the terms of the GNU Lesser General Public License.
//===============================================================================

using System;
using System.Net;
using System.Windows;
using System.Windows.Input;
using SharpMap.Entities;
using NetTopologySuite;
using GeoAPI.CoordinateSystems;
using ProjNet.CoordinateSystems;

namespace SharpMap.Utilities
{
    public class ProjectionConversion
    {
        public static Extent ConvertToMercatore(Extent extent)
        {
            NetTopologySuite.Geometries.PrecisionModel precisionModel = new NetTopologySuite.Geometries.PrecisionModel(GeoAPI.Geometries.PrecisionModels.Floating);

            CoordinateSystem wgs84 = GeographicCoordinateSystem.WGS84 as CoordinateSystem;
            CoordinateSystem mercatore = ProjectedCoordinateSystem.WebMercator as CoordinateSystem;
            ICoordinateSystemFactory cFac = new  CoordinateSystemFactory();

            int SRID_wgs84 = Convert.ToInt32(wgs84.AuthorityCode);    //WGS84 SRID
            int SRID_mercatore = Convert.ToInt32(mercatore.AuthorityCode); //Mercatore SRID

            ProjNet.CoordinateSystems.Transformations.CoordinateTransformationFactory ctFact = new ProjNet.CoordinateSystems.Transformations.CoordinateTransformationFactory();
            GeoAPI.CoordinateSystems.Transformations.ICoordinateTransformation transformation = ctFact.CreateFromCoordinateSystems(wgs84, mercatore);

            NetTopologySuite.Geometries.GeometryFactory factory_wgs84 = new NetTopologySuite.Geometries.GeometryFactory(precisionModel, SRID_wgs84);

            var bottomLeft = factory_wgs84.CreatePoint(new GeoAPI.Geometries.Coordinate(extent.Left, extent.Bottom, 0));
            var topRight = factory_wgs84.CreatePoint(new GeoAPI.Geometries.Coordinate(extent.Right, extent.Top, 0));

            double[] coords_bl = transformation.MathTransform.Transform(new double[] { bottomLeft.X, bottomLeft.Y });
            double[] coords_tr = transformation.MathTransform.Transform(new double[] { topRight.X, topRight.Y });

            NetTopologySuite.Geometries.GeometryFactory factory_mercatore = new NetTopologySuite.Geometries.GeometryFactory(precisionModel, SRID_mercatore);

            var p1_bl = factory_mercatore.CreatePoint(new GeoAPI.Geometries.Coordinate(coords_bl[0], coords_bl[1]));
            var p2_tr = factory_mercatore.CreatePoint(new GeoAPI.Geometries.Coordinate(coords_tr[0], coords_tr[1]));

            return new Extent(p1_bl.X, p1_bl.Y, p2_tr.X, p2_tr.Y);
        }

        public static GisPoint ConvertToMercatore(GisPoint point)
        {
            NetTopologySuite.Geometries.PrecisionModel precisionModel = new NetTopologySuite.Geometries.PrecisionModel(GeoAPI.Geometries.PrecisionModels.Floating);

            CoordinateSystem wgs84 = GeographicCoordinateSystem.WGS84 as CoordinateSystem;
            CoordinateSystem mercatore = ProjectedCoordinateSystem.WebMercator as CoordinateSystem;
            ICoordinateSystemFactory cFac = new CoordinateSystemFactory();

            int SRID_wgs84 = Convert.ToInt32(wgs84.AuthorityCode);    //WGS84 SRID
            int SRID_mercatore = Convert.ToInt32(mercatore.AuthorityCode); //Mercatore SRID

            ProjNet.CoordinateSystems.Transformations.CoordinateTransformationFactory ctFact = new ProjNet.CoordinateSystems.Transformations.CoordinateTransformationFactory();
            GeoAPI.CoordinateSystems.Transformations.ICoordinateTransformation transformation = ctFact.CreateFromCoordinateSystems(wgs84, mercatore);

            NetTopologySuite.Geometries.GeometryFactory factory_wgs84 = new NetTopologySuite.Geometries.GeometryFactory(precisionModel, SRID_wgs84);

            var convertedPoint = factory_wgs84.CreatePoint(new GeoAPI.Geometries.Coordinate(point.X, point.Y, 0));

            double[] coords = transformation.MathTransform.Transform(new double[] { convertedPoint.X, convertedPoint.Y });

            NetTopologySuite.Geometries.GeometryFactory factory_mercatore = new NetTopologySuite.Geometries.GeometryFactory(precisionModel, SRID_mercatore);

            var p1 = factory_mercatore.CreatePoint(new GeoAPI.Geometries.Coordinate(coords[0], coords[1]));

            return new GisPoint(p1.X, p1.Y);
        }

        public static GisPoint ConvertToWgs84(GisPoint point)
        {
            NetTopologySuite.Geometries.PrecisionModel precisionModel = new NetTopologySuite.Geometries.PrecisionModel(GeoAPI.Geometries.PrecisionModels.Floating);

            CoordinateSystem wgs84 = GeographicCoordinateSystem.WGS84 as CoordinateSystem;
            CoordinateSystem mercatore = ProjectedCoordinateSystem.WebMercator as CoordinateSystem;
            ICoordinateSystemFactory cFac = new CoordinateSystemFactory();

            int SRID_wgs84 = Convert.ToInt32(wgs84.AuthorityCode);    //WGS84 SRID
            int SRID_mercatore = Convert.ToInt32(mercatore.AuthorityCode); //Mercatore SRID

            ProjNet.CoordinateSystems.Transformations.CoordinateTransformationFactory ctFact = new ProjNet.CoordinateSystems.Transformations.CoordinateTransformationFactory();
            GeoAPI.CoordinateSystems.Transformations.ICoordinateTransformation transformation = ctFact.CreateFromCoordinateSystems(mercatore, wgs84);

            NetTopologySuite.Geometries.GeometryFactory factory_mercatore = new NetTopologySuite.Geometries.GeometryFactory(precisionModel, SRID_mercatore);

            var convertedPoint = factory_mercatore.CreatePoint(new GeoAPI.Geometries.Coordinate(point.X, point.Y, 0));

            double[] coords = transformation.MathTransform.Transform(new double[] { convertedPoint.X, convertedPoint.Y });

            NetTopologySuite.Geometries.GeometryFactory factory_wgs84 = new NetTopologySuite.Geometries.GeometryFactory(precisionModel, SRID_wgs84);

            var p1 = factory_wgs84.CreatePoint(new GeoAPI.Geometries.Coordinate(coords[0], coords[1]));

            return new GisPoint(p1.X, p1.Y);
        }

        public static Extent ConvertToWgs84(Extent extent)
        {
            GisPoint bottomLeft = ConvertToWgs84(extent.BottomLeft);
            GisPoint topRight = ConvertToWgs84(extent.TopRight);

            return new Extent(bottomLeft, topRight);
        }

    }
}
