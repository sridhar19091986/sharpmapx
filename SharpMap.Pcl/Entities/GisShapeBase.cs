//==============================================================================
// Copyright 2010-2014 - Fabrizio Vita (www.itacasoft.com)
// This file can be redistributed and/or modified under the terms of the GNU Lesser General Public License.
//===============================================================================

using System.Collections.Generic;
using System.ComponentModel;
using System;
using System.Globalization;
using System.Collections.ObjectModel;
using SharpMap.Layers;

namespace SharpMap.Entities
{
    /// <summary>
    /// This class encapsulate a GIS shape.
    /// </summary>
    public abstract class GisShapeBase : INotifyPropertyChanged
    {
        /// <summary>
        /// Creates a new instance of the <see cref="GisShapeBase"/>
        /// </summary>
        public GisShapeBase(LayerVector layer)
        {
            //PropertyChanged = (s, e) => { };
            Layer = layer;
        }

        /// <summary>
        /// The minimum extent for this <see cref="GisShapeBase"/>, returned as a <see cref="Extent"/>.
        /// </summary>
        /// <returns></returns>
        public abstract Extent GetExtent();


        private LayerVector _layer;
        private bool _isSelected = false;
        private bool _isEdited = false;
        private Int64 _uid;

        /// <summary>
        /// Layer that contains the shape.
        /// </summary>
        public LayerVector Layer
        {
            get { return _layer; }
            set
            {
                if (value != _layer)
                {
                    _layer = value;
                    if (_layer != null) PopulateTypes(_layer);
                    NotifyPropertyChanged("Layer");
                }
            }
        }

        private object _graphic;

        /// <summary>
        /// Element of GUI that represents the entity. 
        /// </summary>
        public object Graphic
        {
            get
            {
                return _graphic;
            }
            set
            {
                _graphic = value;
            }
        }

        /// <summary>
        /// This property is a boolean value indicating if the entity is selected.
        /// </summary>
        public bool IsSelected
        {
            get
            {
                return _isSelected;
            }
            set
            {
                if (_isSelected != value)
                {
                    _isSelected = value;
                    NotifyPropertyChanged("IsSelected");
                }
            }
        }

        /// <summary>
        /// This property is a boolean value indicating if the entity is being edited.
        /// </summary>
        public bool IsEdited
        {
            get
            {
                return _isEdited;
            }
            set
            {
                if (_isEdited != value)
                {
                    _isEdited = value;
                    NotifyPropertyChanged("IsEdited");
                }
            }
        }

        /// <summary>
        /// This property is a boolean value indicating if the entity is new.
        /// </summary>
        public bool IsNew
        {
            get
            {
                return _uid == 0;
            }
        }

        /// <summary>
        /// Identifier of entity.
        /// </summary>
        public Int64 UID
        {
            get
            {
                return _uid;
            }
            set
            {
                _uid = value;
            }
        }

        private Dictionary<string, GisFieldType> _types = null;

        public Dictionary<string, GisFieldType> Types
        {
            get
            {
                return _types;
            }
        }

        /// <summary>
        /// Populates the types cache
        /// </summary>
        private void PopulateTypes(LayerVector ll)
        {
            _types = new Dictionary<string, GisFieldType>();

            for (int i = 0; i < ll.Fields.Count; i++)
            {
                _types.Add(ll.Fields[i].Name, ll.Fields[i].FieldType);
            }
        }

        /// <summary>
        /// Populates the types cache
        /// </summary>
        public void PopulateTypes(ICollection<string> fields)
        {
            _types = new Dictionary<string, GisFieldType>();

            foreach(var field in fields)
            {
                _types.Add(field, GisFieldType.GisFieldTypeString);
            }
        }

        private Dictionary<string, object> _data = new Dictionary<string, object>();

        /// <summary>
        /// Gets or sets the field value
        /// </summary>
        /// <param name="key">Field name</param>
        /// <returns></returns>
        public object this[string key]
        {
            get
            {
                if (!_data.ContainsKey(key))
                    _data[key] = null;

                return _data[key];
            }

            set
            {
#if DEBUG
                //type verification
                Type t = value.GetType();
#endif

                GisFieldType fieldType = _types[key];

                try
                {
                    switch (fieldType)
                    {
                        case GisFieldType.GisFieldTypeString:
                            if ((!_data.ContainsKey(key)) || (_data[key] == null) || (_data[key].ToString() != value.ToString()))
                            {
                                _data[key] = value.ToString();
                                NotifyPropertyChanged("[" + key + "]");
                            }
                            break;
                        case GisFieldType.GisFieldTypeNumber:
                            if ((!_data.ContainsKey(key)) || (_data[key] == null) || (Convert.ToInt64(_data[key]) != Convert.ToInt64(value)))
                            {
                                if ((value == null) || (value == ""))
                                    _data[key] = null;
                                else
                                    _data[key] = Convert.ToInt64(Decimal.Parse(value.ToString(), System.Globalization.NumberStyles.Float, CultureInfo.InvariantCulture));
                                NotifyPropertyChanged("[" + key + "]");
                            }
                            break;
                        case GisFieldType.GisFieldTypeFloat:
                            if ((!_data.ContainsKey(key)) || (_data[key] == null) || (Convert.ToDouble(_data[key], CultureInfo.InvariantCulture) != Convert.ToDouble(value, CultureInfo.InvariantCulture)))
                            {
                                if ((value == null) || (value == ""))
                                    _data[key] = null;
                                else
                                    _data[key] = Convert.ToDouble(value, CultureInfo.InvariantCulture);
                                NotifyPropertyChanged("[" + key + "]");
                            }
                            break;
                        case GisFieldType.GisFieldTypeBoolean:
                            if ((!_data.ContainsKey(key)) || (_data[key] == null) || (Convert.ToBoolean(_data[key]) != Convert.ToBoolean(value)))
                            {
                                if ((value == null) || (value == ""))
                                    _data[key] = null;
                                else
                                    _data[key] = Convert.ToBoolean(value);
                                NotifyPropertyChanged("[" + key + "]");
                            }
                            break;
                        case GisFieldType.GisFieldTypeDate:
                            if ((!_data.ContainsKey(key)) || (_data[key] == null) || (Convert.ToDateTime(_data[key]) != Convert.ToDateTime(value)))
                            {
                                if ((value == null) || (value == ""))
                                    _data[key] = null;
                                else
                                    _data[key] = Convert.ToDateTime(value);
                                NotifyPropertyChanged("[" + key + "]");
                            }
                            break;
                        default:
                            _data[key] = null;
                            NotifyPropertyChanged("[" + key + "]");
                            break;
                    }
                }
                catch (Exception ex)
                {
                    throw new FormatException("Input string is not in a correct format: " + ex.Message);
                }
            }
        }

        /// <summary>
        /// Triggers the property changed event.
        /// </summary>
        /// <param name="propertyName">The name of the changed property.</param>
        protected void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                var args = new PropertyChangedEventArgs(propertyName);
                PropertyChanged(this, args);
            }
        }

        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;
    }
}

