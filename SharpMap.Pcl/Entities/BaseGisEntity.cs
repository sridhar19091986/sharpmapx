//==============================================================================
// Copyright 2010-2014 - Fabrizio Vita (www.itacasoft.com)
// This file can be redistributed and/or modified under the terms of the GNU Lesser General Public License.
//===============================================================================

namespace SharpMap.Entities
{
    /// <summary>
    /// Base class for GIS entities.
    /// </summary>
    public abstract class BaseGisEntity
    {
        /// <summary>
        /// Name of the entity.
        /// </summary>
        protected string name;
        /// <summary>
        /// Name.
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
}
