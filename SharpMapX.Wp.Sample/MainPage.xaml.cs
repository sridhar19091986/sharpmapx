//==============================================================================
// Author: Fabrizio Vita            
// Date: 2014-02-22
// Copyright: (c)2010-2014 ItacaSoft di Vita Fabrizio. ALL RIGHTS RESERVED.
//===============================================================================
using System.Windows;
using Microsoft.Phone.Controls;
using System;
using Microsoft.Phone.Controls.Maps;
using System.Windows.Media;
using System.Device.Location;
using Microsoft.Phone.Shell;
//==============================================================================
// Author: Fabrizio Vita            
// Date: 2014-03-20
// Copyright: (c)2010-2011 ItacaSoft di Vita Fabrizio. ALL RIGHTS RESERVED.
//===============================================================================
using System.Windows.Controls.Primitives;
using Microsoft.Phone.Controls.Maps.Core;
using SharpMap.Mobile.Wp.Sample.Providers;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using Size = SharpMap.Styles.Size;
using System.Text;
using System.Globalization;
using Portable.Http;
using GeoAPI.Geometries;
using SharpMap.GMLUtils;

namespace SharpMap.Mobile.Wp.Sample
{
    public partial class MainPage : PhoneApplicationPage
    {
        private static string ApplicationId = "ApBXPZf5IR94SLXE8nh5FYsb5WHKrH1XPY7428-EqQudseivcWhCROIJvGmtnkAV";
        private static string WmsServiceUrl = "http://95.110.142.169/demo/itacamap/srv/itacamapsrv_carta_geologica.dll/wms";
        private static string QueryLayer = "cartageologica";
        private MapLayer _mapLayer;
        private Image _serverImage;
        private int _serverImageIndex = 0;
        private bool GetFeatureInfoEnabled;
        private string lastErrorMessage;
        private const string INFO_FORMAT_TEXT = "text/plain";
        private const string INFO_FORMAT_HTML = "text/html";
        private const string INFO_FORMAT_GML = "application/vnd.ogc.gml";

        public MainPage()
        {
            InitializeComponent();
            MyMap.CredentialsProvider = new ApplicationIdCredentialsProvider(ApplicationId);
        }

        private void Find_Click(object sender, EventArgs e)
        {
            string miobottone = (sender as ApplicationBarIconButton).IconUri.OriginalString;

            if (miobottone.Contains("yellow"))
            {
                (sender as ApplicationBarIconButton).IconUri = new Uri(@"/Images/menu_find.png", UriKind.RelativeOrAbsolute);
            }
            else
            {
                (sender as ApplicationBarIconButton).IconUri = new Uri(@"/Images/menu_find_yellow.png", UriKind.RelativeOrAbsolute);
            }

            GetFeatureInfoEnabled = (sender as ApplicationBarIconButton).IconUri.OriginalString.Contains("yellow");
        }

        private void ShowOptions_Click(object sender, EventArgs e)
        {
            ShowOptionsDialog();
        }

        private void ZoomIn_Click(object sender, EventArgs e)
        {
            MyMap.ZoomLevel++;
        }

        private void ZoomOut_Click(object sender, EventArgs e)
        {
            MyMap.ZoomLevel--;
        }

        #region GetFeatureInfo
        private void MyMap_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Point p = e.GetPosition(MyMap);

            var aSize = new Size();
            aSize.Width = MyMap.ActualWidth;
            aSize.Height = MyMap.ActualHeight;

            string url = GetFeatureInfoRequest((int)p.X, (int)p.Y, aSize, MyMap.BoundingRectangle);

            WebClient client = new WebClient();
            client.DownloadStringCompleted += new DownloadStringCompletedEventHandler(GetFeatureInfoCompleted);
            client.DownloadStringAsync(new Uri(url, UriKind.RelativeOrAbsolute));
        }


        private string GetFeatureInfoRequest(int x, int y, Size actualSize, LocationRect locationRect)
        {
            var b = new StringBuilder();
            b.Append(WmsServiceUrl);

            b.Append("?request=getfeatureinfo");
            b.Append("&SRS=EPSG:3857");
            b.Append("&info_format=" + INFO_FORMAT_HTML);
            b.Append("&layers=&styles=");
            b.Append("&query_layers=");
            b.Append(QueryLayer);
            b.Append("&width=");
            b.Append(actualSize.Width);
            b.Append("&height=");
            b.Append(actualSize.Height);

            b.Append("&bbox=");

            b.Append(string.Format(CultureInfo.InvariantCulture, "{0}", locationRect.West));
            b.Append(",");
            b.Append(string.Format(CultureInfo.InvariantCulture, "{0}", locationRect.South));
            b.Append(",");
            b.Append(string.Format(CultureInfo.InvariantCulture, "{0}", locationRect.East));
            b.Append(",");
            b.Append(string.Format(CultureInfo.InvariantCulture, "{0}", locationRect.North));

            b.Append("&x=");
            b.Append(x);

            b.Append("&y=");
            b.Append(y);

            return b.ToString();
        }

        void GetFeatureInfoCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            System.Windows.Deployment.Current.Dispatcher.BeginInvoke(() =>
            {
                try
                {
                    var gl = new GMLLayer();
                    var dialog = new ShowFeatureDlg();
                    dialog.Show(e.Result);
                    Popup optionsScreen;
                    optionsScreen = new Popup();
                    optionsScreen.Child = dialog;
                    optionsScreen.IsOpen = true;
                    optionsScreen.VerticalOffset = 100;
                    optionsScreen.HorizontalOffset = 25;

                    return;
                    SharpMap.GMLUtils.GMLProvider reader = new GMLUtils.GMLProvider(e.Result, gl);
                    var geometries = reader.Features;

                }
                catch (Exception ex)
                {
                    throw;
                }
                finally
                {
                }
            });
        }
        #endregion

        public void ShowOptionsDialog()
        {
            OptionsDlg dialog = new OptionsDlg();
            if (hybrid.Visibility == System.Windows.Visibility.Visible)
            {
                dialog.MapType = ProviderMapType.GoogleHybrid;
            }
            else if (street.Visibility == System.Windows.Visibility.Visible)
            {
                dialog.MapType = ProviderMapType.GoogleRoad;
            }
            else if (satellite.Visibility == System.Windows.Visibility.Visible)
            {
                dialog.MapType = ProviderMapType.GoogleSatellite;
            }
            else if (MyMap.Mode is RoadMode)
            {
                dialog.MapType = ProviderMapType.BingMapsRoad;
            }
            else if (MyMap.Mode is AerialMode)
            {
                if ((MyMap.Mode as AerialMode).ShouldDisplayLabels)
                    dialog.MapType = ProviderMapType.BingMapsHibrid;
                else
                    dialog.MapType = ProviderMapType.BingMapsSatellite;
            }

            Popup optionsScreen;
            optionsScreen = new Popup();
            optionsScreen.Child = dialog;
            optionsScreen.IsOpen = true;
            optionsScreen.VerticalOffset = 100;
            optionsScreen.HorizontalOffset = 25;
            optionsScreen.Closed += (s1, e1) =>
            {
                if (dialog.MapType == ProviderMapType.BingMapsHibrid)
                {
                    MyMap.Mode = new AerialMode(true);

                    hybrid.Visibility = Visibility.Collapsed;
                    satellite.Visibility = Visibility.Collapsed;
                    street.Visibility = Visibility.Collapsed;
                    physical.Visibility = Visibility.Collapsed;
                }
                else if (dialog.MapType == ProviderMapType.BingMapsRoad)
                {
                    MyMap.Mode = new RoadMode();

                    hybrid.Visibility = Visibility.Collapsed;
                    satellite.Visibility = Visibility.Collapsed;
                    street.Visibility = Visibility.Collapsed;
                    physical.Visibility = Visibility.Collapsed;
                }
                else if (dialog.MapType == ProviderMapType.BingMapsSatellite)
                {
                    MyMap.Mode = new AerialMode(true);

                    hybrid.Visibility = Visibility.Collapsed;
                    satellite.Visibility = Visibility.Collapsed;
                    street.Visibility = Visibility.Collapsed;
                    physical.Visibility = Visibility.Collapsed;
                }
                else if (dialog.MapType == ProviderMapType.GoogleRoad)
                {
                    MyMap.Mode = new MercatorMode();

                    hybrid.Visibility = Visibility.Collapsed;
                    satellite.Visibility = Visibility.Collapsed;
                    street.Visibility = Visibility.Visible;
                    physical.Visibility = Visibility.Collapsed;
                }
                else if (dialog.MapType == ProviderMapType.GoogleHybrid)
                {
                    MyMap.Mode = new MercatorMode();

                    hybrid.Visibility = Visibility.Visible;
                    satellite.Visibility = Visibility.Collapsed;
                    street.Visibility = Visibility.Collapsed;
                    physical.Visibility = Visibility.Collapsed;
                }
                else if (dialog.MapType == ProviderMapType.GoogleSatellite)
                {
                    MyMap.Mode = new MercatorMode();

                    hybrid.Visibility = Visibility.Collapsed;
                    satellite.Visibility = Visibility.Visible;
                    street.Visibility = Visibility.Collapsed;
                    physical.Visibility = Visibility.Collapsed;
                }
                else if (dialog.MapType == ProviderMapType.GooglePhisical)
                {
                    MyMap.Mode = new MercatorMode();

                    hybrid.Visibility = Visibility.Collapsed;
                    satellite.Visibility = Visibility.Collapsed;
                    street.Visibility = Visibility.Collapsed;
                    physical.Visibility = Visibility.Visible;
                }
                else if (dialog.MapType == ProviderMapType.Custom)
                {
                    MyMap.Mode = new RoadMode();

                    hybrid.Visibility = Visibility.Collapsed;
                    satellite.Visibility = Visibility.Collapsed;
                    street.Visibility = Visibility.Collapsed;
                    physical.Visibility = Visibility.Collapsed;
                }
                else
                {
                    hybrid.Visibility = Visibility.Collapsed;
                    satellite.Visibility = Visibility.Collapsed;
                    street.Visibility = Visibility.Collapsed;
                    physical.Visibility = Visibility.Collapsed;
                }
            };
        }

        private void MyMap_ViewChangeEnd(object sender, MapEventArgs e)
        {
            DrawWms();
        }

        void WmsImage_ImageFailed(object sender, ExceptionRoutedEventArgs e)
        {
            string errorMessage = "Failed getting wms image: " + e.ErrorException.Message;
            if (lastErrorMessage != errorMessage)
            {
                MessageBox.Show(errorMessage);
                lastErrorMessage = errorMessage;
            }
        }

        private void WmsImage_ImageOpened(object sender, RoutedEventArgs e)
        {
            while (_mapLayer.Children.Count > 1)
            {
                _mapLayer.Children.RemoveAt(0);
            }
        }

        private void DrawWms()
        {
            if (_mapLayer == null)
            {
                _mapLayer = new MapLayer();
                MyMap.Children.Add(_mapLayer);
            }

            if (_serverImage != null)
            {
                _serverImage.ImageOpened -= new EventHandler<RoutedEventArgs>(WmsImage_ImageOpened);
                _serverImage.ImageFailed -= new EventHandler<ExceptionRoutedEventArgs>(WmsImage_ImageFailed);
                _serverImage = null;
            }

            _serverImage = new Image();
            _serverImageIndex++;
            _serverImage.Name = "serverimage" + _serverImageIndex.ToString();

            var aSize = new SharpMap.Styles.Size();
            aSize.Height = MyMap.ActualHeight;
            aSize.Width = MyMap.ActualWidth;

            string aurl = GetMapRequest(MyMap.BoundingRectangle, aSize);

            BitmapImage bmp = new BitmapImage(new Uri(aurl, UriKind.RelativeOrAbsolute));
            _serverImage.Source = bmp;

            _serverImage.Opacity = 1;

            _serverImage.HorizontalAlignment = HorizontalAlignment.Stretch;
            _serverImage.VerticalAlignment = VerticalAlignment.Stretch;
            _serverImage.ImageOpened += new EventHandler<RoutedEventArgs>(WmsImage_ImageOpened);
            _serverImage.ImageFailed += new EventHandler<ExceptionRoutedEventArgs>(WmsImage_ImageFailed);

            _mapLayer.AddChild(_serverImage, MyMap.BoundingRectangle);
        }

        private String GetMapRequest(LocationRect boundingRectangle, Size size)
        {
            //example: http://www.itacasoft.com/demo/itacamap/srv/itacamapsrv_carta_geologica.dll/wms?request=getmap&TRANSPARENT=true&FORMAT=image/png&SERVICE=WMS&REQUEST=GetMap&STYLES=&VERSION=1.1.1&EXCEPTIONS=application/vnd.ogc.se_inimage&SRS=EPSG:3857&WIDTH=256&HEIGHT=256&LAYERS=cartageologica&BBOX=12.530597,43.600285,12.531284,43.600784

            var b = new StringBuilder();
            b.Append(WmsServiceUrl);
            b.Append("?request=getmap");
            b.Append("&TRANSPARENT=true&FORMAT=image/png&STYLES=&VERSION=1.1.1&EXCEPTIONS=application/vnd.ogc.se_inimage&SRS=EPSG:3857");
            b.Append("&width=");
            b.Append(size.Width.ToString());
            b.Append("&height=");
            b.Append(size.Height.ToString());
            b.Append("&layers=");
            b.Append(QueryLayer);
            b.Append("&bbox=");

            b.Append(string.Format(CultureInfo.InvariantCulture, "{0}", boundingRectangle.West));
            b.Append(",");
            b.Append(string.Format(CultureInfo.InvariantCulture, "{0}", boundingRectangle.South));
            b.Append(",");
            b.Append(string.Format(CultureInfo.InvariantCulture, "{0}", boundingRectangle.East));
            b.Append(",");
            b.Append(string.Format(CultureInfo.InvariantCulture, "{0}", boundingRectangle.North));

            return b.ToString();
        }


    }
}