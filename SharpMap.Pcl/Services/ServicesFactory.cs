//==============================================================================
// Copyright 2010-2014 - Fabrizio Vita (www.itacasoft.com)
// This file can be redistributed and/or modified under the terms of the GNU Lesser General Public License.
//===============================================================================

using System.ComponentModel;
using SharpMap.WMS;

namespace SharpMap.Services
{
    public partial class ServicesFactory
    {
        private static ServicesFactory instance;
        protected SessionManager _sessionManager;

        private ServicesFactory() 
        {
            _sessionManager = new SessionManager();
        }

        public static ServicesFactory Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new ServicesFactory();
                }
                return instance;
            }
        }

        /// <summary>
        /// Url of TkMap
        /// </summary>
        public string TkMapUrl { get; set; }

        /// <summary>
        /// Project file
        /// </summary>
        protected string _projectFile;

        /// <summary>
        /// GIS project file.
        /// </summary>
        public string ProjectFile
        {
            get 
            {
                return _projectFile; 
            }
            set 
            {
                //sistemo eventuali slash sbagliati
                if (!string.IsNullOrEmpty(value))
                    value = value.Replace(@"\", "/");
                _projectFile = value;
            }
        }

        /// <summary>
        /// This method returns the service for operations on wms. 
        /// </summary>
        /// <param name="wmsUrl">Url of wms</param>
        /// <returns>The map service</returns>
        public IWMSService CreateWmsService(string wmsUrl)
        {
            var service = new WmsService(_sessionManager,wmsUrl);
            return service;
        }

        /// <summary>
        /// This method returns the service for operations on wms. 
        /// </summary>
        /// <returns>The map service</returns>
        public IWMSService CreateWmsService()
        {
            //assume that wmsUrl is the same of TkMap + /wms
            var service = new WmsService(_sessionManager, TkMapUrl + "/wms");
            return service;
        }


        /// <summary>
        /// This method returns the service for print operations. 
        /// </summary>
        /// <returns>The print service</returns>
        //public Services.IPrintService CreatePrintService()
        //{
        //    Services.IPrintService service = new Services.PrintService(this);
        //    return service;
        //}


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
