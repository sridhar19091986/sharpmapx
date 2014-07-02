//==============================================================================
// Copyright 2010-2014 - Fabrizio Vita (www.itacasoft.com)
// This file can be redistributed and/or modified under the terms of the GNU Lesser General Public License.
//===============================================================================

using System;
using SharpMap.Entities;
using SharpMap.WMS;

namespace SharpMap
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

    /// <summary>
    /// EventArgs class for a list of geometrical entities.
    /// </summary>
    public class FeaturesEventArgs: EventArgs
    {
        /// <summary>
        /// Creates a new instance of the <see cref="SelectionsEventArgs"/>
        /// </summary>
        /// <param name="features">List of entities.</param>
        public FeaturesEventArgs(GisShapeList features)
        {
            _features = features;
        }

        GisShapeList _features;

        /// <summary>
        /// List of entities.
        /// </summary>
        public GisShapeList Features
        {
            get
            {
                return _features;
            }
            set
            {
                _features = value;
            }
        }
    }

    /// <summary>
    /// EventArgs class for a list of geometrical entities to be saved or deleted.
    /// </summary>
    public class SaveOperationEventArgs : FeaturesEventArgs
    {
        /// <summary>
        /// Creates a new instance of the <see cref="SelectionsEventArgs"/>
        /// </summary>
        /// <param name="features">List of entities.</param>
        public SaveOperationEventArgs(GisShapeList features)
            : base(features)
        {
        }

        bool _abort = false;

        /// <summary>
        /// This boolean property indicates if the operation must be aborted.
        /// </summary>
        public bool Abort
        {
            get
            {
                return _abort;
            }

            set
            {
                _abort = value;
            }
        }
    }

    /// <summary>
    /// EventArgs class for a Wms project.
    /// </summary>
    public class WmsProjectEventArgs : EventArgs
    {
        /// <summary>
        /// Creates a new instance of the <see cref="WmsProjectEventArgs"/>
        /// </summary>
        /// <param name="value">Value.</param>
        public WmsProjectEventArgs(WmsProjectInfo value)
            : base()
        {
            _value = value;
        }

        WmsProjectInfo _value;

        /// <summary>
        /// Value.
        /// </summary>
        public WmsProjectInfo Value
        {
            get
            {
                return _value;
            }
        }
    }



    /// <summary>
    /// EventArgs class for Int32
    /// </summary>
    public class Int32EventArgs : EventArgs
    {
        /// <summary>
        /// Creates a new instance of the <see cref="Int32EventArgs"/>
        /// </summary>
        /// <param name="value">Value</param>
        public Int32EventArgs(Int32 value)
            : base()
        {
            _value = value;
        }

        Int32 _value;

        /// <summary>
        /// The value
        /// </summary>
        public Int32 Value
        {
            get
            {
                return _value;
            }
        }
    }

    /// <summary>
    /// EventArgs class for a boolean representing and handled task
    /// </summary>
    public class HandledEventArgs : EventArgs
    {
        /// <summary>
        /// Creates a new instance of the <see cref="HandledEventArgs"/>
        /// </summary>
        /// <param name="handled">Handled value</param>
        public HandledEventArgs(bool handled)
            : base()
        {
            _handled = handled;
        }

        bool _handled;

        /// <summary>
        /// The value
        /// </summary>
        public bool Handled
        {
            get
            {
                return _handled;
            }

            set
            {
                _handled = value;
            }
        }
    }

}
