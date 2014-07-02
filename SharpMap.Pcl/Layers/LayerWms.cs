//==============================================================================
// Copyright 2010-2014 - Fabrizio Vita (www.itacasoft.com)
// This file can be redistributed and/or modified under the terms of the GNU Lesser General Public License.
//===============================================================================

using System;
using System.IO;
using SharpMap.Entities;
using SharpMap.WMS;
using System.Collections.Generic;
using Portable.Http;
using SharpMap.Services;
using SharpMap.Styles;
using SharpMap.GML;

namespace SharpMap.Layers
{
    public class LayerWms : LayerPixel, ISetupRemoteLayer
    {
        private int _mercatorEpsg = 3857;
        private string _imageFormat = "image/png";
        private Extent _boundingBox;
        private LegendSymbols _Symbols = new LegendSymbols();
        private WmsProjectInfo _wmsInfo;
        protected IWMSService wmsService;

        public LayerWms(WmsProjectInfo wmsInfo)
        {
            wmsService = new WmsService(wmsInfo.GetCapabilities.Url);
            wmsService.OnGetCapabilitiesCompleted += wmsService_OnGetCapabilitiesCompleted;
            wmsService.OnGetFeatureInfoCompleted += wmsService_OnGetFeatureInfoCompleted;
            Initialize(wmsInfo);
        }

        public LayerWms(string serviceName, string url)
        {
            WmsProjectInfo info = new WmsProjectInfo();
            info.LatLonBoundingBox = new Extent(-180, -90, 180, 90);
            info.Name = serviceName;
            info.GetMapInfo.Formats.Add("image/png");
            Initialize(info);

            wmsService = new WmsService(url);
            wmsService.OnGetFeatureInfoCompleted += wmsService_OnGetFeatureInfoCompleted;
            setupDone = true;
        }


        private void Initialize(WmsProjectInfo wmsInfo)
        {
            _wmsInfo = wmsInfo;
            _boundingBox = wmsInfo.LatLonBoundingBox;
            name = _wmsInfo.Name;
            Abstract = _wmsInfo.Abstract;
            Title = _wmsInfo.Title;

            if (!string.IsNullOrEmpty(_wmsInfo.MercatorEpsg))
                _mercatorEpsg = Convert.ToInt32(_wmsInfo.MercatorEpsg);

            if (!_wmsInfo.GetMapInfo.Formats.Contains("image/png") &&
                !_wmsInfo.GetMapInfo.Formats.Contains("image/png8"))
                throw new FormatException("Only PNG support is available at this time");

            if (!_wmsInfo.GetMapInfo.Formats.Contains("image/png"))
                _imageFormat = "image/png8";

            Layers = _wmsInfo.Layers;
        }

        bool setupDone = false;

        public void Setup(string url)
        {
            if (setupDone) return;
            wmsService = new WmsService(url);
            wmsService.OnGetCapabilitiesCompleted += wmsService_OnGetCapabilitiesCompleted;
            wmsService.OnGetFeatureInfoCompleted += wmsService_OnGetFeatureInfoCompleted;
            wmsService.GetCapabilities();
            setupDone = true;
        }

        protected virtual void wmsService_OnGetFeatureInfoCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            GisShapeList features = new GisShapeList();

            try
            {
                if (e.Result == "") return;

                Extent extent;
                features = GMLProviderWrapper.GetFeatures(e.Result, out extent);
            }
            finally
            {
                RaiseGetFeatureInfoCompleted(features);
            }
        }

        public LayerWms()
        {

        }

        protected void RaiseGetFeatureInfoCompleted(GisShapeList features)
        {
            if (OnGetFeatureInfoCompleted != null)
                OnGetFeatureInfoCompleted(this, new FeaturesEventArgs(features));
        }

        /// <summary>
        /// Event thrown when a getfeature info is completed.
        /// </summary>
        public event EventHandler<FeaturesEventArgs> OnGetFeatureInfoCompleted;

        public List<WmsLayerInfo> Layers { get; private set; }

        /// <summary>
        /// Gets or sets comments to the layer.
        /// </summary>
        public string Abstract { get; set; }

        /// <summary>
        /// Title
        /// </summary>
        public string Title { get; private set; }

        public override Extent BoundingBox
        {
            get { return _boundingBox; }
            set { _boundingBox = value; }
        }

        public int MercatorEpsg
        {
            get { return _mercatorEpsg; }
            set
            {
                if ((value == 3857) || (value == 900913))
                    _mercatorEpsg = value;
                else
                {
                    throw new ArgumentOutOfRangeException(string.Format("Invalid mercator EPSG {0}", value));
                }
            }
        }

        public override string ImageFormat
        {
            get { return _imageFormat; }
            set { _imageFormat = value; }
        }

        public override bool Draw()
        {
            throw new NotImplementedException();
        }

        public override Stream Image
        {
            get { throw new NotImplementedException(); }
        }

        public void GetFeatureInfo(int screenX, int screenY, Size actualSize, Extent locationRect)
        {
            wmsService.GetFeatureInfo(screenX, screenY, actualSize, locationRect);
        }

        public string GetMapRequest(Size actualSize, IEnumerable<string> layers)
        {
            return wmsService.GetMapRequest(BoundingBox, actualSize, layers);
        }

        public string GetMapRequest(Extent extent, Size actualSize, IEnumerable<string> layers)
        {
            return wmsService.GetMapRequest(extent, actualSize, layers);
        }

        public bool IsReady
        {
            get { return Layers != null; }
        }

        void wmsService_OnGetCapabilitiesCompleted(object sender, WmsProjectEventArgs e)
        {
            Initialize(e.Value);
            if (OnSetupCompleted != null)
                OnSetupCompleted(this, new EventArgs());
        }

        public event EventHandler OnSetupCompleted;

        public string ServiceUrl
        {
            get
            {
                return wmsService.ServiceUrl;

            }
            set
            {
                wmsService.ServiceUrl = value;
            }
        }

        /// <summary>
        /// Indicates if to force getfeatureinfo request on WGS84 even if GetMap is on Mercatore
        /// </summary>
        public bool ForceWgs84ForFeatureInfo 
        { 
            get
            {
                return wmsService.ForceWgs84ForFeatureInfo;
            }
            set
            {
                wmsService.ForceWgs84ForFeatureInfo = value;
            }
        }


        public void InitializeConnection(IWebClient webClient)
        {
            wmsService.InitializeConnection(webClient);
        }
    }
}
