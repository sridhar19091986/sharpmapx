//==============================================================================
// Copyright 2010-2014 - Fabrizio Vita (www.itacasoft.com)
// This file can be redistributed and/or modified under the terms of the GNU Lesser General Public License.
//===============================================================================

using SharpMap.Entities;
using System;

namespace SharpMap.Entities
{
    /// <summary>
    /// This class manages a field of a layer.
    /// </summary>
    public class LayerField : INamedEntity
    {
        /// <summary>
        /// Creates a new instance of the <see cref="LayerField"/>
        /// </summary>
        public LayerField()
            : base()
        {
            Hidden = false;
            IsUID = false;
            ReadOnly = true;
            Clear();
        }

        /// <summary>
        /// Type of field.
        /// </summary>
        public GisFieldType FieldType { get; set; }
        /// <summary>
        /// Dimension/width of the field.
        /// </summary>
        /// <remarks>Has a meaning for strings and numbers.</remarks>
        public Int32 Width { get; set; }
        /// <summary>
        /// Indicates if the field is read only.
        /// </summary>
        public bool ReadOnly { get; set; }
        /// <summary>
        /// Indicates if the field is the UID (identifier) of the entity.
        /// </summary>
        public bool IsUID { get; set; }
        /// <summary>
        /// Indicates if the field is hidden.
        /// </summary>
        public bool Hidden { get; set; }

        /// <summary>
        /// Returns the type of the filed.
        /// </summary>
        /// <returns>Type.</returns>
        public Type GetFieldType()
        {
            switch (FieldType)
            {
                case GisFieldType.GisFieldTypeString:
                    return typeof(System.String);
                case GisFieldType.GisFieldTypeNumber:
                    return typeof(System.Int64);
                case GisFieldType.GisFieldTypeFloat:
                    return typeof(System.Double);
                case GisFieldType.GisFieldTypeBoolean:
                    return typeof(System.Boolean);
                case GisFieldType.GisFieldTypeDate:
                    return typeof(System.DateTime);
                default:
                    return typeof(System.DBNull);
            }

        }

        /// <summary>
        /// Creates a new instance of the <see cref="LayerField"/>
        /// </summary>
        /// <param name="sName">Field name.</param>
        public LayerField(string sName)
            : this()
        {
            LoadValues(sName);
        }

        /// <summary>
        /// Clears the name of the field.
        /// </summary>
        private void Clear()
        {
            name = string.Empty;
        }

        private void LoadValues(string sName)
        {
            Clear();
            name = sName;
        }

        /// <summary>
        /// Name of the field.
        /// </summary>
        protected string name;
        /// <summary>
        /// Name of the field.
        /// </summary>
        public string Name
        {
            get
            {
                return name;
            }
            set
            {
                name = value;
            }
        }

    }

    /// <summary>
    /// This class manages a list of fields of a GIS layer.
    /// </summary>
    public class LayerFields : System.Collections.Generic.List<LayerField>
    {
        /// <summary>
        /// Returns a field given its name.
        /// </summary>
        /// <param name="lyrFieldName">Name of the field.</param>
        /// <returns>LayerField object.</returns>
        public LayerField GetLayerFieldByName(string lyrFieldName)
        {
            foreach (LayerField obj in this)
            {
                if (obj.Name == lyrFieldName) return obj;
            }
            return null;
        }

        /// <summary>
        /// Returns a field given its name.
        /// </summary>
        /// <param name="lyrFieldName">Name of the field.</param>
        /// <returns>Index of the LayerField object.</returns>
        public int GetLayerFieldIndexByName(string lyrFieldName)
        {
            for (int i = 0; i < Count; i++)
            {
                if (this[i].Name == lyrFieldName) return i;
            }
            return -1;
        }
    }
}
