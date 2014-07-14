//==============================================================================
// Copyright 2010-2014 - Fabrizio Vita (www.itacasoft.com)
// This file can be redistributed and/or modified under the terms of the GNU Lesser General Public License.
//===============================================================================

using SharpMap.Entities;
using System.IO;
using System.Xml;
using System.Xml.Linq;

namespace SharpMap.WMS
{
    public static class WmsUtils
    {
        /// <summary>
        /// Evaluates the received xml to see if the server has thrown an exception.
        /// </summary>
        /// <param name="xml">XML received from server.</param>
        /// <returns>True is an exception has been thrown.</returns>
        public static ServiceExceptionReport CheckException(string xml)
        {
            if (TypeUtils.CheckObjectType(xml, "ServiceExceptionReport"))
            {
                ServiceExceptionReport result = new ServiceExceptionReport();

                XDocument doc;
                var settings = new XmlReaderSettings();
                settings.DtdProcessing = DtdProcessing.Ignore;

                using (XmlReader reader = XmlReader.Create(new StringReader(xml), settings))
                {
                    doc = XDocument.Load(reader);
                }

                var docRoot = doc.Element("ServiceExceptionReport");
                if (docRoot == null) return null;

                var se = docRoot.Element("ServiceException");
                if (se == null) return null;
                result.Code = se.Attribute("code") != null ? se.Attribute("code").Value : "";

                result.ServiceException = se.Value.Trim();

                return result;
            }
            return null;
        }

    }
}
