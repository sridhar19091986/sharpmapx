//==============================================================================
// Copyright 2010-2014 - Fabrizio Vita (www.itacasoft.com)
// This file can be redistributed and/or modified under the terms of the GNU Lesser General Public License.
//===============================================================================

using System;

namespace SharpMap.Entities
{
    public class ServiceExceptionReportEventArgs : EventArgs
    {
        /// <summary>
        /// Creates a new instance of the <see cref="StringEventArgs"/>
        /// </summary>
        /// <param name="value">Value.</param>
        public ServiceExceptionReportEventArgs(ServiceExceptionReport value)
            : base()
        {
            _value = value;
        }

        ServiceExceptionReport _value;

        /// <summary>
        /// Value.
        /// </summary>
        public ServiceExceptionReport Value
        {
            get
            {
                return _value;
            }
        }
    }
}
