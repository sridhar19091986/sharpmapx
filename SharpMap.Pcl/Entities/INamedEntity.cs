//==============================================================================
// Copyright 2010-2014 - Fabrizio Vita (www.itacasoft.com)
// This file can be redistributed and/or modified under the terms of the GNU Lesser General Public License.
//===============================================================================

namespace SharpMap.Entities
{
    /// <summary>
    /// Base interface for entities.
    /// </summary>
    public interface INamedEntity
    {
        /// <summary>
        /// Name.
        /// </summary>
        string Name
        {
            get; set;
        }
     }
}
