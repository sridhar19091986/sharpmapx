//==============================================================================
// Copyright 2010-2014 - Fabrizio Vita (www.itacasoft.com)
// This file can be redistributed and/or modified under the terms of the GNU Lesser General Public License.
//===============================================================================

using System;

namespace SharpMap.Entities
{
    /// <summary>
    /// EventArgs class for strings.
    /// </summary>
    public class StringEventArgs : EventArgs
    {
        /// <summary>
        /// Creates a new instance of the <see cref="StringEventArgs"/>
        /// </summary>
        /// <param name="value">Value.</param>
        public StringEventArgs(string value)
            : base()
        {
            _value = value;
        }

        string _value;

        /// <summary>
        /// Value.
        /// </summary>
        public string Value
        {
            get
            {
                return _value;
            }
        }
    }

}
