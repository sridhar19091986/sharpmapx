//==============================================================================
// Author: Fabrizio Vita            
// Date: 2014-03-20
// Copyright: (c)2010-2011 ItacaSoft di Vita Fabrizio. ALL RIGHTS RESERVED.
//===============================================================================
using System.Net;
using NUnit.Framework;
using Portable.Http;

namespace SharpMapX.Tests
{
    [TestFixture]
    public class HttpExtensionsTest
    {
        [Test]
        public async void HttpWebRequestDownloadTest()
        {
            var request = (HttpWebRequest)HttpWebRequest.Create("http://www.google.it");
            request.Method = "GET";
            var p = await request.GetResponseStreamAsync(null);
            Assert.IsTrue(p.StatusCode == HttpStatusCode.OK);
        }
    }
}
