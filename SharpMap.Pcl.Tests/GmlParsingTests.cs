using System.IO;
using System.Reflection;
using NUnit.Framework;

namespace SharpMapX.Tests
{
    [TestFixture]
    public class GmlParsingTests
    {
        [Test]
        public void ParseSelectByRectangleMmsTest()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = "SharpMap.Pcl.Tests.SampleData.selectbyrectangle_mms_sample.xml";
            string gml;

            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            using (var reader = new StreamReader(stream))
            {
                gml = reader.ReadToEnd();
            }

            var provider = new SharpMap.GMLUtils.GmlProvider(gml);
            var geometries = provider.Features;
            Assert.AreEqual("Streets", geometries.Name);
            Assert.AreEqual(9, geometries.Count);
        }

        [Test]
        public void ParseGetFeaturesTest()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = "SharpMap.Pcl.Tests.SampleData.getfeatures_sample.xml";
            string gml;

            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            using (var reader = new StreamReader(stream))
            {
                gml = reader.ReadToEnd();
            }

            var provider = new SharpMap.GMLUtils.GmlProvider(gml);
            var geometries = provider.Features;
            Assert.AreEqual("streets", geometries.Name);
            Assert.AreEqual(87, geometries.Count);
        }

    }
}
