// This file can be redistributed and/or modified under the terms of the GNU Lesser General Public License.
// SOURCECODE IS MODIFIED FROM SharpMap-SL-80096 BY ITACASOFT DI VITA FABRIZIO
// 22/02/2011: excluded what does not compile under Windows Phone

using System;
using ProjNet.CoordinateSystems.Transformations;
using SharpMap.Data;
using SharpMap.Data.Providers;
using SharpMap.Styles;
using SharpMap.Layers;

namespace SharpMap.Rendering
{
    public interface IRenderer
    {
        void Render(IView view, Map map);
    }
}
