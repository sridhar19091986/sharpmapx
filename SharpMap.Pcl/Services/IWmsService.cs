//==============================================================================
// Copyright 2010-2014 - Fabrizio Vita (www.itacasoft.com)
// This file can be redistributed and/or modified under the terms of the GNU Lesser General Public License.
//===============================================================================

using System;
using System.Collections.Generic;
using SharpMap.Entities;
using SharpMap.Services;
using Portable.Http;
using SharpMap.Styles;

namespace SharpMap.WMS
{
    public interface IWMSService : IMapService
    {
        /// <summary>
        /// Event thrown when a feature info request is completed.
        /// </summary>
        event EventHandler<DownloadStringCompletedEventArgs> OnGetFeatureInfoCompleted;

        /// <summary>
        /// Event thrown when a getcapabilities request is completed.
        /// </summary>
        event EventHandler<WmsProjectEventArgs> OnGetCapabilitiesCompleted;

        string GetMapRequest(Extent locationRect, Size actualSize, IEnumerable<string> layers , bool useCache = true);

        void GetCapabilities();

        void GetCapabilities(string wmsUrl);

        void GetFeatureInfo(int x, int y, Size actualSize, Extent locationRect);

        string IMAGE_FORMAT { get; set; }
        /// <summary>
        /// Comma separated list of layers to view
        /// </summary>
        string Layers { get; set; }
        /// <summary>
        /// Comma separated list of layers to query
        /// </summary>
        string QueryLayers { get; set; }
        /// <summary>
        /// Indicates if to force getfeatureinfo request on WGS84 even if GetMap is on Mercatore
        /// </summary>
        bool ForceWgs84ForFeatureInfo { get; set; }

        /// <summary>
        /// Set the webclient to use
        /// </summary>
        /// <param name="webClient"></param>
        void InitializeConnection(IWebClient webClient);
    }
}
