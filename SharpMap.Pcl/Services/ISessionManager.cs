//==============================================================================
// Copyright 2010-2014 - Fabrizio Vita (www.itacasoft.com)
// This file can be redistributed and/or modified under the terms of the GNU Lesser General Public License.
//===============================================================================

using System;

namespace SharpMap.Services
{
    public interface ISessionManager
    {
        Guid SessionID { get; }
        Guid UserID { get; set; }
    }
}
