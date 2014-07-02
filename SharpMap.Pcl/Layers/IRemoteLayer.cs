//==============================================================================
// Copyright 2010-2014 - Fabrizio Vita (www.itacasoft.com)
// This file can be redistributed and/or modified under the terms of the GNU Lesser General Public License.
//===============================================================================

using Portable.Http;

namespace SharpMap.Layers
{
    public interface IRemoteLayer
    {
        /// <summary>
        /// Pass the web client to use for remote connections
        /// </summary>
        void InitializeConnection(IWebClient webClient);
    }
}
