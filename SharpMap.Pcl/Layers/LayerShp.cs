//==============================================================================
// Copyright 2010-2014 - Fabrizio Vita (www.itacasoft.com)
// This file can be redistributed and/or modified under the terms of the GNU Lesser General Public License.
//===============================================================================

#define USE_SFDR
using System;
using System.Collections;
using System.Collections.Generic;
using GeoAPI.Geometries;
using NetTopologySuite.IO;
using NetTopologySuite.IO.GeoTools;
using SharpMap.Entities;
using NetTopologySuite.Geometries;

namespace SharpMap.Layers
{
    public class LayerShp: LayerVector
    {
#if USE_SFDR
        private ShapefileDataReader reader;
#else
        private IEnumerator shpProvider;
        private IEnumerator dbfProvider;
#endif
        Envelope filterEnvelope = null;
        int uid = 0;

        public override Extent BoundingBox { get; set; }

        public override void Open()
        {
            if (IsOpened)
                throw new Exception(string.Format("Cannot open before to close the resource {0}", Path));
            
            if (string.IsNullOrEmpty(Path))
                throw new Exception("Cannot open a null or empty path");

#if USE_SFDR
            reader = Shapefile.CreateDataReader(Path, new GeometryFactory());
            var s1 = reader.ShapeHeader;
            var h = reader.DbaseHeader;
#else

            var shpReader = new ShapefileReader(Path);
            shpProvider = shpReader.GetEnumerator();

            var dbfReader = new DbaseFileReader(IoManager.File.ChangeExtension(Path, ".dbf"));
            var h = dbfReader.GetHeader();
            dbfProvider = dbfReader.GetEnumerator();
#endif

            foreach (var f in h.Fields)
            {
                var lf = new LayerField();
                lf.Name = f.Name;

                switch (f.DbaseType)
                {
                    case 'L': // logical data type, one character (T,t,F,f,Y,y,N,n)
                        lf.FieldType = GisFieldType.GisFieldTypeBoolean;
                        break;
                    case 'C': // char or string
                        lf.FieldType = GisFieldType.GisFieldTypeString;
                        break;
                    case 'D': // date
                        lf.FieldType = GisFieldType.GisFieldTypeDate;
                        break;
                    case 'N': // numeric
                        if (f.DecimalCount > 0)
                            lf.FieldType = GisFieldType.GisFieldTypeFloat;
                        else
                            lf.FieldType = GisFieldType.GisFieldTypeNumber;
                        break;
                    case 'F': // double
                        lf.FieldType = GisFieldType.GisFieldTypeNumber;
                        break;
                    default:
                        throw new NotSupportedException("Do not know how to parse Field type " + f.DbaseType);
                      
                }

                Fields.Add(lf);
            }
        }

        public override void Close()
        {
#if USE_SFDR
            reader.Close();
            reader.Dispose();
#else
            shpProvider = null;
            dbfProvider = null;
#endif
        }

        public string Path { get; set; }

        private bool InternalRead()
        {
            var result = reader.Read();
            if (result) uid++;
            return result;
        }

        private void InternalReset()
        {
            reader.Reset();
            uid = 0;
        }

        private GisShapeBase InternalGetShape()
        {
            var geom = reader.Geometry;
            if (geom == null)
                return null;

            var data = new object[this.Fields.Count];
            reader.GetValues(data);
            GisShapeBase result = Converter.ToShape(geom, data as IEnumerable<object>, this);
            result.UID = uid;
            return result;
        }

        public override GisShapeBase FindFirst()
        {
            if (!IsOpened)
                throw new Exception("Layer is not open");

            filterEnvelope = null;
#if USE_SFDR
            InternalReset();
            bool found = InternalRead();
#else
            shpProvider.Reset();
            shpProvider.MoveNext();
            var geom = shpProvider.Current as IGeometry;

            dbfProvider.Reset();
            dbfProvider.MoveNext();
            var data = dbfProvider.Current;
#endif
            return found ? InternalGetShape() : null;
        }

        public override GisShapeBase FindFirst(Extent extent)
        {
            if (!IsOpened)
                throw new Exception("Layer is not open");
            filterEnvelope = Converter.ToEnvelope(extent);
            InternalReset();

            return FindNext();
        }

        public override GisShapeBase FindNext()
        {
            bool found = false;
            bool exit = false;

            while (!exit)
            {
                found = InternalRead();
                if (!found)
                    break;

                if (filterEnvelope != null)
                {
                    var geom = reader.Geometry;
                    if (geom.EnvelopeInternal.Intersects(filterEnvelope))
                        break;
                }
                else break;
            }
            if (found)
                return InternalGetShape();

            return null;
        }

        public override bool IsOpened
        {
            get
            {
                if (reader == null) return false;
                return !reader.IsClosed;
            }
        }
    }
}
