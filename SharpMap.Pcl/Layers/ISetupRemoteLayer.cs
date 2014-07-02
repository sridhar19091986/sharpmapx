//==============================================================================
// Copyright 2010-2014 - Fabrizio Vita (www.itacasoft.com)
// This file can be redistributed and/or modified under the terms of the GNU Lesser General Public License.
//===============================================================================

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SharpMap.Layers
{
    public interface ISetupRemoteLayer: IRemoteLayer
    {
        /// <summary>
        /// True if the setup is completed
        /// </summary>
        bool IsReady { get; }

        /// <summary>
        /// Initiates the setup procedure using the passed url
        /// </summary>
        void Setup(string url);

        /// <summary>
        /// Event thrown when a setup method is completed.
        /// </summary>
        event EventHandler OnSetupCompleted;
    }
}
