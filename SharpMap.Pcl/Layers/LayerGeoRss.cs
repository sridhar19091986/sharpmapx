//==============================================================================
// Copyright 2010-2014 - Fabrizio Vita (www.itacasoft.com)
// This file can be redistributed and/or modified under the terms of the GNU Lesser General Public License.
//===============================================================================

using Portable.Http;
using SharpMap.Entities;
using SharpMap.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Xml;
using System.Xml.Linq;

namespace SharpMap.Layers
{
    public class LayerGeoRss : LayerBaseDrawable, IRemoteLayer
    {
        string _url;
        IWebClient _webClient;

        public LayerGeoRss(string url)
        {
            _url = url;
        }

        public LayerGeoRss()
        {
        }

        /// <summary>
        /// Url of the georss service
        /// </summary>
        public string Url
        {
            get
            {
                return _url;
            }
            set
            {
                _url = value;
            }
        }

        public string Link { get; set; }
        public string Description { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public DateTime Updated { get; set; }
        public string ImageUrl { get; set; }
        public string ImageLink { get; set; }
        public string ImageTitle { get; set; }
        public int ImageWidth { get; set; }
        public int ImageHeight { get; set; }

        /// <summary>
        /// Event thrown when a download is completed.
        /// </summary>
        public event EventHandler<FeaturesEventArgs> OnGetFeaturesCompleted;


        /// <summary>
        /// Initiates a getfeatures request.
        /// </summary>
        public void GetFeatures()
        {
            if (OnGetFeaturesCompleted == null)
                throw new Exception("OnGetFeaturesCompleted cannot be null");

            if (_url == null)
                throw new Exception("Url cannot be null");

            BusyState = true;

            if (_webClient == null)
            {
                _webClient = new WebClient();
                _webClient.DownloadStringCompleted += new DownloadStringCompletedEventHandler(GetFeaturesCompleted);
            }

            _webClient.DownloadStringAsync(new Uri(_url, UriKind.RelativeOrAbsolute));
        }

        bool _busy;

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

        public bool CheckException(string xml)
        {
            return false;
        }

        #region INotifyPropertyChanged Members
        /// <summary>
        /// This event is raised when a property is changed.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion


        void GetFeaturesCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            GisShapeList features = new GisShapeList();

            try
            {
                XDocument doc;
                var settings = new XmlReaderSettings();
                settings.DtdProcessing = DtdProcessing.Ignore;

                using (XmlReader reader = XmlReader.Create(new StringReader(e.Result), settings))
                {
                    doc = XDocument.Load(reader);
                }

                var docRoot = doc.Element("rss");
                if (docRoot == null) return;

                var xchannel = docRoot.Element("channel");
                if (xchannel == null) return;

                Title = xchannel.Element("title") != null ? xchannel.Element("title").Value : "";
                Link = xchannel.Element("link") != null ? xchannel.Element("link").Value : "";
                Description = xchannel.Element("description") != null ? xchannel.Element("description").Value : "";
                Author = xchannel.Element("author") != null ? xchannel.Element("author").Value : "";
                Updated = xchannel.Element("updated") != null ? Parser.StringAsDateTime(xchannel.Element("updated").Value) : DateTime.MinValue;

                var ximage = xchannel.Element("image");
                if (ximage != null)
                {
                    ImageUrl = ximage.Element("url") != null ? ximage.Element("url").Value : "";
                    ImageLink = ximage.Element("link") != null ? ximage.Element("link").Value : "";
                    ImageTitle = ximage.Element("title") != null ? ximage.Element("title").Value : "";
                    ImageHeight = ximage.Element("height") != null ? Parser.StringAsInteger(ximage.Element("height").Value, 0) : 0;
                    ImageWidth = ximage.Element("width") != null ? Parser.StringAsInteger(ximage.Element("width").Value, 0) : 0;
                }

                var xitems = xchannel.Descendants("item");
                if (xitems == null) return;

                foreach (XElement xitem in xitems)
                {
                    GisShapePoint p = new GisShapePoint(null);
                    var keys = new Collection<string>();
                    keys.Add("magnitudo");
                    keys.Add("date");
                    keys.Add("link");
                    keys.Add("depth");
                    keys.Add("title");
                    keys.Add("description");
                    p.PopulateTypes(keys);

                    string title = xitem.Element("title") != null ? xitem.Element("title").Value : "";
                    p["title"] = title;

                    string link = xitem.Element("link") != null ? xitem.Element("link").Value : "";
                    p["link"] = link;

                    string description = xitem.Element("description") != null ? xitem.Element("description").Value : "";
                    p["description"] = description;

                    /*Esempio description: <a href='http://cnt.rm.ingv.it/data_id/7232002670/event.html'><img width='225' height='225' border='0' src='http://cnt.rm.ingv.it/data_id/7232002670/map_loc_t.jpg' alt='epicentro evento' align='left'  /></a><p style='color: #333333;font: 14;line-height: 28'>&nbsp;ID:&nbsp;7232002670<br />&nbsp;Data:&nbsp;10/02/2014 07.07.40<br />&nbsp;Magnitudo:&nbsp;2.0<br />&nbsp;Distretto:&nbsp;France<br />&nbsp;Lat:&nbsp;45.336 - &nbsp;Lon:&nbsp;6.543<br />&nbsp;Profondit&agrave;:&nbsp;10.0 km</p> */

                    if ((description.Contains("Lon:&nbsp;")) && (description.Contains("Lat:&nbsp;")))
                    {
                        int indexId0 = description.IndexOf(@"http://cnt.rm.ingv.it/data_id/");
                        int indexId1 = description.IndexOf(@"/event", indexId0);
                        String sid = description.Substring(indexId0 + 30, indexId1 - indexId0 - 30);
                        Int64 id;
                        Int64.TryParse(sid, out id);
                        p.UID = id;

                        int indexLon0 = description.IndexOf("Lon:&nbsp;");
                        int indexLon1 = description.IndexOf("<br", indexLon0);
                        String sLon = description.Substring(indexLon0 + 10, indexLon1-indexLon0 - 10);

                        int indexLat0 = description.IndexOf("Lat:&nbsp;");
                        int indexLat1 = description.IndexOf(" -", indexLat0);
                        String sLat = description.Substring(indexLat0 + 10, indexLat1-indexLat0-10);

                        p.Point.Y = Parser.StringAsDouble(sLat, -1);
                        p.Point.X = Parser.StringAsDouble(sLon, -1);

                        int indexMagnitudo0 = description.IndexOf("Magnitudo:&nbsp;");
                        int indexMagnitudo1 = description.IndexOf("<br", indexMagnitudo0);
                        String magnitudo = description.Substring(indexMagnitudo0 + 16, indexMagnitudo1-indexMagnitudo0-16);
                        p["magnitudo"] = magnitudo;

                        int indexData0 = description.IndexOf("Data:&nbsp;");
                        int indexData1 = description.IndexOf("<br", indexData0);
                        String date = description.Substring(indexData0 + 11, indexData1 -indexData0 -11);
                        p["date"] = date;
                    }

                    features.Add(p);
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                BusyState = false;
                if (OnGetFeaturesCompleted != null)
                    OnGetFeaturesCompleted(sender, new FeaturesEventArgs(features));
            }
        }

        public override Entities.Extent BoundingBox
        {
            get; set;
        }

        public void InitializeConnection(IWebClient webClient)
        {
            if (_webClient != null)
            {
                _webClient.DownloadStringCompleted -= GetFeaturesCompleted;
            }

            _webClient = webClient;
            _webClient.DownloadStringCompleted += GetFeaturesCompleted;
        }
    }
}
