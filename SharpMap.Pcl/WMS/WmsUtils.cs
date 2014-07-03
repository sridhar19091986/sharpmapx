using SharpMap.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SharpMap.WMS
{
    public static class WmsUtils
    {
        /// <summary>
        /// Evaluates the received xml to see if the server has thrown an exception.
        /// </summary>
        /// <param name="xml">XML received from server.</param>
        /// <returns>True is an exception has been thrown.</returns>
        public static bool CheckException(string xml)
        {
            if (TypeUtils.CheckObjectType(xml, typeof(ServiceExceptionReport)))
            {
                ServiceExceptionReport ex = TypeUtils.DeSerializeObject<ServiceExceptionReport>(xml);
                return true;
            }
            return false;
        }

    }
}
