//==============================================================================
// Copyright 2010-2014 - Fabrizio Vita (www.itacasoft.com)
// This file can be redistributed and/or modified under the terms of the GNU Lesser General Public License.
//===============================================================================

using System;

namespace SharpMap.Services
{
    public class SessionManager : ISessionManager
    {
        public SessionManager() 
        {
            _userID = new Guid("00000000-0000-0000-0000-000000000001");
            _sessionID = Guid.Empty;
        }

        private Guid _sessionID;

        /// <summary>
        /// Identifier of the session
        /// </summary>
        public Guid SessionID
        {
            get
            {
                if (_sessionID == Guid.Empty)
                    _sessionID = Guid.NewGuid();
                return _sessionID;
            }
        }

        private Guid _userID = Guid.Empty;

        /// <summary>
        /// User identifier.
        /// </summary>
        /// <remarks>This value should be set on login.</remarks>
        public Guid UserID
        {
            get
            {
                return _userID;
            }
            set
            {
                _userID = value;
            }
        }
    }
}
