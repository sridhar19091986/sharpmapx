//==============================================================================
// Copyright 2010-2014 - Fabrizio Vita (www.itacasoft.com)
// This file can be redistributed and/or modified under the terms of the GNU Lesser General Public License.
//===============================================================================

using System;
using System.Collections.Generic;

namespace SharpMap.Entities
{
/*
    /// <summary>
    /// This class represents a list of alphanumerical field for a GIS entity (shape)
    /// </summary>
    public class ShapeFields
    {
        internal ShapeFields(GIS_Shape parent)
        {
            _parent = parent;
        }

        GIS_Shape _parent;
        List<string> _values;

        /// <summary>
        /// Gets or sets the value of a field given its index.
        /// </summary>
        /// <param name="index">Index of the field.</param>
        /// <returns>Value.</returns>
        public string this[int index]
        {
            get
            {
                return _values[index];
            }
            set
            {
                _values[index] = value;
            }
        }

        /// <summary>
        /// Gets or sets the value of a field given its name.
        /// </summary>
        /// <param name="key">Name of the field.</param>
        /// <returns>Value.</returns>
        public string this[string key]
        {
            get
            {
                int index = _parent.Layer.Fields.GetLayerFieldIndexByName(key);
                if (index >= 0)
                {
                    return _values[index];
                }
                else
                    return "";
            }
            set
            {
                int index = _parent.Layer.Fields.GetLayerFieldIndexByName(key);
                if (index >= 0)
                {
                    _values[index] = value;
                }
                else
                    throw new Exception("key " + key + " does not exists");
            }
        }

        /// <summary>
        /// List of values.
        /// </summary>
        public List<string> Values
        {
            get
            {
                return _values;
            }
            set
            {
                _values = value;
            }
        }

        /// <summary>
        /// List of field names.
        /// </summary>
        public List<string> Keys
        {
            get
            {
                List<string> result = new List<string>();

                for (int i = 0; i < _parent.Layer.Fields.Count; i++)
                {
                    result.Add(_parent.Layer.Fields[i].Name);
                }
                return result;
            }
        }
    }
    */
}
