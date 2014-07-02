//==============================================================================
// Copyright 2010-2014 - Fabrizio Vita (www.itacasoft.com)
// This file can be redistributed and/or modified under the terms of the GNU Lesser General Public License.
//===============================================================================

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using GeoAPI.Geometries;
using NetTopologySuite.Geometries;
using SharpMap.Layers;
using SharpMap.Geometries;

namespace SharpMap.Entities
{
    public static class Converter
    {
        public static Envelope ToEnvelope(Extent extent)
        {
            if (extent == null)
                throw new ArgumentNullException("extent");

            var result = new Envelope(extent.Left, extent.Right, extent.Bottom, extent.Top);
            return result;
        }

        public static Extent ToExtent(Envelope envelope)
        {
            if (envelope == null)
                throw new ArgumentNullException("envelope");

            var result = new Extent(envelope.MinX, envelope.MinY, envelope.MaxX, envelope.MaxY);
            return result;
        }

        public static GisShapeBase ToShape(IGeometry geometry, IEnumerable<object> data, LayerVector layer)
        {
            if (geometry == null)
                throw new Exception("Cannot convert a null geometry");

            GisShapeBase result;

            if (geometry is ScreenPoint)
            {
                var p = geometry as ScreenPoint;
                result = new GisShapePoint(layer);
                ((GisShapePoint)result).Point.X = p.X;
                ((GisShapePoint)result).Point.Y = p.Y;
            }
            else if (geometry is LineString)
            {
                var p = geometry as LineString;
                result = new GisShapeArc(layer);                

                foreach (var c in p.Coordinates)
                {
                    ((GisShapeArc)result).Points.Add(new GisPoint(c.X, c.Y));
                }
            }
            else if (geometry is Polygon)
            {
                var p = geometry as Polygon;
                result = new GisShapePolygon(layer);

                foreach (var c in p.Coordinates)
                {
                    ((GisShapePolygon)result).Points.Add(new GisPoint(c.X, c.Y));
                }

                foreach (var ir0 in p.InteriorRings)
                {
                    var ir1 = new Collection<GisPoint>();
                    ((GisShapePolygon)result).InteriorRings.Add(ir1);

                    foreach (var c in ir0.Coordinates)
                    {
                        ir1.Add(new GisPoint(c.X, c.Y));
                    }
                }
            }
            else if (geometry is MultiPolygon)
            {
                var p = geometry as MultiPolygon;
                result = new GisShapeMultiPolygon(layer);

                foreach (var g in p.Geometries)
                {
                    var child = new GisShapeMultiPolygon.PolygonGeometry();
                    ((GisShapeMultiPolygon)result).Geometries.Add(child);

                    foreach (var c in g.Coordinates)
                    {
                        child.Points.Add(new GisPoint(c.X, c.Y));
                    }

                    foreach (var ir0 in ((Polygon)g).InteriorRings)
                    {
                        var ir1 = new Collection<GisPoint>();
                        child.InteriorRings.Add(ir1);

                        foreach (var c in ir0.Coordinates)
                        {
                            ir1.Add(new GisPoint(c.X, c.Y));
                        }
                    }
                }
            }
            else
            {
                throw new NotImplementedException("Other geometries are not implemented");
            }

            int i = 0;
            foreach(var d in data)
            {
                string key = layer.Fields[i].Name;
                result[key] = d;
                i++;
            }

            return result;
        }
    }
}
