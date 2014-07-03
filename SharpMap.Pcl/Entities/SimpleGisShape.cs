//==============================================================================
// Copyright 2010-2014 - Fabrizio Vita (www.itacasoft.com)
// This file can be redistributed and/or modified under the terms of the GNU Lesser General Public License.
//===============================================================================

using System.Collections.Generic;
using GeoAPI.Geometries;

namespace SharpMap.Entities
{
    public class SimpleGisShape
    {
        public SimpleGisShape()
        {
            
        }

        public SimpleGisShape(IGeometry geometry)
        {
            this.Geometry = geometry;
        }

        public IGeometry Geometry { get; set; }

        private Dictionary<string, string> _data = new Dictionary<string, string>();

        /// <summary>
        /// Gets or sets the field value
        /// </summary>
        /// <param name="key">Field name</param>
        /// <returns></returns>
        public string this[string key]
        {
            get
            {
                if (!_data.ContainsKey(key))
                    _data[key] = null;

                return _data[key];
            }

            set
            {
                _data[key] = value;
            }
        }

        public ICollection<string> Keys
        {
            get
            {
                return _data.Keys;
            }
        }

        public bool IsSelected { get; set; }
        public int UID { get; set; }
    }
}
