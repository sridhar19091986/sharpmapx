//==============================================================================
// Copyright 2010-2014 - Fabrizio Vita (www.itacasoft.com)
// This file can be redistributed and/or modified under the terms of the GNU Lesser General Public License.
//===============================================================================

using System;
using System.Collections.Generic;
using SharpMap.Entities;
using SharpMap.Layers;

namespace SharpMap
{
    /// <summary>
    /// This interface represent a map viewer GUI control
    /// </summary>
    public interface IMapViewer: IDisposable
    {
        List<LayerBase> Layers { get; }

        Extent BoundingBox  { get; }
    }
}
