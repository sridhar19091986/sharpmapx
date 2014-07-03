//==============================================================================
// Copyright 2010-2014 - Fabrizio Vita (www.itacasoft.com)
// This file can be redistributed and/or modified under the terms of the GNU Lesser General Public License.
//===============================================================================

using System.Collections.Generic;
using System.Linq;
using SharpMap.Entities;

namespace SharpMap.WMS
{
    public class WmsGetCapabilities
    {
        public WmsGetCapabilities()
        {
            Formats = new List<string>();
        }

        public List<string> Formats { get; set; }
        public string Url { get; set; }
    }

    public class WmsGetMapInfo
    {
        public WmsGetMapInfo()
        {
            Formats = new List<string>();
        }

        public List<string> Formats { get; set; }
        public string Url { get; set; }
    }

    public class WmsGetFeatureInfo
    {
        public WmsGetFeatureInfo()
        {
            Formats = new List<string>();
        }

        public List<string> Formats { get; set; }
        public string Url { get; set; }        
    }

    public class WmsGetLegendGraphic
    {
        public WmsGetLegendGraphic()
        {
            Formats = new List<string>();
        }

        public List<string> Formats { get; set; }
        public string Url { get; set; }        
    }

    public class WmsProjectInfo: BaseGisProjectInfo<WmsLayerInfo>
    {
        public WmsProjectInfo()
            : base()
        {
            Projections =  new List<string>();
            GetCapabilities = new WmsGetCapabilities();
            GetMapInfo = new WmsGetMapInfo();
            GetFeatureInfo = new WmsGetFeatureInfo();
            ExceptionFormats = new List<string>();
        }

        public string Title { get; set; }
        public string Abstract { get; set; }

        public List<string> Projections { get; private set; }

        public void CalculateExtent()
        {
            MaxX = -180;
            MinX = 180;
            MaxY = -90;
            MinY = 90;
            int index = 0;

            foreach (var layer in Layers)
            {
                if (layer.LatLonBoundingBox != null)
                {
                    if (layer.LatLonBoundingBox.MinX < MinX) MinX = layer.LatLonBoundingBox.MinX;
                    if (layer.LatLonBoundingBox.MinY < MinY) MinY = layer.LatLonBoundingBox.MinY;
                    if (layer.LatLonBoundingBox.MaxX > MaxX) MaxX = layer.LatLonBoundingBox.MaxX;
                    if (layer.LatLonBoundingBox.MaxY > MaxY) MaxY = layer.LatLonBoundingBox.MaxY;
                }
                else
                {
                }
                index++;
            }
        }
        public Extent LatLonBoundingBox
        {
            get;
            set;
        }

        //102113: va in mezzo all'atlantico
        //1024: va in mezzo all'atlantico
        //3785: crea righe orizzontali e verticali lato DK!!
        //3857: ok

        private string _mercatorEpsg = "3857";

        /// <summary>
        /// EPSG of the google mercatore projection
        /// </summary> 
        public string MercatorEpsg
        {
            get
            {
                if (_mercatorEpsg == "")
                {
                    if (Projections.Any(s => s.Contains("3857")))
                        _mercatorEpsg = "3857";
                    else if (Projections.Any(s => s.Contains("900913")))
                        _mercatorEpsg = "900913";
                }

                return _mercatorEpsg;
            }
        }

        public WmsGetCapabilities GetCapabilities { get; private set; }
        public WmsGetMapInfo GetMapInfo { get; private set; }
        public WmsGetFeatureInfo GetFeatureInfo { get; private set;}
        public WmsGetLegendGraphic GetLegendGraphic { get; internal set; }

        public List<string> ExceptionFormats { get; private set; }

        /// <summary>
        /// Returns a GIS layer given its name.
        /// </summary>
        /// <param name="layerName">Name of the layer.</param>
        /// <returns>layer base</returns>
        public override WmsLayerInfo GetLayerByName(string layerName)
        {
            foreach (WmsLayerInfo obj in Layers)
            {
                if (obj.Name.ToLower() == layerName.ToLower()) return obj;
            }
            return null;
        }

    }
}
