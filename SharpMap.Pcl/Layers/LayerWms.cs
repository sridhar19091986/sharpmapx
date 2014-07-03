//==============================================================================
// Copyright 2010-2014 - Fabrizio Vita (www.itacasoft.com)
// This file can be redistributed and/or modified under the terms of the GNU Lesser General Public License.
//===============================================================================

using System;
using System.IO;
using System.Linq;
using SharpMap.Entities;
using SharpMap.WMS;
using System.Collections.Generic;
using Portable.Http;
using SharpMap.Styles;
using SharpMap.GmlUtils;
using System.Text;
using System.Globalization;
using SharpMap.Utilities;

namespace SharpMap.Layers
{
    /// <summary>
    /// EventArgs class for a Wms project.
    /// </summary>
    public class WmsProjectEventArgs : EventArgs
    {
        /// <summary>
        /// Creates a new instance of the <see cref="WmsProjectEventArgs"/>
        /// </summary>
        /// <param name="value">Value.</param>
        public WmsProjectEventArgs(WmsProjectInfo value)
            : base()
        {
            _value = value;
        }

        WmsProjectInfo _value;

        /// <summary>
        /// Value.
        /// </summary>
        public WmsProjectInfo Value
        {
            get
            {
                return _value;
            }
        }
    }


    public class LayerWms : LayerPixel, ISetupRemoteLayer
    {
        private int _mercatorEpsg = 3857;
        private string _imageFormat = "image/png";
        private Extent _boundingBox;
        private LegendIcons _Symbols = new LegendIcons();
        private WmsProjectInfo _wmsInfo;
        protected WmsService wmsService;

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
            GisShapeCollection features = new GisShapeCollection();

            try
            {
                if (e.Result == "") return;

                Extent extent;
                features = GmlReader.GetShapes(e.Result, out extent);
            }
            finally
            {
                RaiseGetFeatureInfoCompleted(features);
            }
        }

        public LayerWms()
        {

        }

        protected void RaiseGetFeatureInfoCompleted(GisShapeCollection features)
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

        protected class WmsService
        {
            IWebClient _webClient;
            string _serviceUrl;
            bool _serviceUrlContainsQuestionMark;

            /// <summary>
            /// Creates a new instance of the <see cref="WmsService"/>
            /// </summary>
            /// <param name="serviceUrl">Url of this service</param>
            public WmsService(string serviceUrl)
            {
                IMAGE_FORMAT = "image/png; mode=24bit";
                _serviceUrl = serviceUrl;
                _serviceUrlContainsQuestionMark = (_serviceUrl != null) && (_serviceUrl.Contains("?"));
            }

            /// <summary>
            /// Event thrown when a getfeature info is completed.
            /// </summary>
            public event EventHandler<DownloadStringCompletedEventArgs> OnGetFeatureInfoCompleted;

            public event EventHandler<WmsProjectEventArgs> OnGetCapabilitiesCompleted;

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

            private string _mercatorEpsg = "3857";
            public string MERCATOR_EPSG
            {
                get
                {
                    return _mercatorEpsg;
                }

                set
                {
                    _mercatorEpsg = value;
                }
            }

            private string _queryLayers = "";
            /// <summary>
            /// Comma separated list of layers names to query
            /// </summary>
            public string QueryLayers
            {
                get
                {
                    return _queryLayers;
                }
                set
                {
                    _queryLayers = value;
                }
            }

            private string _layers = "";
            /// <summary>
            /// Comma separated list of layers names to view
            /// </summary>
            public string Layers
            {
                get
                {
                    return _layers;
                }
                set
                {
                    _layers = value;
                }
            }

            public bool ForceWgs84ForFeatureInfo { get; set; }

            #region GetCapabilities
            /// <summary>
            /// Returns the url for requesting e getcapabilities.
            /// </summary>
            private string GetCapabilitiesRequest()
            {
                if (_serviceUrl == "")
                    throw new Exception("Missing property WMSUrl");

                var b = new StringBuilder();
                b.Append(_serviceUrl);

                if (!_serviceUrl.Contains("?"))
                    b.Append("?");
                if (!b.ToString().EndsWith("&") && !b.ToString().EndsWith("?"))
                    b.Append("&");
                if (!_serviceUrl.ToLower().Contains("service=wms"))
                    b.AppendFormat("SERVICE=WMS&");
                if (!_serviceUrl.ToLower().Contains("request=getcapabilities"))
                    b.AppendFormat("REQUEST=GetCapabilities&");

                //If version is NOT set at this point then add to query string
                if (!_serviceUrl.ToLower().Contains("version="))
                    b.AppendFormat("VERSION=1.1.1");

                return b.ToString();
            }

            public void GetCapabilities(string wmsUrl)
            {
                //Esempio: http://rsdi.regione.basilicata.it/geoserver/wms?VERSION=1.1.1&SERVICE=WMS&REQUEST=GetCapabilities

                if (OnGetCapabilitiesCompleted == null)
                    throw new Exception("OnGetCapabilitiesCompleted cannot be null");

                BusyState = true;

                WebClient client = new WebClient();
                client.DownloadStringCompleted += new DownloadStringCompletedEventHandler(GetCapabilitiesCompleted);
                string requesturl = wmsUrl + @"?request=GetCapabilities&Service=WMS&Version=1.1.1";
                client.DownloadStringAsync(new Uri(requesturl, UriKind.RelativeOrAbsolute));
            }

            /// <summary>
            /// Initiates a getcapabilities request.
            /// </summary>
            public void GetCapabilities()
            {
                //Esempio: http://rsdi.regione.basilicata.it/geoserver/wms?VERSION=1.1.1&SERVICE=WMS&REQUEST=GetCapabilities

                if (OnGetCapabilitiesCompleted == null)
                    throw new Exception("OnGetCapabilitiesCompleted cannot be null");

                BusyState = true;
                WebClient client = new WebClient();
                client.DownloadStringCompleted += new DownloadStringCompletedEventHandler(GetCapabilitiesCompleted);
                string requesturl = GetCapabilitiesRequest();
                client.DownloadStringAsync(new Uri(requesturl, UriKind.RelativeOrAbsolute));
            }

            void GetCapabilitiesCompleted(object sender, DownloadStringCompletedEventArgs e)
            {
                try
                {
                    if (WmsUtils.CheckException(e.Result)) return;

                    var parsed = WmsParser.Read(e.Result);

                    if (OnGetCapabilitiesCompleted != null)
                        OnGetCapabilitiesCompleted(sender, new WmsProjectEventArgs(parsed));

                }
                catch (Exception ex)
                {
                    throw;
                }
                finally
                {
                    BusyState = false;
                }
            }
            #endregion

            #region GetFeatureInfo
            /// <summary>
            /// Returns the url for requesting e getfeatureinfo.
            /// </summary>
            private string GetFeatureInfoRequest(int x, int y, Size actualSize, Extent locationRect)
            {
                //ReQuEsT=GetFeatureInfo&WiDtH=100&X=50&Y=50&HeIgHt=100&QuErY_LaYeRs=streets&BbOx=-2,2,2,6&StYlEs=&InFo_fOrMaT=text/gml&SrS=EPSG:4326&LaYeRs=streets

                if (_serviceUrl == "")
                    throw new Exception("Missing property WMSUrl");

                if (!ForceWgs84ForFeatureInfo)
                    locationRect = ProjectionConversion.ConvertToMercatore(locationRect);

                var b = new StringBuilder();
                b.Append(_serviceUrl);
                b.Append(_serviceUrlContainsQuestionMark ? "&" : "?");
                b.Append("request=getfeatureinfo");

                b.Append("&SRS=EPSG:" + MERCATOR_EPSG);

                b.Append("&info_format=application/vnd.ogc.gml&styles=");

                b.Append("&query_layers=");
                b.Append(_queryLayers);

                b.Append("&layers=");
                b.Append(_layers);

                b.Append("&width=");
                b.Append(actualSize.Width);

                b.Append("&height=");
                b.Append(actualSize.Height);

                b.Append("&bbox=");

                b.Append(string.Format(CultureInfo.InvariantCulture, "{0}", locationRect.MinX));
                b.Append(",");
                b.Append(string.Format(CultureInfo.InvariantCulture, "{0}", locationRect.MinY));
                b.Append(",");
                b.Append(string.Format(CultureInfo.InvariantCulture, "{0}", locationRect.MaxX));
                b.Append(",");
                b.Append(string.Format(CultureInfo.InvariantCulture, "{0}", locationRect.MaxY));

                b.Append("&x=");
                b.Append(x);

                b.Append("&y=");
                b.Append(y);

                return b.ToString();
            }

            private bool _busy = false;

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
                    }
                }
            }


            /// <summary>
            /// Initiates a getfeatureinfo request.
            /// </summary>
            /// <param name="x">Position x on the screen</param>
            /// <param name="y">Position y on the screen</param>
            /// <param name="queryLayer">Layer where to do the query</param>
            /// <param name="actualSize">Size on the screen</param>
            /// <param name="locationRect">extent</param>
            public void GetFeatureInfo(int x, int y, Size actualSize, Extent locationRect)
            {
                if (OnGetFeatureInfoCompleted == null)
                    throw new Exception("OnGetFeatureInfoCompleted cannot be null");

                BusyState = true;
                if (_webClient == null)
                {
                    _webClient = new WebClient();
                    _webClient.DownloadStringCompleted += new DownloadStringCompletedEventHandler(GetFeatureInfoCompleted);
                }
                string requesturl = GetFeatureInfoRequest(x, y, actualSize, locationRect);
                _webClient.DownloadStringAsync(new Uri(requesturl, UriKind.RelativeOrAbsolute));
            }

            void GetFeatureInfoCompleted(object sender, DownloadStringCompletedEventArgs e)
            {
                try
                {
                    if (WmsUtils.CheckException(e.Result)) return;

                    if (OnGetFeatureInfoCompleted != null)
                        OnGetFeatureInfoCompleted(sender, e);
                }
                catch (Exception ex)
                {
                    throw;
                }
                finally
                {
                    BusyState = false;
                }
            }
            #endregion

            #region GetMap
            private StringBuilder InternalGetMapRequest(Size actualSize, IEnumerable<string> layers, bool useCache = true)
            {
                if (_serviceUrl == "")
                    throw new Exception("Missing property WMSUrl");

                var b = new StringBuilder();
                b.Append(_serviceUrl);
                b.Append(_serviceUrlContainsQuestionMark ? "&" : "?");
                b.Append("request=getmap");
                b.Append("&width=");
                b.Append(actualSize.Width.ToString());
                b.Append("&height=");
                b.Append(actualSize.Height.ToString());
                b.Append("&");

                if ((layers == null) || (layers.Count() == 0))
                {
                    b.Append("layers=");
                }
                else
                {
                    b.Append("layers=");
                    foreach (var layer in layers)
                    {
                        b.Append(layer);
                        b.Append(",");
                    }

                    b.Remove(b.Length - 1, 1);
                }

                b.Append("&SRS=EPSG:" + MERCATOR_EPSG);

                b.Append("&format=" + IMAGE_FORMAT);
                b.Append("&transparent=true");
                b.Append("&transparentcolor=0xFFFFFF");

                if (!useCache)
                    //passando i ticks, viene artificialmente modificata la url, quindi 
                    //la cache viene ignorata
                    b.Append("&" + DateTime.Now.Ticks.ToString());

                //NO, c'è un limite di 2047 nell'URL, e per tanti elementi selezionati (> circa 200-300), 
                //si raggiunge facilmente questo limite
                /*
                if ((gis.SelectedElements != null) && (gis.SelectedElements.Count > 0))
                {
                    b.Append("&selecteditems=");

                    for (int i = 0; i < gis.SelectedElements.Count; i++)
                    {
                        b.Append((gis.SelectedElements[i].Geometry as SMS.Core.Entities.Geometry).UID);
                        if (i < (gis.SelectedElements.Count - 1))
                            b.Append("+");
                    }
                }
                 */

                return b;
            }


            /// <summary>
            /// Returns an url for obtaining a map from WMS.
            /// </summary>
            /// <param name="locationRect">Extent of the map</param>
            /// <param name="actualSize">Size of the map in pixel</param>
            /// <param name="layers">List of layers to view</param>
            /// <param name="useCache">If set to true, use the current cache</param>
            /// <returns>Url to be called</returns>
            public virtual string GetMapRequest(Extent locationRect, Size actualSize, IEnumerable<string> layers, bool useCache = true)
            {
                StringBuilder b = InternalGetMapRequest(actualSize, layers, useCache);

                var extentMercatore = ProjectionConversion.ConvertToMercatore(locationRect);

                b.Append("&bbox=");

                b.Append(string.Format(CultureInfo.InvariantCulture, "{0}", extentMercatore.MinX));
                b.Append(",");
                b.Append(string.Format(CultureInfo.InvariantCulture, "{0}", extentMercatore.MinY));
                b.Append(",");
                b.Append(string.Format(CultureInfo.InvariantCulture, "{0}", extentMercatore.MaxX));
                b.Append(",");
                b.Append(string.Format(CultureInfo.InvariantCulture, "{0}", extentMercatore.MaxY));

                return b.ToString();
            }
            #endregion

            public string IMAGE_FORMAT
            {
                get;
                set;
            }

            public void InitializeConnection(IWebClient webClient)
            {
                if (_webClient != null)
                {
                    _webClient.DownloadStringCompleted -= GetFeatureInfoCompleted;
                }

                _webClient = webClient;
                _webClient.DownloadStringCompleted += GetFeatureInfoCompleted;
            }


            /*
                        //esempio di chiamata (gml):
                        //http://rsdi.regione.basilicata.it/geoserver/wms?VERSION=1.1.1&SERVICE=WMS&REQUEST=GetFeatureInfo&BBOX=15.3484689043799,40.2707315886485,16.1038691585095,41.003494987216&info_format=application/vnd.ogc.gml&styles=&query_layers=rsdi:corine&WIDTH=901&HEIGHT=874&SRS=EPSG:4326&TRANSPARENT=TRUE&STYLES=&LAYERS=topp:autostrade,sirs:Carta_suoli_2006_utente_2_543,rsdi:corine&x=200&y=200
                        //sua risposta:
                        <?xml version="1.0" encoding="UTF-8"?><wfs:FeatureCollection xmlns="http://www.opengis.net/wfs" xmlns:wfs="http://www.opengis.net/wfs" xmlns:gml="http://www.opengis.net/gml" xmlns:rsdi="http://www.opengeospatial.net/rsdi" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xsi:schemaLocation="http://www.opengeospatial.net/rsdi http://rsdi.regione.basilicata.it:80/geoserver/wfs?service=WFS&amp;version=1.0.0&amp;request=DescribeFeatureType&amp;typeName=rsdi:corine http://www.opengis.net/wfs http://rsdi.regione.basilicata.it:80/geoserver/schemas/wfs/1.0.0/WFS-basic.xsd"><gml:boundedBy><gml:Box srsName="http://www.opengis.net/gml/srs/epsg.xml#32633"><gml:coordinates xmlns:gml="http://www.opengis.net/gml" decimal="." cs="," ts=" ">536392.192913,4506612.245965 557301.997967,4525423.658472</gml:coordinates></gml:Box></gml:boundedBy><gml:featureMember><rsdi:corine fid="corine.53"><gml:boundedBy><gml:Box srsName="http://www.opengis.net/gml/srs/epsg.xml#32633"><gml:coordinates xmlns:gml="http://www.opengis.net/gml" decimal="." cs="," ts=" ">536392.192913,4506612.245965 557301.997967,4525423.658472</gml:coordinates></gml:Box></gml:boundedBy><rsdi:gid>53</rsdi:gid><rsdi:perimetro>257346</rsdi:perimetro><rsdi:classe5>TERRITORI BOSCATI E AMBIENTI SEMI-NATURALI</rsdi:classe5><rsdi:classe15>ZONE BOSCATE</rsdi:classe15><rsdi:classe44>BOSCHI DI LATIFOGLIE</rsdi:classe44><rsdi:fid>1080</rsdi:fid><rsdi:the_geom><gml:MultiPolygon srsName="http://www.opengis.net/gml/srs/epsg.xml#32633"><gml:polygonMember><gml:Polygon><gml:outerBoundaryIs><gml:LinearRing><gml:coordinates xmlns:gml="http://www.opengis.net/gml" decimal="." cs="," ts=" ">536433.22164912,4525158.84877673 536453.90256566,4525159.36245155 536457.66150027,4525159.60304887 536490.78778618,4525159.32280169 536519.79083725,4525164.30915412 536555.82563733,4525157.34135246 536594.78959272,4525150.16623606 536639.88199996,4525130.58860774 536642.79499271,4525129.38196364 536670.82937891,4525119.4748402 536685.86464724,4525112.57932629 536687.090192,4525109.445408 536669.753915,4525115.132783 536673.315234,4525093.414681 536667.792196,4525050.348393 536673.349584,4525049.466479 536721.72262,4524953.253194 536770.992221,4524880.948254 536814.631005,4524783.063036 536876.772516,4524691.894782 536963.02233,4524594.573516 537003.951397,4524525.339273 537054.720262,4524538.325685 537100.475277,4524544.664932 537109.406032,4524546.547685 537225.752833,4524481.113222 537271.951697,4524385.050378 537417.570804,4524388.513432 537461.536673,4524451.405517 537522.05036,4524467.216593 537560.133537,4524444.621396 537632.894647,4524446.106111 537643.937956,4524502.27514 537704.997211,4524550.507936 537744.279551,4524431.95034 537752.429449,4524358.980386 537623.865643,4524314.892839 537573.342683,4524251.95217 537564.314288,4524120.73903 537594.443497,4524078.715953 537677.229816,4524033.569329 537751.803469,4524061.396447 537817.275343,4524149.275901 537906.129483,4524288.479427 537991.392105,4524375.495734 538079.883026,4524412.352943 538095.776367,4524384.792313 538138.668074,4524311.429695 538146.721363,4524341.039817 538146.80352588,4524341.34190515 538154.68618304,4524333.77496711 538175.91596868,4524298.34756113 538192.91611539,4524274.2075092 538206.47411051,4524259.79083739 538216.93959511,4524248.59135135 538236.56941161,4524227.26803237 538241.90893458,4524221.39827408 538263.35468892,4524207.438878 538270.94406652,4524202.42726194 538299.03160517,4524191.51357724 538342.03020651,4524179.56735876 538375.60010293,4524165.76842828 538388.96433148,4524160.35680214 538397.30216553,4524157.29189143 538422.04976465,4524148.5953779 538445.85422677,4524138.96432031 538462.94363721,4524132.29561647 538480.19920111,4524128.11167731 538508.95939901,4524121.6427119 538552.3965567,4524118.65021901 538555.93722134,4524118.41894458 538601.0394162,4524115.31055077 538636.96073583,4524119.32840815 538650.5993331,4524117.90119063 538682.95595257,4524114.67104363 538704.08977017,4524114.21293295 538708.96619918,4524114.37727489 538746.93700489,4524117.26856192 538781.05607095,4524111.42547582 538782.69209495,4524110.80964 538820.97627442,4524100.18742483 538832.37310206,4524102.39821981 538841.98107077,4524104.23883198 538859.77179684,4524110.50745951 538878.99041288,4524117.67173767 538893.38261308,4524129.16778947 538906.944941,4524140.21545231 538928.0838969,4524160.73900351 538931.96183812,4524164.46443958 538943.06862412,4524181.68805397 538954.85782175,4524193.35088462 538962.95868625,4524201.28928094 538972.23454218,4524210.13585298 538980.05352593,4524217.58710875 538994.76940186,4524225.57436505 539012.98385411,4524235.30081168 539035.74522752,4524247.71561078 539039.05399716,4524249.49540868 539043.59053934,4524250.17159867 539070.9727296,4524255.28426079 539098.96590999,4524268.33752003 539114.06069611,4524286.27635055 539119.89027201,4524299.36487718 539123.9601863,4524308.57258354 539138.95950385,4524342.49649015 539151.96746404,4524364.57335627 539159.39593292,4524380.04325367 539161.94849269,4524385.35755854 539163.01796995,4524388.27726393 539170.98359275,4524409.70297045 539177.37145705,4524422.75288239 539183.98852691,4524436.27581286 539193.92217441,4524450.08001709 539200.07044097,4524458.6466228 539213.44202905,4524469.70773076 539217.9413494,4524473.39499618 539225.91283515,4524475.83797682 539249.98252954,4524483.68122095 539262.47179014,4524488.3129838 539277.98782439,4524494.23750306 539294.04614068,4524507.11698668 539307.07241717,4524517.70463359 539338.96195086,4524548.47191327 539352.95430078,4524569.4720472 539359.08883214,4524582.53465056 539367.05659066,4524599.46466285 539383.82027039,4524616.2889926 539387.97954201,4524620.50185042 539408.06504736,4524636.5946494 539416.97108257,4524643.48326065 539433.95618394,4524656.28475229 539476.98896828,4524669.30887073 539527.98525435,4524680.28057607 539585.97462766,4524699.26905292 539607.57007223,4524702.78466117 539629.99529184,4524706.73495677 539674.07603208,4524718.68350782 539711.97917224,4524731.55653752 539719.74195193,4524735.51145682 539743.97312148,4524748.32493532 539772.96916476,4524764.31411241 539816.06275982,4524787.31765124 539821.99212976,4524810.88394851 539824.99373979,4524822.65342242 539834.95554506,4524850.43216023 539849.94655728,4524873.38135363 539853.76777279,4524881.60292154 539865.98184824,4524907.72871105 539870.91404488,4524940.35092794 539876.06996211,4524974.44525359 539851.47781848,4525011.99723213 539864.255615,4525006.65157 539891.281483,4524980.32254 539907.277963,4524964.241029 539961.661034,4524890.08861 539972.071358,4524855.416062 540086.337751,4524830.573986 540211.578438,4524707.602106 540298.480999,4524764.041743 540220.051769,4524967.68433 540347.243731,4524991.888614 540400.745658,4524935.774087 540413.894835,4524936.367513 540501.167789,4524829.491308 540540.840705,4524733.88027 540519.592382,4524616.495616 540518.898911,4524511.179309 540539.802319,4524430.343348 540390.597499,4524374.694929 540341.888546,4524338.098431 540301.741869,4524318.889232 540302.356981,4524317.848492 540284.322477,4524227.207022 540306.212138,4524125.330183 540411.735492,4524064.138543 540409.115549,4524037.852863 540471.385474,4524012.096151 540516.249107,4523896.653634 540636.224636,4523815.992557 540716.716527,4523737.548463 540786.328377,4523693.309552 540824.410077,4523670.715521 540863.701168,4523613.083112 540947.896148,4523604.793301 541101.358876,4523507.34762 541159.915007,4523568.234341 541242.146016,4523544.100754 541287.786895,4523616.862052 541246.454704,4523725.56802 541298.11795,4523782.435042 541412.390632,4523724.138032 541419.649317,4523717.146934 541426.557166,4523817.540377 541451.243244,4523888.747095 541582.947412,4523978.556251 541667.282862,4524052.149496 541752.06331,4524132.204291 541812.821706,4524246.867836 541899.859608,4524359.723579 541920.662959,4524470.645396 541978.241412,4524539.08853 541961.486155,4524676.563871 541949.318475,4524663.918858 541881.793387,4524684.045329 541820.666314,4524649.302689 541770.74909,4524692.186523 541653.574794,4524812.60378 541774.844919,4524943.077229 541828.291769,4524952.381751 541984.558007,4525014.53259 542135.955591,4525102.983471 542169.870181,4525109.637907 542197.182772,4525188.153053 542244.772222,4525303.722057 542282.247848,4525367.556035 542371.25988,4525413.861251 542424.802425,4525423.658472 542501.442092,4525385.426361 542622.302286,4525317.686292 542726.372998,4525278.065709 542727.000726,4525200.124964 542783.884455,4525163.253703 542888.933059,4525160.517921 543118.372085,4525038.866333 543335.563655,4524931.54092 543572.954849,4524829.317112 543769.911524,4524716.394571 543854.244948,4524646.675612 543883.76892,4524642.146857 544009.164565,4524559.615539 544062.847166,4524497.998512 544065.747853,4524494.803297 544013.034156,4524409.049777 544202.195732,4524277.190792 544281.794089,4524185.828013 544350.293278,4524029.317332 544466.575523,4523895.983246 544622.333107,4523759.43441 544634.884171,4523744.089998 544701.486771,4523661.61209 544706.073747,4523535.964705 544741.614744,4523381.223352 544832.604802,4523263.113445 544927.630331,4523203.646678 545016.364549,4523181.563048 545015.420538,4523189.617099 545240.473677,4523142.176153 545330.468241,4523113.015845 545330.614047,4523111.507507 545336.826467,4523111.080535 545346.054326,4523110.944439 545495.454231,4523105.159846 545534.065003,4523093.016121 545773.682925,4523019.607609 545995.861965,4522984.84789 546004.589627,4522979.254128 546065.435474,4522940.115937 546191.48116,4522852.550872 546186.076408,4522842.936354 546154.133845,4522788.710743 546043.943369,4522723.890203 545907.722368,4522759.726017 545824.551631,4522771.939118 545821.665595,4522762.650379 545665.6384,4522771.886593 545662.550476,4522765.108609 545629.398873,4522691.492004 545621.017867,4522673.094581 545533.930506,4522690.070633 545500.906791,4522696.337382 545364.596127,4522698.22522 545309.226887,4522699.037977 545120.734994,4522595.662678 545074.203515,4522474.531251 544932.437811,4522404.391289 544830.664315,4522438.356183 544768.625231,4522485.5661 544691.876136,4522416.945109 544706.972414,4522351.49336 544829.760338,4522289.119328 544877.803236,4522219.903148 544907.54844,4522171.419255 544898.074324,4522157.091557 544921.093464,4522156.007619 544988.080579,4522015.582235 545075.319936,4521893.7379 545135.10622,4521893.619849 545149.690377,4522083.858435 545268.395909,4522163.574349 545299.042536,4522278.807386 545357.135011,4522261.329473 545430.388449,4522173.902186 545471.258356,4522180.078128 545393.454327,4522101.543735 545341.924141,4522025.696356 545286.006135,4521884.239171 545275.135531,4521726.201459 545308.62124,4521699.431527 545299.625821,4521692.060491 545300.035494,4521678.051682 545223.640169,4521650.850101 545165.205631,4521568.985047 545068.907744,4521484.731252 545062.830249,4521521.600085 544986.294054,4521561.317906 544995.671142,4521639.566415 544993.84131,4521640.191821 544933.528101,4521854.057502 544832.867288,4521822.533331 544809.984112,4521711.259438 544700.295655,4521637.413605 544653.602002,4521564.726996 544739.598899,4521497.394959 544835.988167,4521412.370759 544806.699983,4521166.219964 544896.509453,4521147.060625 544887.282722,4521092.769394 545008.445793,4520974.084956 545050.353668,4520862.849085 544990.947022,4520759.57947 544912.9637,4520816.373381 544808.818047,4520908.421516 544796.980241,4520888.762761 544701.543877,4520982.209475 544648.32778,4520880.011928 544702.033325,4520796.924982 544764.307139,4520762.183634 544893.454357,4520697.876565 545017.988014,4520586.451601 545025.166406,4520596.443436 545133.899511,4520641.894698 545137.806932,4520640.627352 545250.298093,4520680.826515 545259.706155,4520679.680262 545386.976397,4520862.168161 545364.920884,4521022.470192 545242.0503,4521156.250743 545349.463174,4521298.660548 545350.966667,4521300.55486 545426.010766,4521335.338878 545488.419397,4521376.984542 545640.067156,4521373.045031 545670.755296,4521370.93434 545703.797017,4521243.332225 545641.739912,4521108.788753 545605.009875,4520959.520791 545607.338311,4520904.43569 545601.386931,4520906.842307 545547.158207,4520791.732597 545495.753863,4520715.875482 545438.688019,4520653.889781 545434.541051,4520651.678384 545432.809252,4520568.410498 545616.630818,4520456.90363 545644.919945,4520389.04795 545639.633545,4520388.412376 545647.083814,4520322.48904 545614.422006,4520232.360508 545589.677831,4520161.161537 545558.951336,4520097.363875 545432.595816,4520086.079467 545306.398619,4520075.283373 545304.285013,4520169.801153 545216.844236,4520279.673198 545132.413854,4520264.507172 544986.401239,4520150.715206 545013.251605,4520036.521118 545101.703551,4520013.960886 545204.962394,4520008.858403 545205.909707,4520006.296639 545236.128882,4520013.705559 545332.217116,4519875.276833 545486.383639,4519811.748665 545599.090693,4519816.98135 545698.708794,4519806.136865 545700.991549,4519808.476602 545728.672015,4519976.841592 545760.604186,4520114.455317 545777.314798,4520123.292687 545806.022887,4520232.167364 545954.924005,4520380.711795 546031.037878,4520527.271003 546105.340398,4520647.490077 546185.817696,4520761.292445 546299.184664,4520872.333817 546331.71724,4520857.114271 546331.630491,4520863.111837 546464.000782,4520867.489618 546506.08571,4520904.041356 546533.530303,4521014.500041 546686.099039,4521023.979947 546748.199485,4520966.781226 546878.882317,4520944.811171 546945.444474,4520945.22679 546964.804023,4520945.393198 547022.792498,4521020.795542 547067.701738,4521096.598252 547077.648046,4521241.214306 547134.269295,4521296.738495 547232.689623,4521382.841179 547222.695431,4521518.841969 547222.40349,4521521.857926 547414.479889,4521528.619004 547441.675156,4521504.77914 547443.743762,4521507.632354 547545.099089,4521516.638981 547552.982166,4521515.098342 547672.815973,4521636.675844 547673.260618,4521643.1368 547922.627173,4521618.993469 547922.524836,4521617.502975 547907.279124,4521395.85991 547656.95536,4521406.087224 547591.754594,4521225.827056 547579.802702,4521222.654599 547470.777631,4521194.702091 547324.009221,4521078.970433 547288.771077,4520949.575506 547134.974014,4520827.835057 547012.758537,4520707.917389 547017.090866,4520680.157096 546917.619781,4520515.233775 546806.02392,4520379.105838 546835.41619,4520209.315023 546798.977114,4520071.513823 546865.670218,4519888.672745 546917.792612,4519920.540162 546924.750203,4519900.089083 546926.175791,4519904.484803 546957.913996,4519981.194018 546960.647895,4520020.950809 546998.264653,4520183.137034 547025.367673,4520194.255784 547032.646993,4520300.108773 547108.177479,4520532.587398 547198.965036,4520605.73427 547274.678947,4520554.09131 547361.044864,4520561.134262 547378.79575,4520552.423628 547533.62837,4520476.86522 547572.692252,4520467.687998 547723.185463,4520543.219848 547785.729944,4520492.482892 547721.420056,4520325.143174 547743.567557,4520264.202718 547715.286566,4520208.726944 547722.613657,4520202.730863 547745.14613,4520123.788193 547889.151149,4520075.938204 547954.429713,4520064.958562 547950.773071,4520011.784304 547953.857437,4520013.070318 548060.856414,4519900.858246 548088.653306,4519764.134175 548183.399314,4519615.317158 548189.570367,4519583.436425 548158.367511,4519552.627853 548159.906837,4519420.705788 548170.593886,4519345.574486 548197.469272,4519275.321636 548250.404375,4519268.187509 548269.368782,4519170.018417 548213.535987,4519125.923754 548182.640639,4519059.64338 548202.328897,4518962.922768 548246.703429,4518838.541169 548294.454506,4518772.345875 548294.879955,4518765.825253 548248.368231,4518686.63843 548203.831847,4518605.317725 548101.698194,4518615.834183 548093.995416,4518616.363691 548101.238692,4518721.718029 548096.45208,4518806.429614 548068.731498,4518940.651123 548009.078572,4519071.576283 547984.104299,4519247.551121 548023.777846,4519396.112744 548003.043434,4519479.42444 547984.739984,4519489.171541 547935.032486,4519590.453373 547844.199423,4519709.043012 547749.438413,4519676.113224 547836.205427,4519498.385702 547868.333568,4519468.215666 547860.61533,4519461.256717 547882.557534,4519297.47346 547828.758461,4519215.790891 547835.677535,4519009.600774 547872.362409,4518927.689016 547851.151374,4518849.757374 547790.121972,4518804.52163 547780.204436,4518823.677725 547672.28046,4518762.692532 547670.17163,4518768.32965 547556.74066,4518796.100337 547508.740842,4518942.202495 547510.430841,4519033.959415 547459.641729,4519081.390255 547346.608352,4519153.073405 547258.369927,4519209.570232 547121.783728,4519296.354407 547103.828041,4519302.08279 547034.868229,4519275.866364 546891.752555,4519342.128945 546835.927384,4519294.537956 546802.39425,4519119.088108 546690.685698,4519139.250701 546648.580499,4519064.253142 546685.296406,4518877.481765 546710.706602,4518811.323629 546649.188934,4518742.653116 546825.294845,4518629.685066 546773.004934,4518540.906717 546733.434753,4518473.72325 546512.769331,4518526.840923 546450.424748,4518595.038797 546426.269074,4518603.190701 546495.079929,4518725.285722 546455.307082,4518820.393798 546282.400048,4518898.189958 546289.469598,4518755.898616 546157.018454,4518708.581289 546193.368771,4518652.655858 546179.205128,4518653.629577 546138.938375,4518554.537147 546140.053878,4518548.967805 546317.853717,4518453.359263 546492.537493,4518401.405856 546659.427987,4518330.51518 546770.800925,4518316.367984 546787.847046,4518299.217749 546842.224054,4518303.968154 546918.107405,4518229.347701 547113.509942,4518266.346051 547193.657558,4518202.91595 547334.570992,4518177.251693 547402.74101,4518246.463465 547494.883712,4518203.180488 547415.436054,4518124.258595 547394.989838,4518079.228454 547431.964005,4518028.753449 547420.892002,4517942.135699 547416.481839,4517932.45237 547568.237992,4517886.570584 547631.513988,4517808.323432 547687.223706,4517772.538681 547779.74221,4517734.722764 547792.241027,4517685.930558 547767.014808,4517656.707316 547774.264364,4517653.212997 547801.623582,4517535.494091 547787.621966,4517502.503338 547712.363801,4517522.655552 547645.970797,4517479.285327 547581.046875,4517457.284638 547458.97862,4517450.196132 547525.342223,4517378.727401 547513.188504,4517378.065148 547510.407354,4517290.378249 547648.022145,4517223.999231 547742.165781,4517162.605193 547860.200658,4517270.830519 547909.258491,4517505.127614 547970.099623,4517620.279671 548078.641887,4517555.898247 548060.659419,4517492.224914 548038.331591,4517434.342313 548004.099283,4517335.835832 547951.828609,4517174.658221 547886.623421,4517092.260699 547870.971744,4516973.502977 547952.453832,4516974.893798 547954.538129,4516965.263269 548083.991006,4517010.7906 548123.922235,4516976.090975 548120.937372,4516970.803814 548051.792021,4516849.232484 547966.377007,4516759.735259 547892.084533,4516790.305212 547889.484852,4516786.989171 547837.117493,4516800.574244 547726.456269,4516728.789899 547662.191875,4516672.790349 547655.287827,4516666.773654 547652.554321,4516666.961758 547629.832832,4516715.956763 547665.30617,4516834.351221 547603.902962,4516889.000763 547454.47844,4516819.880229 547274.715683,4516959.557877 547107.103212,4517094.406566 546925.111487,4517165.33396 546868.747307,4517286.046321 546835.588434,4517375.704624 546740.975207,4517426.646037 546670.55531,4517435.481085 546659.832233,4517510.116446 546653.734043,4517573.947973 546624.362955,4517607.923069 546565.311105,4517605.989987 546566.037632,4517612.930006 546519.847834,4517625.592204 546476.295162,4517607.614548 546376.533662,4517540.073248 546380.637544,4517485.365871 546373.397163,4517485.364451 546298.75575,4517334.708698 546366.241938,4517268.654563 546350.965663,4517155.361295 546349.009216,4517150.503091 546441.591461,4517102.69692 546589.659021,4517103.006537 546640.615697,4517152.431488 546685.478171,4517191.289994 546779.056705,4517168.880705 546796.591403,4517115.248271 546794.747261,4517088.412143 546802.043049,4517087.410975 546877.766667,4516881.484039 546726.149424,4516874.926976 546713.345595,4516779.440328 546794.000746,4516732.454607 546719.609905,4516687.136572 546663.865196,4516722.423798 546591.658918,4516748.856436 546582.102895,4516686.100463 546610.757546,4516641.690038 546598.452332,4516611.578151 546669.784362,4516534.276103 546606.112246,4516488.720267 546584.500162,4516395.835266 546443.341951,4516347.115784 546303.055315,4516323.80117 546326.403679,4516220.337094 546335.396013,4516222.215732 546384.258857,4516159.440039 546492.332336,4516099.086562 546588.282187,4516053.047957 546767.899505,4515974.796752 546784.034198,4515920.760889 546762.026312,4515889.31885 546749.521499,4515887.182032 546676.083578,4515784.876077 546527.24293,4515709.720955 546524.914068,4515684.915135 546517.940881,4515612.494752 546730.079408,4515602.413053 547058.783884,4515488.45518 547427.470681,4515373.24836 547522.074553,4515129.578825 547526.736779,4515124.764768 547814.108609,4515050.597218 547955.103902,4514987.984411 548099.718036,4514978.049134 548214.264233,4515010.123979 548324.445106,4515074.952592 548368.813716,4515144.80194 548454.021622,4515231.3185 548486.07172,4515158.215584 548487.699179,4515160.100972 548684.968256,4515062.665825 548744.537112,4515063.067006 548765.966907,4515156.960615 548767.776847,4515183.298912 548697.723608,4515221.065673 548710.835211,4515411.895257 548766.085553,4515447.543222 548826.268848,4515456.889365 548871.524987,4515539.659282 548835.726863,4515594.544936 548787.113718,4515657.301872 548693.010262,4515730.174103 548572.658491,4515804.351376 548523.7251,4515860.639517 548551.92625,4515984.025379 548601.638485,4516033.535235 548658.034244,4515993.211159 548657.754473,4515998.22347 548766.081749,4515954.33076 548790.886883,4516021.03032 548801.333717,4516062.25332 548868.115605,4516033.1992 548957.022502,4515959.185364 548998.080365,4516131.617173 549009.994045,4516148.773073 549064.012791,4516226.446583 549178.080509,4516251.561258 549280.487199,4516205.080147 549283.895863,4516158.411203 549231.969145,4516127.528315 549240.929895,4515923.699137 549376.426348,4515997.27159 549391.638566,4516015.199243 549397.075334,4516170.605283 549591.506941,4516117.300836 549592.7372,4516115.219579 549670.852951,4516140.308583 549670.847058,4516129.324091 549670.430843,4516026.997701 549726.880646,4516025.615391 549773.062997,4515916.591838 549892.035429,4515971.327633 549974.652629,4515965.650366 549974.506747,4515967.158612 550110.755478,4515971.775981 550108.932554,4515963.413374 550225.146962,4516014.343738 550304.036603,4516008.92237 550356.867166,4515934.892751 550358.345584,4515932.793901 550301.606172,4515877.277291 550218.174945,4515816.605211 550265.899444,4515740.928861 550312.577877,4515651.843753 550390.81288,4515540.6193 550452.026131,4515470.50688 550515.936399,4515439.653378 550531.518517,4515366.685942 550601.070207,4515323.462092 550724.793824,4515438.284306 550783.04966,4515543.126248 550949.860425,4515676.456703 551046.306737,4515852.067725 551140.026913,4515998.908196 551130.541477,4516107.904086 551081.115734,4516189.688179 551042.723335,4516226.777551 550930.000556,4516177.604872 550874.395872,4516425.576406 551094.929278,4516463.345234 551182.656576,4516490.269223 551224.02622,4516612.746145 551295.886769,4516601.317096 551392.096733,4516462.894498 551505.505023,4516382.205192 551528.576462,4516334.686149 551563.661436,4516280.34969 551568.984822,4516286.973616 551644.692265,4516289.759476 551667.893375,4516273.186617 551796.720436,4516258.841183 551906.952793,4516131.938668 551997.021975,4516000.429762 552197.410868,4515841.37 552418.44003,4515693.873877 552472.750625,4515623.239256 552554.208295,4515651.592415 552567.346481,4515635.711623 552657.890048,4515525.640859 552668.361559,4515485.478946 552653.501508,4515449.054431 552660.660566,4515446.066199 552665.148741,4515433.275761 552644.110155,4515266.965605 552587.321103,4515112.5976 552622.639106,4515050.757739 552688.481979,4515046.23403 552696.295521,4515063.670801 552757.406373,4515102.908431 552761.137129,4515238.954056 552880.503787,4515343.088345 553160.289511,4515376.786437 553246.682316,4515384.330824 553253.49963,4515290.997834 553328.801074,4515233.401061 553379.541287,4515203.453357 553418.711833,4515101.407194 553443.239463,4515073.261287 553526.90011,4515041.051433 553570.168149,4514998.63677 553682.419667,4514997.414343 553683.961469,4515116.134151 553744.460556,4515131.947741 553820.649118,4515087.27143 553874.034503,4514997.729653 553875.065778,4514918.275247 553842.537108,4514828.146495 553757.954176,4514846.938283 553643.18205,4514907.7463 553595.851409,4514891.027514 553574.406553,4514809.622341 553624.566846,4514751.25657 553728.683097,4514766.070957 553702.693115,4514653.025708 553828.639004,4514685.811915 553967.226405,4514682.780535 554084.127363,4514654.779067 554123.632467,4514652.06489 554108.820428,4514598.163424 554161.662749,4514506.163383 554157.284397,4514491.485949 554196.026642,4514455.873123 554214.723599,4514379.199822 554232.057157,4514308.112353 554181.695353,4514172.776805 554190.598711,4514069.816424 554054.983892,4514010.733748 554028.028705,4513907.240431 553916.979403,4513916.866121 553762.916463,4513914.468687 553759.394369,4513917.706119 553712.228926,4513892.487536 553667.504714,4513915.530822 553594.489877,4514006.420426 553586.715301,4514085.838183 553558.902588,4514160.642101 553492.028482,4514244.619616 553442.49517,4514293.954783 553380.396008,4514351.143685 553261.474987,4514253.468117 553131.992726,4514196.459705 553070.336743,4514260.108556 553102.989077,4514350.229992 553210.819696,4514382.763686 553247.803754,4514439.636092 553231.191634,4514486.709889 553208.807368,4514640.52487 553223.22211,4514690.459902 553162.862996,4514727.559106 553048.731619,4514806.795784 553046.64489,4514876.337755 552973.152468,4514960.271764 552898.152421,4514925.982195 552721.069143,4514944.63948 552553.316381,4514879.27694 552605.909562,4514807.262861 552661.105291,4514744.057204 552648.649173,4514659.038613 552638.445758,4514606.816199 552603.715059,4514582.741075 552601.359383,4514548.453096 552620.164977,4514549.657398 552700.774382,4514445.76304 552727.646284,4514429.936975 552786.729146,4514457.831624 552861.763383,4514492.618314 552931.544373,4514547.237997 553035.1344,4514612.515121 553079.768124,4514589.977208 553066.390343,4514491.541484 552967.131622,4514393.014758 552880.563236,4514286.625689 552940.534086,4514196.63149 552987.051302,4514101.569967 553038.720521,4514076.052668 553202.897793,4513954.935369 553213.787416,4513937.212019 553344.722909,4513831.858912 553394.699843,4513788.983724 553461.574844,4513705.006639 553584.617274,4513670.093651 553683.256041,4513663.318303 553770.535153,4513683.783943 553821.719256,4513660.297212 553887.26249,4513556.940584 553964.783424,4513531.645665 554035.275946,4513500.342222 554112.030575,4513502.060095 554112.447116,4513499.035605 554177.996165,4513452.095874 554272.208812,4513493.553414 554305.949826,4513594.084051 554363.482231,4513663.024329 554414.520693,4513731.910879 554419.948263,4513810.920985 554283.614319,4513846.746716 554246.930313,4513888.708731 554278.251203,4513959.449515 554308.04784,4514009.824897 554404.432633,4513970.252759 554503.548344,4513970.43324 554564.456376,4513992.210545 554673.774789,4514044.61208 554768.400676,4514077.553568 554763.296089,4514055.936809 554740.682264,4513961.133023 554771.648624,4513914.072264 554757.83494,4513845.624645 554913.139712,4513938.801885 555096.899319,4513955.634214 555055.005826,4513858.16171 555059.589827,4513855.849455 555052.447196,4513735.520109 555135.945419,4513762.735323 555168.310707,4513690.616216 555246.813491,4513734.150658 555254.668691,4513944.796299 555309.957609,4513977.44411 555309.056762,4513982.498166 555356.792859,4514130.493743 555340.134648,4514371.280374 555349.789364,4514520.893349 555276.485244,4514529.424308 555284.412195,4514570.317881 555319.351966,4514693.730277 555420.418057,4514818.590009 555499.350485,4514926.497807 555507.906285,4514938.391417 555591.297113,4514998.563074 555636.549902,4515081.325663 555669.602534,4515115.500285 555672.167732,4515118.319422 555824.273709,4515121.347786 555910.735507,4515115.407174 555916.325738,4515115.023096 556028.449342,4515113.809428 556133.669281,4515106.579251 556279.156439,4515109.563525 556276.979644,4515094.236458 556266.699636,4515024.548416 556208.972141,4515014.535413 556202.88866,4515000.475118 556158.019271,4514979.594345 556090.01588,4514950.81692 555979.401412,4514879.535073 556001.188082,4514795.162568 556000.126059,4514794.236562 556012.908808,4514791.361374 556028.871495,4514734.848184 556037.479796,4514740.247154 556194.491691,4514838.29674 556262.904281,4514873.036941 556319.638968,4514928.549293 556386.276688,4514937.450412 556409.498461,4514919.379873 556423.846594,4514908.408907 556446.119483,4514847.46802 556438.881521,4514742.124353 556491.429182,4514738.513735 556585.290216,4514758.524957 556629.568243,4514729.02276 556538.228878,4514649.427479 556482.381942,4514606.834598 556416.934286,4514518.969763 556344.903363,4514431.557435 556477.537294,4514390.992417 556536.55258,4514423.383005 556562.507364,4514470.526529 556642.274197,4514492.504722 556662.313615,4514522.580458 556665.723526,4514521.347668 556724.95211,4514638.595862 556860.059818,4514490.522699 556978.858298,4514297.639486 557023.404766,4514175.759072 557066.057601,4514104.431662 556994.706162,4514106.837722 557001.725304,4514096.370357 556846.570183,4514067.089572 556681.628018,4514178.769449 556387.325187,4514131.589809 556155.011757,4514040.210298 556169.886079,4513953.317275 556295.813828,4513893.244058 556373.255278,4513850.480484 556468.052868,4513891.396933 556526.123057,4513931.84067 556574.558555,4513926.516525 556659.504795,4513851.285828 556818.856444,4513870.792761 556867.807334,4513525.945853 556868.043625,4513523.932533 556809.7877,4513428.085044 556692.869109,4513235.918794 556467.500822,4513116.601678 556395.756165,4512944.296563 556494.281631,4512626.997178 556655.442376,4512400.254578 556743.237027,4512282.893064 556917.94663,4512117.127762 557056.66173,4512156.028302 557066.131699,4512144.8935 557120.924066,4512183.067035 557181.863975,4512205.342052 557250.984731,4512168.643572 557254.823363,4512166.383171 557251.286419,4512162.132789 557251.432362,4512160.625317 557255.320392,4512120.918126 557301.997967,4512031.842775 557186.259432,4511980.381189 557045.484297,4511955.101261 557062.605619,4511793.668832 557056.574571,4511793.084149 556994.751293,4511788.84258 556924.975763,4511734.224286 556920.915071,4511675.093195 556931.832114,4511666.854565 556921.348071,4511659.58682 556879.893843,4511435.776772 556728.486383,4511470.137173 556661.406149,4511454.773372 556623.538845,4511384.98302 556606.271258,4511315.275838 556505.60609,4511163.427361 556497.513856,4511151.002553 556189.371088,4510994.927804 555942.692678,4510825.145115 555880.381882,4510650.192666 555790.556756,4510588.960723 555780.727929,4510582.147244 555747.130105,4510656.345846 555766.514116,4510807.785511 555702.255733,4510933.514758 555570.364055,4511044.917113 555503.018015,4511183.839935 555510.395753,4511236.753472 555337.206686,4511215.693642 555327.005963,4511163.473409 555316.773009,4511161.680113 555264.095102,4511083.419326 555456.1056,4511022.307523 555489.986825,4510903.156094 555578.541023,4510878.603894 555593.127494,4510749.295385 555532.083147,4510674.604318 555393.901042,4510763.472259 555292.776649,4510835.817651 555095.038925,4510966.218964 554927.412065,4511077.079751 554969.39153,4511148.586498 554925.235669,4511178.078917 554669.715294,4511208.603812 554574.32569,4511182.70163 554536.043973,4511192.319392 554336.208553,4511143.133453 554162.144337,4511091.179745 554095.201557,4511012.398871 554145.75747,4510830.692428 554197.530178,4510690.340309 554269.29968,4510344.918869 554395.007332,4510334.291832 554370.994094,4510522.662926 554436.999636,4510651.433551 554334.167407,4510744.365834 554335.368252,4510752.770838 554265.255517,4510800.520842 554304.091351,4510982.580267 554367.76196,4511044.610037 554521.768185,4511073.477752 554626.083949,4511060.324601 554648.506115,4511068.770347 554681.802608,4510930.187094 554711.942088,4510707.445897 554645.902857,4510410.928266 554584.006575,4510131.100444 554566.348936,4509753.875963 554669.076932,4509603.038859 554764.290327,4509493.656343 554941.581949,4509519.929648 554960.108677,4509678.918692 554908.392856,4509945.575838 554796.114052,4510028.171324 554801.533796,4510147.121224 554802.810056,4510147.533036 554754.545286,4510231.725729 554805.046868,4510294.660036 554877.347695,4510289.696304 554982.569032,4510282.473242 555145.558166,4510251.313554 555248.25494,4510303.675046 555301.689886,4510312.987295 555361.743158,4510322.34419 555394.390833,4510412.464312 555449.493487,4510486.065585 555572.055008,4510393.278302 555478.324388,4510287.880526 555397.904717,4510174.579288 555362.827466,4510103.597277 555470.034328,4510045.314152 555425.445705,4509750.322095 555554.154019,4509661.607374 555641.703776,4509589.69651 555696.776659,4509526.505493 555744.02576,4509443.881815 555745.220828,4509364.918301 555868.704766,4509336.472394 555849.292903,4509291.874547 555935.599141,4509092.741436 556031.017237,4509062.72797 555974.096362,4508986.256038 556010.455151,4508895.893078 556164.461534,4508833.90043 556066.100655,4508748.290904 555980.907637,4508661.776836 555831.603514,4508506.772846 555830.249126,4508508.863155 555753.365334,4508617.982599 555865.338086,4508709.148999 555798.779501,4508799.588069 555856.431189,4508868.521145 555848.651425,4508947.936355 555831.182232,4509002.555122 555839.222996,4509001.503639 555815.491152,4509043.072451 555767.445124,4509112.270894 555709.677006,4509136.205603 555670.890715,4509052.99699 555667.070946,4508997.343386 555657.61763,4508995.995426 555624.287485,4508864.983492 555604.673363,4508721.047841 555602.290775,4508722.709344 555474.093765,4508818.876345 555395.16564,4508878.211794 555220.891025,4509126.818172 555160.670826,4509247.77673 555157.360389,4509245.008572 555158.099019,4509279.40619 555006.913314,4509289.783009 554797.515774,4509317.635541 554795.775649,4509215.907111 554731.841235,4509166.375495 554714.740862,4509062.705601 554624.070171,4509005.523426 554516.354237,4508990.949001 554322.815199,4509026.198982 554226.56656,4508996.858571 554238.73617,4508959.577846 554346.501109,4508905.750702 554424.056517,4508880.956989 554635.428115,4508896.404995 554637.52803,4508894.264333 554750.64433,4508912.961048 554756.723755,4508896.068667 554829.821532,4508879.06993 554885.505049,4508864.763496 554910.057594,4508764.22663 555030.237312,4508667.610766 555238.304386,4508620.380409 555410.128122,4508621.569148 555475.844229,4508617.059246 555584.384883,4508583.648749 555578.370858,4508570.582099 555684.645585,4508395.041769 555691.010489,4508384.120622 555554.703964,4508327.574359 555580.277616,4508261.915482 555594.233018,4508150.623544 555501.547881,4508027.678455 555481.150508,4507923.236934 555475.285452,4507837.768573 555372.148077,4507778.945309 555301.01034,4507704.445562 555247.526399,4507503.423121 555185.942539,4507375.347308 555102.390552,4507216.327235 555032.859708,4507068.826165 554967.295314,4506980.96285 554928.635193,4506897.744153 554893.19177,4506888.693016 554901.751421,4506860.646454 554907.456755,4506838.288349 554906.214643,4506838.373606 554806.286319,4506837.240849 554628.883147,4506971.229762 554637.119943,4507058.533712 554609.405238,4507165.777701 554536.17945,4507279.139962 554343.221345,4507379.249621 554173.216072,4507488.268877 554124.101611,4507494.634281 554055.857482,4507558.728631 553956.180251,4507644.949911 553919.093035,4507779.298769 554117.290072,4507779.179213 554139.445094,4507718.247351 554179.669329,4507629.615151 554301.348579,4507574.834974 554364.672116,4507519.066715 554558.816892,4507578.137635 554563.988355,4507575.286601 554628.452709,4507625.28218 554726.612565,4507611.557116 554865.349673,4507516.166082 554994.972965,4507551.705532 554989.60409,4507593.512027 554956.884797,4507695.10781 554989.529025,4507785.22999 555030.440617,4507901.244574 555131.455077,4507856.369809 555131.859319,4507876.811123 555176.032742,4507987.609597 555196.632015,4508165.926345 555089.04572,4508162.325344 555014.053639,4508082.599194 554950.198126,4508030.564934 554850.671889,4508044.384242 554749.274222,4508194.628388 554642.374276,4508211.949465 554550.764005,4508224.726161 554491.782823,4508136.411294 554383.991148,4508104.366896 554359.629421,4508231.352145 554502.1492,4508287.473604 554491.730663,4508377.555449 554413.434848,4508362.458863 554119.759593,4508337.17991 553947.541142,4508266.62045 553720.797123,4508354.573235 553653.981049,4508250.319831 553642.778247,4508252.586404 553526.430773,4508208.647182 553564.993383,4508194.018367 553566.700769,4508173.431374 553459.89478,4508073.918829 553396.842455,4508000.859539 553372.272162,4507910.182366 553224.276507,4507965.271299 553138.940038,4507942.169237 553178.940238,4507883.008221 553296.697333,4507765.58939 553394.268143,4507665.032815 553395.566524,4507663.945662 553411.677884,4507651.357146 553438.689936,4507617.550524 553440.034692,4507593.494164 553442.180644,4507593.846013 553471.850271,4507542.383353 553564.024405,4507550.537527 553591.391498,4507529.188818 553644.445307,4507542.023861 553767.878357,4507570.998884 553898.261437,4507532.097171 553825.958464,4507429.718053 553752.998195,4507346.853905 553814.480648,4507264.252279 553803.859664,4507173.117303 553835.622398,4507077.576457 553900.649625,4506990.238108 553873.174702,4506917.234274 553789.788628,4506857.052866 553628.402379,4506815.203599 553508.558247,4506753.0296 553496.890349,4506750.335221 553454.6737,4506669.855341 553397.592232,4506612.861219 553282.844937,4506612.245965 553310.616462,4506826.021699 553209.840595,4506854.403682 553158.047365,4506910.878843 553197.868388,4506960.069958 553200.951969,4506961.356113 553175.474998,4507073.940608 553197.034753,4507098.921933 553318.715944,4507044.142496 553441.441945,4507002.771176 553483.097222,4506929.518 553583.677525,4506920.120988 553590.40875,4507000.039948 553542.364805,4507054.759927 553428.507268,4507154.435269 553154.615387,4507286.559495 553058.903832,4507299.617114 553024.099786,4507467.261113 553067.662315,4507523.684834 553170.068311,4507477.216315 553252.417116,4507616.852039 553183.887019,4507700.437548 553039.941947,4507721.79744 552998.044728,4507704.202418 552896.645066,4507727.136461 552837.511058,4507731.194148 552626.709386,4507738.668727 552554.282363,4507743.638358 552407.869044,4507727.223045 552241.832311,4507672.212885 552236.626461,4507674.567167 552250.564882,4507886.798861 552414.338384,4508034.328608 552418.995958,4508043.993879 552542.694106,4508054.97762 552616.239315,4508049.931112 552621.213559,4508035.110962 552923.692608,4508112.211221 552952.860057,4508100.723424 553059.657166,4508178.270059 553123.789417,4508214.309411 553118.690853,4508305.525559 553020.16806,4508372.197996 552898.043479,4508420.520209 552851.032865,4508410.266088 552847.250568,4508258.748954 552842.749052,4508193.154682 552725.367307,4508214.190368 552705.984188,4508413.729131 552772.348177,4508515.019439 552736.306318,4508662.779387 552761.447298,4508739.938279 552771.748915,4508742.725986 552801.749342,4508750.652563 552899.94591,4508737.423337 553005.169886,4508730.201666 553030.637404,4508728.454042 553130.022112,4508721.633569 553196.242134,4508702.610027 553220.713283,4508695.438742 553363.473658,4508627.727395 553367.843505,4508625.929401 553453.314268,4508620.063681 553490.527408,4508593.046291 553493.533151,4508596.834242 553636.943876,4508569.518308 553704.028131,4508615.838868 553606.034875,4508728.407142 553786.731524,4508762.437294 553896.536231,4508721.950471 553913.587126,4508741.249943 554100.158348,4508780.867975 554093.616439,4508900.140435 553948.338393,4508916.601479 553898.373935,4508966.961219 553820.681053,4508913.380511 553696.630928,4508844.50903 553610.451913,4508907.338933 553510.044489,4508873.789781 553504.154624,4508860.714118 553456.822364,4508863.962403 553342.922523,4508813.864948 553307.870225,4508874.184991 553284.755922,4508859.295988 553272.712875,4508876.598209 553101.008679,4508875.40088 553075.2659,4508867.682097 553073.752973,4508869.283349 553071.765297,4508869.419851 552823.478151,4508874.477572 552781.330144,4509029.646191 552785.423199,4509089.27769 552617.781539,4509483.721963 552574.939798,4509576.031838 552454.856214,4509566.799269 552350.232974,4509580.970176 552225.424804,4509688.392314 552230.404587,4509760.944508 552251.172326,4509854.380026 552210.186427,4509904.62467 552192.36723,4509886.875618 552162.7738,4509930.346163 552071.434673,4509850.741122 551929.626804,4509708.695818 551894.453722,4509678.158328 551867.160827,4509640.589332 551837.522665,4509636.132527 551837.445139,4509638.634201 551804.125562,4509611.4643 551736.030801,4509583.186229 551800.165566,4509842.900019 551844.697181,4510011.593471 551913.096849,4510429.782921 551964.270093,4510789.740634 552000.837654,4511093.284816 551996.284574,4511090.601854 551819.057915,4511263.536446 551660.395479,4511499.60329 551592.484114,4511455.836066 551592.255218,4511448.862149 551516.969781,4511454.031027 551497.488676,4511363.00248 551468.506178,4511313.567251 551464.267782,4511306.368832 551457.157554,4511317.342026 551396.71922,4511336.9694 551292.586154,4511456.456534 551130.648151,4511599.384946 551069.466097,4511669.990273 551039.261677,4511780.907072 551006.04061,4511795.170829 550859.97507,4511876.597499 550773.12042,4511875.072744 550579.452688,4511941.294844 550440.268296,4512030.238044 550226.243171,4512181.738572 550218.462927,4512184.769602 550216.294734,4512185.916739 550156.202607,4512221.498692 549914.822632,4512072.809954 549876.212182,4512014.048642 549866.158245,4512011.243914 549649.523803,4511884.820411 549623.561446,4511775.76139 549508.905235,4511836.558495 549418.654191,4511869.217571 549326.594951,4511875.538784 549227.029317,4511868.894594 549125.813386,4511836.400371 549065.187677,4511820.591363 548977.755952,4511892.501826 549060.263854,4511939.761103 549135.299653,4511974.55256 549183.405036,4512000.707709 549182.46841,4512096.136779 549126.422286,4512137.931132 549024.640818,4512220.813044 549026.143809,4512222.706766 548863.081023,4512243.889763 548829.37626,4512123.877112 548683.034122,4512201.331074 548384.873647,4512198.337863 548266.513634,4512217.949459 548170.265946,4512301.45023 548038.474781,4512294.522511 547931.362479,4512334.332241 547728.975223,4512368.201728 547685.406031,4512311.77675 547710.651004,4512197.700885 547759.255376,4512132.949042 547741.826692,4512037.281356 547745.404582,4511891.239914 547619.987187,4511728.091597 547516.486086,4511596.891437 547458.217548,4511600.892269 547342.431372,4511548.925585 547232.252573,4511484.09153 547097.632969,4511446.898969 547026.69018,4511471.74136 546855.018846,4511569.408316 546747.389638,4511570.80624 546733.275137,4511579.764691 546605.442652,4511494.17199 546484.653149,4511492.978426 546403.49527,4511534.500457 546332.073903,4511552.385854 546208.008103,4511574.385636 546184.636557,4511523.06331 546179.328103,4511523.926818 546155.576579,4511456.153603 546214.082708,4511379.237536 546349.99963,4511338.949153 546517.950999,4511374.353208 546578.011477,4511383.711405 546605.066464,4511295.972852 546693.956279,4511243.434608 546771.854659,4511212.621967 546763.140549,4511200.238413 546798.966092,4511123.881769 546904.041752,4511096.196573 546979.733567,4511105.979114 547186.259946,4510981.454476 547205.973135,4510988.589161 547313.446817,4510921.294805 547400.744269,4510943.762464 547465.250352,4510897.891833 547459.65555,4510816.390321 547373.246507,4510761.407524 547270.783133,4510794.405424 547181.89361,4510846.942676 547060.648672,4510908.192248 546997.177097,4510945.503486 546892.388751,4510959.188432 546839.340033,4510942.857761 546804.659252,4510932.256587 546722.320558,4510983.845096 546677.442837,4511099.269991 546608.307789,4511150.451935 546497.392504,4511178.538072 546492.93356,4511160.869237 546387.775467,4511109.170062 546260.40816,4511124.904556 546177.278397,4511224.980882 545966.368716,4511232.96986 545949.003636,4511127.308882 545937.753866,4511127.082579 545868.115038,4511045.482641 546004.07484,4510925.801157 545999.141962,4510810.299758 546135.637612,4510702.066892 546213.854092,4510575.864362 546075.231125,4510529.45788 545867.816918,4510646.553638 545675.272067,4510751.145253 545560.796219,4510801.944355 545440.791234,4510984.942347 545300.381173,4510961.62634 545274.837635,4510925.931149 545244.865724,4510938.474435 545145.463887,4510832.452903 545099.640186,4510712.267567 545101.886186,4510708.617876 545084.716182,4510705.802266 545051.58746,4510608.712288 545029.26181,4510570.798661 545009.231016,4510511.756623 544892.033847,4510468.371526 544861.714875,4510535.863326 544883.153452,4510560.855833 544901.747551,4510638.970881 544955.517605,4510747.62632 544954.083873,4510919.489532 545042.736438,4510959.840323 545126.134554,4511020.024368 545077.959109,4511089.241853 545136.506377,4511171.105003 545087.643957,4511244.862677 545041.971944,4511281.452694 544962.470474,4511268.935 544929.636876,4511225.252248 544926.707835,4511097.128763 544813.252137,4511035.512089 544825.21556,4511111.586069 544790.456747,4511225.319767 544835.259278,4511381.526499 544833.708692,4511422.577431 544830.276444,4511521.677682 544771.464973,4511670.517869 544795.461799,4511849.124306 544849.031196,4511934.824948 544871.922663,4512044.600607 544730.796412,4511990.877649 544729.982595,4511989.934738 544640.265168,4511897.728582 544529.920394,4511801.446366 544460.338989,4511820.703724 544435.549627,4511923.268846 544376.778461,4512038.15363 544271.442625,4512023.915268 544186.67965,4512136.5904 544039.393072,4512218.605605 544124.602242,4512305.129589 544188.724987,4512373.627897 544241.576901,4512468.864821 544295.941214,4512491.596247 544383.501947,4512419.672763 544342.535553,4512235.739745 544352.033926,4512234.088812 544401.678312,4512166.267605 544505.075732,4512143.189797 544528.874215,4512187.992486 544567.638694,4512256.234311 544641.258659,4512297.61574 544722.781381,4512259.56229 544768.482072,4512283.387287 544738.741751,4512353.836236 544725.703052,4512462.085248 544802.232772,4512587.65216 544905.772854,4512603.011625 544911.67325,4512541.689366 544971.309108,4512512.129085 544999.169966,4512479.757522 544965.37908,4512325.791088 545020.999918,4512285.022642 545129.396547,4512347.484103 545206.313521,4512525.95104 545169.384742,4512609.875229 545104.310324,4512720.19938 545167.06768,4512768.817657 545223.808062,4512824.339637 545260.669701,4512881.227116 545264.731378,4512940.366615 545240.76851,4512974.96757 545152.321974,4513033.969379 545137.072245,4513100.926563 545192.724806,4513126.064774 545225.726062,4513141.274253 545287.56219,4513176.473972 545438.765772,4513166.089927 545531.751542,4513173.185059 545598.806983,4513188.053241 545673.389314,4513188.923028 545764.10158,4513190.183047 545852.547365,4513131.181365 545840.119295,4513101.077392 545724.065793,4513087.077791 545634.525614,4513033.808526 545655.585097,4512860.598458 545707.045582,4512744.718364 545656.127161,4512675.814488 545483.560415,4512562.336981 545484.327906,4512560.785909 545528.099283,4512472.897132 545435.62256,4512396.859775 545415.59582,4512348.80261 545472.441977,4512320.433089 545381.033403,4512210.868598 545326.327035,4512132.237917 545251.695998,4511936.137816 545129.462292,4511872.129679 545076.803095,4511752.413609 545150.261103,4511691.446683 545166.639389,4511691.820156 545267.763311,4511830.178159 545424.983991,4511716.524287 545505.297957,4511775.42213 545601.728485,4511878.150851 545644.851317,4511968.062598 545651.397838,4511981.593304 545612.145031,4512071.668507 545663.704861,4512113.566056 545700.246666,4512163.984077 545806.844761,4512176.637271 545918.539071,4512168.967709 546149.522436,4512166.089711 546130.541876,4512273.246998 546108.707,4512340.654823 546163.514005,4512369.845867 546214.112219,4512432.280974 546255.512169,4512555.264197 546346.2098,4512529.063237 546296.007436,4512374.229477 546261.068339,4512250.802538 546256.56404,4512185.203006 546286.314107,4512136.724098 546427.076525,4512167.003906 546491.163562,4512235.002735 546582.954815,4512321.072094 546634.350255,4512396.933075 546632.428626,4512463.472498 546611.037936,4512537.34099 546576.511474,4512612.610908 546514.371337,4512669.305852 546400.154573,4512736.567311 546330.576234,4512781.290325 546315.447516,4512817.28093 546302.157167,4512849.150679 546234.346747,4512974.141681 546220.025701,4513000.090556 546352.24873,4513096.863595 546343.584491,4513163.368039 546315.643525,4513238.186154 546270.768864,4513353.613002 546221.562216,4513502.292101 546259.311263,4513572.099811 546364.100274,4513558.411594 546426.649942,4513507.679172 546517.825054,4513488.435647 546547.460043,4513481.906442 546602.381713,4513469.146404 546724.991267,4513427.771099 546767.022942,4513365.466482 546755.489091,4513293.858949 546733.71845,4513169.528652 546753.743442,4513075.781698 546797.730663,4512947.434173 546849.633998,4512838.01671 546851.145521,4512810.950349 546855.568105,4512731.755947 546859.72791,4512599.653341 546879.151586,4512498.957864 546988.922094,4512557.827722 546980.701008,4512630.791607 546967.862627,4512731.035032 547049.451331,4512764.877237 547051.517162,4512762.238725 547122.030632,4512667.521827 547218.300655,4512627.95612 547283.545904,4512616.485623 547443.854219,4512546.060213 547512.544262,4512488.416807 547606.414687,4512508.433663 547593.13254,4512602.215917 547564.713944,4512670.075474 547475.55517,4512815.004547 547582.324201,4512926.5062 547557.511345,4512999.610739 547548.420318,4513001.733058 547506.779364,4513076.991292 547439.898225,4513160.973819 547333.031459,4513240.71245 547208.165864,4513263.768433 547163.681138,4513457.557934 547228.553086,4513693.268829 547209.914765,4513779.930243 547104.185771,4513741.755379 547040.872521,4513754.092861 547112.061256,4514016.332219 547076.411239,4514055.229922 547080.947097,4514090.369253 547024.270217,4514226.578607 546950.771282,4514310.518255 546881.463699,4514262.852058 546882.732991,4514226.814199 546880.971588,4514228.433231 546807.178405,4514133.640535 546783.930146,4514127.74781 546645.019491,4514186.222277 546513.717666,4514102.868564 546463.686208,4514046.887416 546397.518607,4514044.9412 546337.013971,4514029.124638 546282.970288,4514012.864439 546283.228302,4514009.351451 546276.205189,4514014.328126 546184.036422,4513968.230939 546184.490704,4513951.22293 546140.964817,4513871.82614 546084.945077,4513857.698918 545813.341814,4513788.973855 545519.150936,4513660.384126 545520.82243,4513750.146242 545536.840313,4513797.979312 545562.382805,4513873.619062 545656.738446,4514022.424712 545650.344693,4514154.683676 545489.396049,4514081.354181 545438.336106,4514024.943571 545377.462155,4513994.671869 545349.045299,4513931.712459 545206.818805,4513981.926121 545204.55364,4513992.567334 545198.034349,4513988.521293 545120.948934,4514020.28012 545034.755788,4514112.083665 544962.196854,4514227.916545 544919.92928,4514226.825433 544777.665295,4514427.338037 544695.175121,4514514.893173 544653.009842,4514515.29269 544656.964874,4514496.546183 544651.963679,4514501.883317 544558.089294,4514481.867348 544462.835705,4514459.949531 544453.796299,4514475.550141 544317.704807,4514487.894516 544296.501285,4514728.027651 544246.227534,4514819.362465 544286.447648,4514836.073026 544220.400366,4514874.064966 544288.832059,4514970.727281 544384.038606,4515010.124432 544421.948264,4515080.421228 544558.174392,4515044.598631 544636.147749,4515025.75992 544712.999131,4515086.89057 544711.364709,4515159.404412 544607.482493,4515204.489875 544602.400509,4515206.835945 544607.795476,4515285.358519 544670.110757,4515327.514336 544702.323742,4515411.184594 544651.009794,4515434.68321 544565.529803,4515440.556008 544460.862695,4515454.238331 544439.777368,4515547.06313 544351.875837,4515588.554967 544292.839396,4515639.547549 544220.942915,4515616.025842 544114.785335,4515609.837239 544002.201394,4515604.59005 543903.550482,4515611.368315 543793.538979,4515645.390703 543695.18861,4515751.015524 543610.805168,4515869.162486 543576.724792,4515950.897683 543499.194595,4515976.197641 543365.009903,4515945.470854 543314.975185,4515889.488328 543341.903122,4515801.75344 543381.681033,4515706.643675 543325.825333,4515664.043627 543221.190609,4515678.2236 543163.910757,4515807.49107 543122.447091,4515876.25181 543048.535999,4515954.233019 542969.639453,4515959.654006 542847.612049,4515915.108687 542761.349094,4515875.097175 542755.894868,4515877.469519 542722.803058,4515842.792319 542590.337033,4515838.910543 542401.551061,4515878.346452 542321.020223,4515956.28395 542127.874091,4516029.474977 542260.93286,4516139.175012 542302.387502,4516244.683274 542274.341034,4516267.083447 542135.991659,4516417.403206 542138.551398,4516419.224568 542106.805214,4516441.379819 542226.651298,4516535.009691 542222.585269,4516535.788632 542192.978679,4516587.257542 542193.783169,4516591.696605 542353.860109,4516708.526925 542461.352126,4516734.096622 542621.434761,4516756.551905 542678.76616,4516726.147125 542804.469974,4516631.623097 542880.63474,4516586.442478 542901.454636,4516506.117021 542875.949889,4516421.9838 542814.873792,4516399.715761 542579.5145,4516435.362977 542565.245449,4516323.992657 542599.360705,4516242.75318 542698.012985,4516235.974479 542803.569861,4516235.212423 542910.173332,4516247.860999 543004.937882,4516280.796894 543079.059723,4516302.168225 543159.322403,4516316.626514 543219.351751,4516325.484336 543322.511015,4516384.307882 543407.99262,4516378.433828 543473.7189,4516373.917617 543557.601313,4516441.055767 543620.362107,4516489.672533 543725.120527,4516475.482893 543782.007381,4516438.618211 543854.318228,4516433.649484 543915.713028,4516462.386029 543910.351533,4516575.103396 543960.8642,4516638.043252 544027.921972,4516652.908694 544039.049618,4516718.554933 543979.19799,4516808.552439 543939.688158,4516811.267473 543832.998836,4516699.259215 543781.622973,4516712.776562 543778.644383,4516698.500646 543598.505138,4516658.450026 543487.743969,4516641.593843 543339.964945,4516663.733391 543336.806159,4516755.827306 543303.550796,4516795.063541 543179.116563,4516642.828756 543039.935536,4516753.758109 542960.807852,4516897.511583 542961.826704,4516901.435759 542891.88413,4516942.693891 542837.694735,4517018.821759 542792.254902,4517127.80352 542845.113537,4517223.039207 542948.361836,4517381.223732 543025.139864,4517384.934937 543039.415621,4517401.929868 543131.758929,4517405.071015 543137.064353,4517398.713842 543192.193819,4517329.013426 543165.23791,4517091.185675 543217.823522,4517037.139119 543331.994959,4517092.708124 543429.815847,4517282.722229 543577.802518,4517394.388432 543658.765616,4517471.713049 543634.974314,4517548.747022 543516.38124,4517566.884561 543421.686367,4517600.357355 543405.2937,4517697.854856 543448.477254,4517781.271222 543591.2688,4517831.876332 543911.418046,4517874.784168 544089.132565,4517859.572859 544199.20103,4517850.010287 544477.04114,4518005.677503 544482.722611,4517984.814856 544519.907879,4517842.946342 544641.192834,4517773.19328 544596.467527,4517616.483483 544727.172728,4517442.223005 544866.457849,4517509.04674 544933.967929,4517401.546542 545088.572273,4517308.032986 545158.652979,4517254.28268 545209.411231,4517311.711547 545256.74745,4517328.43107 545383.33456,4517246.830489 545475.717994,4517246.972345 545589.825513,4517272.584481 545659.136178,4517320.248819 545675.92677,4517372.022456 545679.725775,4517523.554763 545670.176546,4517577.138794 545586.72051,4517708.702994 545554.633317,4517815.765403 545540.820861,4517818.212936 545477.194293,4517940.425994 545425.117888,4517950.995804 545383.797799,4517927.372091 545372.225815,4517855.266648 545378.187697,4517749.50057 545383.143404,4517661.778822 545253.463359,4517725.617223 545254.326877,4517734.545862 545218.588154,4517734.006354 545116.052612,4517780.500282 544941.100385,4517924.845879 544974.07942,4518021.44502 545052.530319,4518009.561662 545117.012663,4517985.156556 545157.764664,4518002.328285 545165.4509,4518114.147505 545137.604089,4518188.462969 545137.339301,4518280.856359 545140.960334,4518333.535613 545142.068659,4518396.873409 545119.552851,4518408.906761 545111.209567,4518396.49808 545085.530914,4518489.638883 545051.808295,4518478.47542 544963.416317,4518345.739698 544880.456022,4518292.022779 544835.992726,4518222.677521 544839.844632,4518182.466867 544842.462985,4518178.79153 544820.245398,4518178.820469 544810.395802,4518178.998176 544764.99598,4518293.967922 544786.072379,4518475.271916 544869.492072,4518662.776315 544673.356704,4518798.594334 544645.966769,4518901.341181 544766.990329,4518949.444512 544944.236683,4518929.270088 544991.387189,4519055.852943 544990.281705,4519197.737111 544995.397396,4519195.887062 545043.302782,4519219.057869 545074.028149,4519282.85641 545081.272242,4519388.215043 545132.765617,4519463.56747 545182.357397,4519513.086228 545276.677466,4519539.556447 545362.634799,4519540.636696 545449.925508,4519561.098958 545486.344407,4519586.058032 545508.161526,4519576.568374 545541.366079,4519649.183422 545267.906923,4519707.931708 545136.457689,4519716.970251 545061.82013,4519622.736445 545055.381406,4519621.681547 544973.01288,4519852.540085 544840.030697,4519770.307545 544797.745797,4519935.99522 544715.117276,4519930.691469 544629.374228,4520028.962887 544549.771964,4520120.320691 544533.501378,4520266.744147 544501.896997,4520480.632657 544643.679653,4520523.811086 544712.477665,4520578.50041 544547.951179,4520640.246358 544529.884073,4520687.926761 544464.158868,4520692.446681 544415.715974,4520563.455827 544388.455973,4520517.394864 544328.454612,4520498.052517 544305.112153,4520507.147676 544257.26495,4520468.494372 544190.526446,4520460.10109 544125.219972,4520447.115547 544091.492943,4520584.752949 544078.696101,4520605.606471 544156.756752,4520640.683891 544219.520287,4520689.296928 544291.830814,4520684.323962 544377.493395,4520777.300585 544385.627883,4520895.581381 544384.47774,4520975.054312 544450.988437,4521076.337578 544417.357764,4521164.535287 544331.171373,4521256.347455 544297.926153,4521252.143006 544224.031422,4521137.885283 544148.576324,4521097.135694 544038.122317,4521104.732098 544124.511595,4520916.035625 544092.662757,4520826.848417 543969.610096,4520847.295176 543790.111678,4520940.031721 543782.215634,4520939.576072 543659.854646,4520926.519919 543500.54337,4520927.98842 543499.425193,4520928.065285 543427.66721,4520951.974988 543393.966531,4520999.233028 543296.819215,4521047.85856 543283.112969,4521097.236489 543368.195552,4521276.139597 543503.088478,4521220.923421 543521.104872,4521259.630886 543518.958552,4521259.279263 543530.914578,4521382.292289 543541.370082,4521488.93052 543671.193687,4521552.404611 543794.910446,4521623.289366 543939.395735,4521705.727977 544048.131021,4521751.178364 544142.931314,4521784.604567 544182.885492,4521788.347788 544230.267142,4521613.318515 544309.162651,4521607.891692 544422.977698,4521725.395337 544266.293448,4521848.522262 544204.375308,4522004.577954 544287.817427,4522065.249109 544366.452663,4522152.216188 544406.945301,4522261.780676 544334.803826,4522378.09404 544356.830346,4522397.051268 544393.560784,4522486.401295 544406.066302,4522488.536893 544403.143884,4522493.232019 544426.39859,4522544.561511 544427.506011,4522656.834678 544455.842673,4522780.217426 544488.952469,4522876.807089 544496.200817,4522982.166351 544507.074365,4523140.205625 544564.875152,4523180.169588 544550.619954,4523216.103902 544550.291891,4523416.358145 544518.93541,4523416.018862 544498.119467,4523490.852284 544411.67715,4523675.061486 544237.807602,4523931.199297 544044.217689,4524189.193881 543959.847023,4524307.350189 543873.921682,4524306.771497 543809.954554,4524324.156114 543809.137296,4524317.721015 543783.753385,4524313.476079 543699.076003,4524358.251057 543637.420113,4524421.914977 543527.669241,4524363.554941 543480.995523,4524452.653025 543460.982531,4524546.407628 543461.648257,4524652.22208 543419.268666,4524707.56895 543286.668983,4524796.089315 543171.887583,4524856.918735 543037.474758,4524919.099316 542979.697456,4524972.011236 542971.08664,4524966.61181 542801.416524,4525040.706612 542727.104083,4524920.485745 542844.742053,4524873.94067 542885.080428,4524805.251144 543149.191388,4524692.200182 543305.737814,4524478.195897 543236.227709,4524340.667564 543256.355753,4524195.971879 543432.20176,4524081.007535 543526.865662,4523983.614114 543505.246363,4523821.818516 543520.576229,4523788.806115 543460.389938,4523710.55694 543449.994394,4523559.473172 543340.518252,4523525.061051 543318.296554,4523628.45541 543253.311259,4523825.171943 543075.290688,4523950.272617 542984.002002,4524134.818502 542922.952196,4524067.613929 542784.710807,4524114.077205 542657.016043,4523930.117772 542549.896898,4523791.1809 542610.133065,4523694.158381 542685.476448,4523686.977121 542738.272859,4523632.411616 542684.831032,4523584.157041 542700.189027,4523431.799727 542763.399809,4523303.613598 542841.266663,4523257.809574 542865.146416,4523185.759564 542992.587977,4523210.946603 543044.255702,4523194.409459 543060.195731,4523179.331236 543065.865173,4523196.417997 543119.138644,4523183.264995 543166.453065,4523141.560695 543281.845241,4523098.668699 543289.039186,4523096.176313 543339.06756,4523170.131578 543305.548253,4523216.379347 543304.296896,4523219.961031 543512.839095,4523224.588229 543544.430677,4523202.940205 543531.277256,4522919.22346 543530.991247,4522913.250846 543548.317618,4522911.06041 543522.707258,4522850.904469 543523.226923,4522666.114178 543632.206381,4522591.706226 543563.577762,4522485.075428 543501.70133,4522449.385103 543481.456824,4522407.335485 543472.660185,4522411.934923 543389.695325,4522358.220946 543274.14428,4522157.446515 543270.566393,4522161.687346 543198.130646,4522166.670026 543125.600883,4522072.291144 542918.733683,4521848.834676 542907.981713,4521794.147746 542936.422288,4521810.167988 542928.3935,4521738.815192 542899.29747,4521627.965648 542865.706731,4521498.949731 542730.322446,4521356.461963 542725.32762,4521352.810417 542598.207527,4521259.688231 542496.63741,4521127.856535 542388.038613,4520990.017018 542285.824634,4520944.115753 542231.610712,4521020.74798 542243.500355,4521118.301353 542261.007243,4521115.099718 542343.358279,4521229.777878 542339.211131,4521321.942062 542328.706197,4521341.639509 542279.189547,4521433.928109 542246.089742,4521532.578111 542244.654501,4521531.677823 542234.910917,4521548.826438 542178.503652,4521592.653792 542061.490006,4521620.175786 541937.397139,4521633.704466 542025.69122,4521729.498192 542132.530477,4521764.593941 542248.481757,4521746.132048 542363.648726,4521691.772186 542408.779071,4521698.155436 542415.475613,4521685.710632 542491.314692,4521722.938507 542551.886041,4521738.746295 542662.400454,4521810.040517 542738.904903,4521864.200372 542776.968813,4521878.559472 542791.917287,4522017.845911 542797.182242,4522019.980799 542937.847909,4522077.715807 542918.175678,4522216.387326 542843.302722,4522338.383395 542963.631375,4522443.455902 542965.183226,4522562.192381 543071.083219,4522660.766801 543104.193389,4522757.35839 543124.199083,4522870.330823 543105.745766,4522876.094345 543039.314633,4522966.550785 542991.181931,4523036.274355 542944.287045,4523026.018367 542852.923946,4522946.417678 542768.146238,4522866.363365 542662.341663,4522768.280942 542547.783918,4522736.214601 542474.32252,4522820.663952 542365.00339,4522960.011345 542373.461815,4522965.920834 542372.066914,4522966.515741 542309.04093,4523010.300192 542190.435304,4523110.838364 542087.515636,4523150.876323 542008.173486,4523149.843436 541954.445909,4523041.186841 541911.003338,4522793.50311 541812.131168,4522700.934849 541720.71592,4522722.204031 541727.397828,4522732.230611 541699.665122,4522794.559471 541796.021118,4522946.723392 541776.450041,4523046.940695 541872.235728,4523217.119394 541838.0426,4523215.477088 541837.426743,4523260.960559 541556.031542,4523299.794572 541502.823271,4523006.342403 541517.232247,4522925.45523 541499.643955,4522905.6931 541391.874528,4522940.571247 541306.980169,4522995.847005 541217.399515,4522943.086319 541140.033253,4522969.880891 541076.473564,4523112.074941 541040.018281,4523106.5928 540970.473878,4523141.837811 540932.207356,4523065.572536 540894.104423,4522896.416594 540951.67872,4522820.548905 540823.933563,4522747.443429 540732.754387,4522583.436237 540842.531832,4522392.6222 540842.450628,4522389.631129 540937.980237,4522285.686378 541007.114118,4522234.49133 540913.999984,4522128.542278 540852.538764,4522099.812102 540844.571423,4522102.856975 540731.734205,4522053.192807 540674.185291,4521984.245945 540652.878761,4521866.865051 540571.505772,4521740.132982 540525.287982,4521644.440167 540588.056731,4521544.246982 540511.789284,4521349.251246 540417.139527,4521280.857436 540320.978992,4521303.949478 540170.233611,4521006.214501 540157.086662,4520941.203464 540369.669367,4521014.470734 540538.320534,4520969.915471 540514.177919,4520915.64793 540521.390462,4520916.150673 540552.262649,4520704.299679 540781.725618,4520689.019753 540799.98932,4520627.841167 540713.103891,4520603.3554 540616.147139,4520558.589353 540551.807047,4520571.503134 540494.675747,4520509.517039 540519.406257,4520388.97052 540588.333446,4520329.301836 540568.666414,4520240.271185 540568.451355,4520140.914861 540528.75827,4520044.772401 540376.622424,4520041.750362 540133.338598,4520058.477873 540022.28196,4519980.225152 540125.913187,4519854.253438 540257.69635,4519752.811566 540349.045268,4519640.668152 540461.516996,4519547.046997 540328.727133,4519536.701767 540025.314557,4519544.078981 539948.489835,4519483.446248 539873.919811,4519455.6151 539796.256512,4519439.981461 539797.532972,4519460.367516 539776.652903,4519540.701214 539757.350077,4519555.011489 539812.881579,4519624.599561 539791.529215,4519698.973417 539758.845135,4519800.093115 539721.483322,4519928.499803 539669.96651,4520044.397963 539617.144288,4520140.410985 539516.858114,4520219.713302 539434.994759,4520278.274196 539276.993171,4520381.520176 539193.188389,4520412.750414 539187.370668,4520441.613848 539185.35861,4520507.668167 539171.036589,4520588.051316 539093.240319,4520705.757429 539072.395113,4520786.589491 539014.79954,4520909.397995 539049.213673,4521025.879678 539111.33692,4521160.430527 539127.906951,4521196.243853 539225.358434,4521185.546783 539297.673581,4521180.573579 539285.408226,4521386.155856 539157.397088,4521399.952969 539180.348894,4521492.254407 539240.878068,4521699.821223 539314.528063,4521878.520781 539303.140145,4521876.307929 539253.742855,4522047.49139 539219.054292,4522230.147139 539100.293609,4522326.703446 538956.456452,4522294.650698 538940.046445,4522403.642225 538952.059894,4522431.279649 538887.69136,4522539.075257 538991.786794,4522671.237808 538919.417447,4522717.164189 538815.449039,4522664.891432 538695.670011,4522628.687096 538613.459271,4522681.282919 538493.092014,4522923.267152 538404.769966,4523022.225089 538314.408498,4523008.966334 538262.989311,4522946.587023 538064.306778,4522970.242252 537997.977005,4522983.294548 537951.209383,4522954.052221 537930.020538,4522885.597891 537950.641851,4522828.749035 538006.914849,4522782.930886 538062.19347,4522727.194322 537942.109835,4522637.577909 537888.508252,4522524.411866 537840.798728,4522478.755617 537821.252816,4522538.526474 537746.175962,4522584.139893 537740.513308,4522588.024673 537733.108352,4522690.905636 537652.682944,4522735.888453 537530.219006,4522876.646898 537513.364979,4522904.772435 537307.208058,4522891.488375 537347.092313,4522838.806836 537437.927741,4522793.607629 537478.816029,4522767.324005 537529.300027,4522655.487387 537548.898328,4522569.245654 537525.598903,4522540.886243 537494.215372,4522546.540457 537422.542279,4522559.959598 537383.537051,4522619.072105 537260.204219,4522631.051461 537184.638872,4522653.228412 537101.854567,4522693.878864 537071.323156,4522736.429094 536990.410066,4522807.912232 536842.307953,4522864.542305 536735.530824,4522944.797235 536669.983518,4523048.183125 536607.875747,4523105.389803 536468.615263,4523194.371356 536395.217496,4523289.808679 536397.233834,4523289.170431 536450.458059,4523391.377571 536437.533905,4523491.144202 536465.845781,4523615.040637 536502.138911,4523757.863504 536592.141869,4523817.589616 536642.682653,4523744.698267 536652.068427,4523576.261072 536717.609437,4523353.023654 536926.372497,4523330.671382 536857.480044,4523505.199591 536833.607378,4523628.190843 536775.939776,4523694.580937 536782.149797,4523698.647879 536769.383634,4523897.780251 536647.6127,4524143.863198 536604.052414,4524279.196052 536564.484577,4524411.757592 536677.282941,4524478.903327 536547.976856,4524712.522327 536491.612261,4524738.872894 536490.298162,4524742.459268 536439.213737,4524802.903962 536462.772848,4524847.725577 536510.288249,4524886.903221 536510.150638,4524919.372321 536454.301091,4524983.141681 536392.192913,4525153.710076 536433.275937,4525158.373884 536433.22164912,4525158.84877673</gml:coordinates></gml:LinearRing></gml:outerBoundaryIs><gml:innerBoundaryIs><gml:LinearRing><gml:coordinates xmlns:gml="http://www.opengis.net/gml" decimal="." cs="," ts=" ">552313.323482,4514331.086024 552279.876227,4514343.868077 552079.969037,4514455.459279 552035.695904,4514390.599532 551967.723553,4514362.316656 551869.794677,4514283.168419 551788.653839,4514255.790545 551624.589788,4514295.020529 551511.070515,4514206.956903 551484.482146,4514137.886161 551467.653711,4514094.606292 551519.469364,4513911.306305 551663.427974,4513735.657037 551703.7363,4513666.484044 551756.006814,4513577.017995 552044.065962,4513551.240975 552313.207116,4513526.263927 552482.341292,4513488.184587 552713.576089,4513392.918009 552713.845738,4513300.533878 552656.364281,4513199.635061 552657.986129,4513190.536856 552744.553731,4513140.655438 552881.295523,4513170.705923 552946.605863,4513140.257583 553003.853723,4513203.22822 552880.456735,4513322.041866 552920.933543,4513431.597541 553034.725196,4513549.098293 553058.09622,4513600.415419 553048.194159,4513717.924448 552998.531802,4513736.314216 552830.424727,4513860.197449 552722.465399,4514018.393677 552726.663621,4514034.081974 552655.321329,4514083.917166 552638.264947,4514124.531627 552617.480136,4514205.343685 552539.036604,4514217.222663 552442.474227,4514157.952076 552283.099245,4514241.794665 552313.323482,4514331.086024</gml:coordinates></gml:LinearRing></gml:innerBoundaryIs><gml:innerBoundaryIs><gml:LinearRing><gml:coordinates xmlns:gml="http://www.opengis.net/gml" decimal="." cs="," ts=" ">545659.921629,4519156.691287 545528.792674,4519172.197882 545429.643602,4519073.657676 545405.344814,4519008.91836 545372.239855,4518912.32852 545283.583301,4518871.986746 545176.21846,4518846.412378 545116.773939,4518751.63329 545243.020904,4518710.497969 545240.418488,4518701.689347 545340.518713,4518543.513409 545413.944129,4518373.190382 545488.965575,4518329.585953 545589.242786,4518244.798989 545605.881889,4518247.150246 545640.362753,4518200.340363 545669.222037,4518138.938024 545672.114143,4518099.293028 545669.759829,4518090.467032 545628.071758,4517932.551871 545651.936182,4517887.470392 545742.030917,4517874.28767 545806.323633,4517834.416697 545838.858611,4517924.553887 545851.763052,4518016.041179 545808.590958,4518114.378284 545828.342095,4518147.473299 545860.557674,4518231.141589 545832.620994,4518305.962451 545768.263331,4518330.359506 545760.336603,4518334.898915 545762.193628,4518343.758741 545691.62404,4518368.582483 545656.180278,4518430.43831 545628.812062,4518511.711112 545557.126609,4518621.996034 545514.389782,4518770.236352 545512.370888,4518835.286796 545608.118388,4518891.61864 545698.222197,4518914.884048 545654.703112,4518977.29483 545656.635021,4518978.160675 545757.209975,4519041.151203 545771.227184,4519021.712517 545837.243939,4519054.123294 545916.13778,4519048.69965 546033.652355,4519027.637747 546084.395735,4518997.685348 546135.583275,4518974.193457 546168.057372,4519074.320849 546162.581215,4519078.192648 546158.647021,4519309.64708 546148.515566,4519449.153991 546259.946079,4519533.866069 546244.597301,4519693.704166 546325.848584,4519722.570154 546335.447939,4519856.725264 546351.652323,4519869.092856 546349.701146,4520069.452924 546421.565862,4520058.019795 546496.755618,4520191.160544 546560.930619,4520067.91004 546538.668768,4520011.021326 546540.18148,4520009.418766 546491.847486,4519927.359916 546476.24111,4519796.61347 546546.1297,4519787.314224 546546.138944,4519783.818408 546629.099702,4519643.298708 546631.40446,4519644.138702 546662.760512,4519724.369777 546683.61913,4519835.281432 546704.967233,4519875.256639 546692.428157,4519892.595962 546677.38523,4520071.386403 546616.017664,4520166.481553 546614.222972,4520361.837628 546716.512854,4520614.945856 546665.990608,4520635.39739 546613.436621,4520639.011717 546502.084716,4520631.689501 546503.867736,4520628.570995 546427.652262,4520520.467868 546396.455288,4520366.327152 546316.287525,4520316.914866 546209.155357,4520243.892231 546159.059442,4520143.478967 546027.2255,4520130.574281 545958.484736,4520094.856441 545939.049161,4520022.792687 546126.977854,4520001.881421 546012.433718,4519826.507581 546029.118119,4519691.543067 546004.259621,4519616.856506 545939.76853,4519644.759291 545848.289177,4519685.003024 545788.173867,4519622.227355 545821.718717,4519560.002598 545737.146289,4519491.918006 545874.144491,4519331.204915 545960.37787,4519221.917154 545847.739434,4519157.759555 545791.879859,4519209.53425 545722.791942,4519177.833913 545706.512627,4519153.487934 545659.921629,4519156.691287</gml:coordinates></gml:LinearRing></gml:innerBoundaryIs></gml:Polygon></gml:polygonMember></gml:MultiPolygon></rsdi:the_geom></rsdi:corine></gml:featureMember></wfs:FeatureCollection>

                        altro esempio di chiamata (text)
                        http://rsdi.regione.basilicata.it/geoserver/wms?VERSION=1.1.1&SERVICE=WMS&REQUEST=GetFeatureInfo&BBOX=15.3484689043799,40.2707315886485,16.1038691585095,41.003494987216&info_format=text/plain&styles=&query_layers=rsdi:corine&WIDTH=901&HEIGHT=874&SRS=EPSG:4326&TRANSPARENT=TRUE&STYLES=&LAYERS=topp:autostrade,sirs:Carta_suoli_2006_utente_2_543,rsdi:corine&x=200&y=200
                        sua risposta: Results for FeatureType 'corine':
                        --------------------------------------------
                        gid = 53
                        boundary = null
                        perimetro = 257346
                        classe5 = TERRITORI BOSCATI E AMBIENTI SEMI-NATURALI
                        classe15 = ZONE BOSCATE
                        classe44 = BOSCHI DI LATIFOGLIE
                        fid = 1080
                        the_geom = [GEOMETRY (MultiPolygon) with 2481 points]
                        --------------------------------------------

                        ultimo esempio di chiamata (html): 
                        http://rsdi.regione.basilicata.it/geoserver/wms?VERSION=1.1.1&SERVICE=WMS&REQUEST=GetFeatureInfo&BBOX=15.3484689043799,40.2707315886485,16.1038691585095,41.003494987216&info_format=text/html&styles=&query_layers=rsdi:corine&WIDTH=901&HEIGHT=874&SRS=EPSG:4326&TRANSPARENT=TRUE&STYLES=&LAYERS=topp:autostrade,sirs:Carta_suoli_2006_utente_2_543,rsdi:corine&x=200&y=200
                        sua risposta:
                                    <html>
                          <head>
                            <title>Geoserver GetFeatureInfo output</title>
                          </head>
                          <style type="text/css">
                            table.featureInfo, table.featureInfo td, table.featureInfo th {
                                border:1px solid #ddd;
                                border-collapse:collapse;
                                margin:0;
                                padding:0;
                                font-size: 90%;
                                padding:.2em .1em;
                            }
                            table.featureInfo th {
                                padding:.2em .2em;
                                font-weight:bold;
                                background:#eee;
                            }
                            table.featureInfo td{
                                background:#fff;
                            }
                            table.featureInfo tr.odd td{
                                background:#eee;
                            }
                            table.featureInfo caption{
                                text-align:left;
                                font-size:100%;
                                font-weight:bold;
                                text-transform:uppercase;
                                padding:.2em .2em;
                            }
                          </style>
                          <body>

                        <table class="featureInfo">
                          <caption class="featureInfo">corine</caption>
                          <tr>
                          <th>fid</th>
                            <th >gid</th>
                            <th >boundary</th>
                            <th >perimetro</th>
                            <th >classe5</th>
                            <th >classe15</th>
                            <th >classe44</th>
                            <th >fid</th>
                          </tr>

                            <tr>

                          <td>corine.53</td>    
                              <td>53</td>
                              <td></td>
                              <td>257346</td>
                              <td>TERRITORI BOSCATI E AMBIENTI SEMI-NATURALI</td>
                              <td>ZONE BOSCATE</td>
                              <td>BOSCHI DI LATIFOGLIE</td>
                              <td>1080</td>
                          </tr>
                        </table>
                        <br/>

                          </body>
                        </html>
            */

        }
    }
}
