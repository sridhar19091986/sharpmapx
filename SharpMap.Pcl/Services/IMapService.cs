//==============================================================================
// Copyright 2010-2014 - Fabrizio Vita (www.itacasoft.com)
// This file can be redistributed and/or modified under the terms of the GNU Lesser General Public License.
//===============================================================================

namespace SharpMap.Services
{
    public interface IMapService: IBaseService
    {
        string MERCATOR_EPSG { get; set; }
    }
}
