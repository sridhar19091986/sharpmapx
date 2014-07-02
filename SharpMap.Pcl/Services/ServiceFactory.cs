//==============================================================================
// Author: Fabrizio Vita            
// Date: 2010-10-18
// Copyright: (c)2010-2011 ItacaSoft di Vita Fabrizio. ALL RIGHTS RESERVED.
//===============================================================================
using System;
using System.Windows;
using System.Collections.Generic;
using SMS.Core.Entities;
using System.ComponentModel;
using SMS.Core.Utilities;

namespace SMS.Core.Services
{
    /// <summary>
    /// Factory class for construction of web services consumers towards WMS and asp.net part of SMS.
    /// </summary>
    public class ServiceFactory : INotifyPropertyChanged
    {
        private string _wmsUrl;

        /// <summary>
        /// Url of WMS
        /// </summary>
        public string WMSUrl
        {
            get { return _wmsUrl; }
            set { _wmsUrl = value; }
        }

        /// <summary>
        /// Project file
        /// </summary>
        protected string _projectFile;

        /// <summary>
        /// GIS project file.
        /// </summary>
        public string GISProjectFile
        {
            get { return _projectFile; }
            set { _projectFile = value; }
        }

        /// <summary>
        /// Parameters on query url.
        /// </summary>
        protected IDictionary<string, string> _queryParams = null;

        /// <summary>
        /// Creates a new instance of the <see cref="ServiceFactory"/>
        /// </summary>
        /// <param name="parent">Parent element, necessary for dispatcher in async operations</param>
        /// <param name="projectFile">GIS project file</param>
        /// <param name="isapiUrl">URL of WMS</param>
        public ServiceFactory(string projectFile, string isapiUrl)
        {
            _projectFile = projectFile;
           //sistemo eventuali slash sbagliati
           _projectFile = _projectFile.Replace(@"\", "/");

           _wmsUrl = isapiUrl;

            _userID = new Guid("00000000-0000-0000-0000-000000000001");
        }

        #region Events
        /// <summary>
        /// This event is thrown when a line of text has to be sent to the debug output
        /// </summary>
        public event EventHandler<StringEventArgs> OnDebugMessage;
        #endregion

        private Guid _sessionID = Guid.Empty;

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

        /// <summary>
        /// Evaluates the received xml to see if the server has thrown an exception.
        /// </summary>
        /// <param name="xml">XML received from server.</param>
        /// <returns>True is an exception has been thrown.</returns>
        public bool CheckException(string xml)
        {
            //xml = xml.Replace("<!DOCTYPE  ServiceExceptionReport SYSTEM \"http://www.itacasoft.com/wms/1.1.1/exception_1_1_1.dtd\">", "");
            int i0 = xml.IndexOf("<!DOCTYPE");
            if (i0 >= 0)
            {
                int i1 = xml.IndexOf(">", i0);
                xml = xml.Remove(i0, 1 + i1 - i0);
            }

            if (ObjectXMLSerializer.CheckObjectType(xml, typeof(ServiceExceptionReport)))
            {
                ServiceExceptionReport ex = ObjectXMLSerializer<ServiceExceptionReport>.DeSerializeObject(xml);

                WriteDebugLine("Error: " + ex.ServiceException);
                WriteDebugLine("StackTrace: " + ex.StackTrace);
                return true;
            }
            return false;
        }

        /// <summary>
        /// This method returns the service for SQL operations. 
        /// </summary>
        /// <returns>The SQL Service</returns>
        public Services.ISQLService GetSQLService()
        {
            Services.ISQLService service = new Services.SQLService(this);
            return service;
        }

        /// <summary>
        /// This method returns the service for operations on map. 
        /// </summary>
        /// <returns>The map service</returns>
        public Services.IMapServerService GetMapService()
        {
            Services.IMapServerService service = new Services.MapServerService(this);
            return service;
        }

        /// <summary>
        /// This method returns the service for print operations. 
        /// </summary>
        /// <returns>The print service</returns>
        //public Services.IPrintService GetPrintService()
        //{
        //    Services.IPrintService service = new Services.PrintService(this);
        //    return service;
        //}

        /// <summary>
        /// This method returns the service for operations on POIs. 
        /// </summary>
        /// <returns>The POIs service</returns>
        public Services.IPOIService GetPOIService()
        {
            Services.IPOIService service = new Services.POIService(this);
            return service;
        }

        /// <summary>
        /// This method returns the service for selection operations. 
        /// </summary>
        /// <returns>The print service</returns>
        public Services.ISelectionsService GetSelectionService()
        {
            Services.ISelectionsService service = new Services.SelectionsService(this);
            return service;
        }

        /// <summary>
        /// Writes a text line to the debug output. 
        /// </summary>
        /// <param name="line">Thext line to be written.</param>
        public void WriteDebugLine(string line)
        {
            if (OnDebugMessage != null)
                OnDebugMessage(this, new StringEventArgs(line));
        }

        private bool _busy = false;

        /// <summary>
        /// Gets or sets the busy state of the service.
        /// </summary>
        /// <remarks>The service is busy during asyncronous requests.</remarks>
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

    }
}
