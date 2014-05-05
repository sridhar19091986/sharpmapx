using System;
using System.IO;
using GeoAPI.Geometries;
using NetTopologySuite.IO;
using NUnit.Framework;
using NetTopologySuite.IO.GeoTools;
using Portable.IO;

namespace NetTopologySuite.Tests
{
    [TestFixture]
    public class NetTopologySuiteShapeFileTests
    {
        [SetUp]
        public void SetupFixture()
        {
            IoManager.Initialize(new FileNet());            
        }

        [Test]
        public void OpenShapefile_Test()
        {
            TestShapeReadWrite(@"SampleData\tnp_arc.shp", "arc.shp");
            TestShapeReadWrite(@"SampleData\Zone_ISTAT.shp", "Test_Zone_ISTAT.shp");
            TestShapeReadWrite(@"SampleData\Strade.shp", "Test_Strade.shp");
        }

        private static IGeometryCollection ReadShape(string shapepath)
        {
            if (!File.Exists(shapepath))
                throw new ArgumentException("File " + shapepath + " not found!");

            var reader = new ShapefileReader(shapepath);
            var geometries = reader.ReadAll();
            return geometries;
        }

        private static void WriteShape(IGeometryCollection geometries, string shapepath)
        {
            if (File.Exists(shapepath))
                File.Delete(shapepath);
            var sfw = new ShapefileWriter(geometries.Factory);
            sfw.Write(Path.GetFileNameWithoutExtension(shapepath), geometries);
        }


        private static void TestShapeReadWrite(string shapepath, string outputpath)
        {
            var collection = ReadShape(shapepath);
            WriteShape(collection, outputpath);
            var testcollection = ReadShape(outputpath);

            if (!collection.EqualsExact(testcollection))
                throw new ArgumentException("Geometries are not equals");
            Console.WriteLine("TEST OK!");
        }        

    }
}
