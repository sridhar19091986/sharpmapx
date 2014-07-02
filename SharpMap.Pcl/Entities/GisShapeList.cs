//==============================================================================
// Copyright 2010-2014 - Fabrizio Vita (www.itacasoft.com)
// This file can be redistributed and/or modified under the terms of the GNU Lesser General Public License.
//===============================================================================

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace SharpMap.Entities
{
    /// <summary>
    /// This class manages a list of shapes.
    /// </summary>
    public class GisShapeList : ObservableCollection<GisShapeBase>
    {
        /// <summary>
        /// Returns an shape given its ID.
        /// </summary>
        /// <param name="uid">ID of the GIS shape</param>
        /// <returns>GIS shape</returns>
        public GisShapeBase ShapeByUid(long uid)
        {
            for (int i = 0; i < this.Count; i++)
            {
                GisShapeBase fe = this[i];
                if ((fe != null) && (fe.UID == uid))
                {
                    return this[i];
                }
            }

            return null;
        }


        /// <summary>
        /// Creates a new instance of the <see cref="GisShapeList"/>
        /// </summary>
        public GisShapeList()
        {
            //Perhaps this constructor should get a dictionary parameter
            //to specify the name and type of the columns
        }

        /// <summary>
        /// This property gets a boolean value indicating if the list is read only.
        /// </summary>
        public bool IsReadOnly
        {
            get
            {
                return false;
            }
        }

        string _name;

        /// <summary>
        /// This property gets a boolean value indicating the name of the list
        /// </summary>
        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                _name = value;
            }
        }

        /// <summary>
        /// Removes a shape given its ID.
        /// </summary>
        /// <param name="uid">ID of the shape.</param>
        public void RemoveByUid(int uid)
        {
            for (int i = 0; i < Count; i++)
            {
                GisShapeBase fe = this[i];
                if ((fe != null) && (fe.UID == uid))
                {
                    this.RemoveAt(i);
                }
            }
        }
    }
}
