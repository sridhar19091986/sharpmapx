//==============================================================================
// Copyright 2010-2014 - Fabrizio Vita (www.itacasoft.com)
// This file can be redistributed and/or modified under the terms of the GNU Lesser General Public License.
//===============================================================================

namespace SharpMap.Entities
{
    /// <summary>
    /// Exception class thrown by the TkMap Module.
    /// </summary>
    public class ServiceExceptionReport
    {
        /// <summary>
        /// Message of the exception.
        /// </summary>
        public string ServiceException
        {
            get; set;
        }

        /// <summary>
        /// Stacktrace of the exception.
        /// </summary>
        public string StackTrace
        {
            get;
            set;
        }

        /// <summary>
        /// Internal exception.
        /// </summary>
        public ServiceExceptionReport InnerException
        {
            get;
            set;
        }
    }
}
