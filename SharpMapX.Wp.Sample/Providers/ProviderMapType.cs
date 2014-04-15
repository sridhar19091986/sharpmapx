//==============================================================================
// Author: Fabrizio Vita            
// Date: 2014-03-20
// Copyright: (c)2010-2011 ItacaSoft di Vita Fabrizio. ALL RIGHTS RESERVED.
//===============================================================================
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpMap.Mobile.Wp.Sample.Providers
{
    public enum ProviderMapType
    {
        /// <summary>
        /// Undefined
        /// </summary>
        Undefined,
        /// <summary>
        /// Road map
        /// </summary>
        BingMapsRoad,
        /// <summary>
        /// Satellite map
        /// </summary>
        BingMapsSatellite,
        /// <summary>
        /// Satellite map with labels
        /// </summary>
        BingMapsHibrid,
        /// <summary>
        /// Google maps road
        /// </summary>
        GoogleRoad,
        /// <summary>
        /// Google maps satellite
        /// </summary>
        GoogleSatellite,
        /// <summary>
        /// Google maps hybrid
        /// </summary>
        GoogleHybrid,
        /// <summary>
        /// Google maps phisical
        /// </summary>
        GooglePhisical,
        /// <summary>
        /// Custom provider
        /// </summary>
        Custom
    }
}
