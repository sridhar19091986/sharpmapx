//==============================================================================
// Copyright 2010-2014 - Fabrizio Vita (www.itacasoft.com)
// This file can be redistributed and/or modified under the terms of the GNU Lesser General Public License.
//===============================================================================

using System;

namespace SharpMap.Services
{
    public abstract class MapService: BaseService, IMapService
    {
        public MapService(ISessionManager sessionManager, string serviceUrl)
            : base(sessionManager, serviceUrl)
        {
        }

        public MapService(string serviceUrl)
            : base(serviceUrl)
        {
        }

        public virtual string MERCATOR_EPSG
        {
            get
            {
                return "3857";
            }

            set
            {
                throw new InvalidOperationException();
            }
        }
    }
}
