//==============================================================================
// Copyright 2010-2014 - Fabrizio Vita (www.itacasoft.com)
// This file can be redistributed and/or modified under the terms of the GNU Lesser General Public License.
//===============================================================================

using System.ComponentModel;
using System;
using SharpMap.Entities;
using SharpMap.Utilities;

namespace SharpMap.Services
{
    /// <summary>
    /// Base abstract class of all map access services.
    /// </summary>
    public abstract class BaseService
    {
        /// <summary>
        /// This is the constructor.
        /// </summary>
        public BaseService(ISessionManager sessionManager, string serviceUrl)
        {
            _sessionManager = sessionManager;
            _serviceUrl = serviceUrl;
            _serviceUrlContainsQuestionMark = (_serviceUrl != null) && (_serviceUrl.Contains("?"));
        }

        public BaseService(string serviceUrl)
        {
            _sessionManager = new SessionManager();
            _serviceUrl = serviceUrl;
            _serviceUrlContainsQuestionMark = (_serviceUrl != null) && (_serviceUrl.Contains("?"));
        }

        protected ISessionManager _sessionManager;
        protected string _serviceUrl;
        protected bool _serviceUrlContainsQuestionMark;
        private bool _busy = false;

        public string ServiceUrl
        {
            get
            {
                return _serviceUrl;
            }
            set
            {
                _serviceUrl = value;
            }
        }

        /// <summary>
        /// This property gets and set if the service is busy in a asyncronous operation.
        /// </summary>
        public bool BusyState
        {
            get { return _busy; }
            set
            {
                if (!value.Equals(_busy))
                {
                    _busy = value;
                    OnPropertyChanged("BusyState");
                }
            }
        }

        /// <summary>
        /// This property returns a boolean value indicating if the service client is ready to start working.
        /// </summary>
        public virtual bool IsReady
        {
            get
            {
                return (_serviceUrl != "");
            }
        }

        private void OnPropertyChanged(string property)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(property));
            }
        }

        #region INotifyPropertyChanged Members
        /// <summary>
        /// This event is raised when a property is changed.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion

        /// <summary>
        /// Evaluates the received xml to see if the server has thrown an exception.
        /// </summary>
        /// <param name="xml">XML received from server.</param>
        /// <returns>True is an exception has been thrown.</returns>
        public bool CheckException(string xml)
        {
            if (ObjectXMLSerializer.CheckObjectType(xml, typeof(ServiceExceptionReport)))
            {
                ServiceExceptionReport ex = ObjectXMLSerializer<ServiceExceptionReport>.DeSerializeObject(xml);

                WriteDebugLine("Error: " + ex.ServiceException);
                WriteDebugLine("StackTrace: " + ex.StackTrace);
                return true;
            }
            return false;
        }

        #region Events
        /// <summary>
        /// This event is thrown when a line of text has to be sent to the debug output
        /// </summary>
        public event EventHandler<StringEventArgs> OnDebugMessage;
        #endregion

        /// <summary>
        /// Writes a text line to the debug output. 
        /// </summary>
        /// <param name="line">Thext line to be written.</param>
        public void WriteDebugLine(string line)
        {
            if (OnDebugMessage != null)
                OnDebugMessage(this, new StringEventArgs(line));
        }

        public void WriteException(Exception ex)
        {
            if (OnDebugMessage != null)
            {
                WriteDebugLine("Error: " + ex.Message);
                WriteDebugLine("InnerException: " + ex.InnerException);
            }
        }
    }
}
