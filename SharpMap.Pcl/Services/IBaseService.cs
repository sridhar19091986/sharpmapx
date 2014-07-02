//==============================================================================
// Copyright 2010-2014 - Fabrizio Vita (www.itacasoft.com)
// This file can be redistributed and/or modified under the terms of the GNU Lesser General Public License.
//===============================================================================

using System.ComponentModel;

namespace SharpMap.Services
{
    /// <summary>
    /// Interface for al classes that manages a service for SMS.
    /// </summary>
    public interface IBaseService
    {
        /// <summary>
        /// This property is true when the service is busy in an asyncronous operation.
        /// </summary>
        bool BusyState
        {
            get; set;
        }

        string ServiceUrl { get; set; }

        #region INotifyPropertyChanged Members
        /// <summary>
        /// This event is thrown when a property is changed.
        /// </summary>
        event PropertyChangedEventHandler PropertyChanged;
        #endregion
    }
}
