//==============================================================================
// Copyright 2010-2014 - Fabrizio Vita (www.itacasoft.com)
// This file can be redistributed and/or modified under the terms of the GNU Lesser General Public License.
//===============================================================================

using System;
using System.Collections.Generic;
using GeoAPI.Geometries;
using NetTopologySuite.Geometries;
using SharpMap.Entities;
using SharpMap.Layers;

namespace SharpMap.GmlUtils
{
    /// <summary>
    /// Provides access to SharpMap utilities for parsing GML
    /// </summary>
    /// <remarks>Obsolete, use GMLReader</remarks>
    public static class GmlReader
    {
        /// <summary>
        /// Returns a list of shapes contained in the passed GML string
        /// </summary>
        /// <param name="gml">GML string</param>
        /// <param name="layer">Layer where to load features</param>
        /// <param name="extent">extent of the shapes</param>
        /// <returns>List of shapes</returns>
        public static GisShapeCollection GetShapes(string gml, LayerVector layer, out Extent extent)
        {
            var res = new GisShapeCollection();
            res.Name = layer.Name;

            var slayer = new List<string>();

            foreach (LayerField t in layer.Fields)
            {
                slayer.Add(t.Name);
            }

            var provider = new SharpMap.GmlUtils.GmlProvider(gml, slayer);

            if (provider.Shapes == null)
            {
                extent = new Extent(provider.Extent.MinX,provider.Extent.MinY,provider.Extent.MaxX,provider.Extent.MaxY);
                return res;
            }

            for (int i = 0; i < provider.Shapes.Count; i++)
            {
                var shpOrig = provider.Shapes[i];
                GisShapeBase shpDest = layer.CreateShape();

                if (shpDest is GisShapePoint)
                {
                    (shpDest as GisShapePoint).Point.X = (shpOrig.Geometry as Point).X;
                    (shpDest as GisShapePoint).Point.Y = (shpOrig.Geometry as Point).Y;
                }
                else if (shpDest is GisShapeArc)
                {
                    for (int j=0; j<(shpOrig.Geometry as LineString).Coordinates.Length; j++)
                    {
                        var p = new GisPoint();
                        p.X = (shpOrig.Geometry as LineString).Coordinates[j].X;
                        p.Y = (shpOrig.Geometry as LineString).Coordinates[j].Y;
                        (shpDest as GisShapeArc).Points.Add(p);
                    }
                }
                else if (shpDest is GisShapeMultiPoint)
                {
                    for (int j = 0; j < (shpOrig.Geometry as MultiPoint).Geometries.Length; j++)
                    {
                        var p = new GisPoint();
                        p.X = ((shpOrig.Geometry as MultiPoint).Geometries[j] as IPoint).X;
                        p.Y = ((shpOrig.Geometry as MultiPoint).Geometries[j] as IPoint).Y;
                        (shpDest as GisShapeMultiPoint).Points.Add(p);
                    }
                }
                else if (shpDest is GisShapePolygon)
                {
                    for (int j = 0; j < (shpOrig.Geometry as Polygon).ExteriorRing.Coordinates.Length; j++)
                    {
                        var p = new GisPoint();
                        p.X = (shpOrig.Geometry as Polygon).ExteriorRing.Coordinates[j].X;
                        p.Y = (shpOrig.Geometry as Polygon).ExteriorRing.Coordinates[j].Y;
                        (shpDest as GisShapePolygon).Points.Add(p);
                    }
                }

                shpDest.UID = shpOrig.UID;
                shpDest.IsSelected = shpOrig.IsSelected;

                IEnumerable<string> keys = shpOrig.Keys;
                foreach(string key in keys)
                {
                    shpDest[key] = shpOrig[key];
                }

                if (shpDest != null)
                    res.Add(shpDest);
            }

            if (provider.Extent != null)
                extent = new Extent(provider.Extent.MinX, provider.Extent.MinY, provider.Extent.MaxX, provider.Extent.MaxY);
            else
                extent = new Extent();

            return res;
        }

        /// <summary>
        /// Returns a list of features contained in the passed GML string
        /// </summary>
        /// <param name="gml">GML string</param>
        /// <param name="extent">extent of the shapes</param>
        /// <returns>List of features</returns>
        /// <remarks>Because the layer is not passed, all fields are treated as strings</remarks>
        public static GisShapeCollection GetShapes(string gml, out Extent extent)
        {
            var res = new GisShapeCollection();
            res.Name = "";

            var provider = new SharpMap.GmlUtils.GmlProvider(gml);

            if (provider.Shapes == null)
            {
                extent = new Extent(provider.Extent.MinX, provider.Extent.MinY, provider.Extent.MaxX, provider.Extent.MaxY);
                return res;
            }

            for (int i = 0; i < provider.Shapes.Count; i++)
            {
                var shpOrig = provider.Shapes[i];
                GisShapeBase shpDest = null;

                if (shpOrig.Geometry is IPoint)
                {
                    shpDest = CreateShape(GisShapeType.GisShapeTypePoint);
                    (shpDest as GisShapePoint).Point.X = (shpOrig.Geometry as Point).X;
                    (shpDest as GisShapePoint).Point.Y = (shpOrig.Geometry as Point).Y;
                }
                else if (shpOrig.Geometry is ILineString)
                {
                    shpDest = CreateShape(GisShapeType.GisShapeTypeArc);
                    for (int j = 0; j < (shpOrig.Geometry as LineString).Coordinates.Length; j++)
                    {
                        var p = new GisPoint();
                        p.X = (shpOrig.Geometry as LineString).Coordinates[j].X;
                        p.Y = (shpOrig.Geometry as LineString).Coordinates[j].Y;
                        (shpDest as GisShapeArc).Points.Add(p);
                    }
                }
                else if (shpOrig.Geometry is IMultiPoint)
                {
                    shpDest = CreateShape(GisShapeType.GisShapeTypeMultiPoint);
                    for (int j = 0; j < (shpOrig.Geometry as MultiPoint).Geometries.Length; j++)
                    {
                        var p = new GisPoint();
                        p.X = ((shpOrig.Geometry as MultiPoint).Geometries[j] as IPoint).X;
                        p.Y = ((shpOrig.Geometry as MultiPoint).Geometries[j] as IPoint).Y;
                        (shpDest as GisShapeMultiPoint).Points.Add(p);
                    }
                }
                else if (shpOrig.Geometry is IPolygon)
                {
                    shpDest = CreateShape(GisShapeType.GisShapeTypePolygon);
                    for (int j = 0; j < (shpOrig.Geometry as Polygon).ExteriorRing.Coordinates.Length; j++)
                    {
                        var p = new GisPoint();
                        p.X = (shpOrig.Geometry as Polygon).ExteriorRing.Coordinates[j].X;
                        p.Y = (shpOrig.Geometry as Polygon).ExteriorRing.Coordinates[j].Y;
                        (shpDest as GisShapePolygon).Points.Add(p);
                    }
                }

                shpDest.UID = shpOrig.UID;
                shpDest.IsSelected = shpOrig.IsSelected;

                var keys = shpOrig.Keys;
                shpDest.PopulateTypes(keys);

                foreach (string key in keys)
                {
                    shpDest[key] = shpOrig[key];
                }

                if (shpDest != null)
                    res.Add(shpDest);
            }

            if (provider.Extent != null)
                extent = new Extent(provider.Extent.MinX, provider.Extent.MinY, provider.Extent.MaxX, provider.Extent.MaxY);
            else
                extent = new Extent();

            return res;
        }


        private static GisShapeBase CreateShape(GisShapeType shapeType)
        {
            switch (shapeType)
            {
                case GisShapeType.GisShapeTypePoint:
                    return new GisShapePoint(null);
                case GisShapeType.GisShapeTypeMultiPoint:
                    return new GisShapeMultiPoint(null);
                case GisShapeType.GisShapeTypeArc:
                    return new GisShapeArc(null);
                case GisShapeType.GisShapeTypePolygon:
                    return new GisShapePolygon(null);
                default:
                    throw new Exception("Default shape type not implemented");
            }
        }
    }
}
