//==============================================================================
// Author: Fabrizio Vita            
// Date: 2014-03-20
// Copyright: (c)2010-2011 ItacaSoft di Vita Fabrizio. ALL RIGHTS RESERVED.
//===============================================================================
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using SharpMapX.Wp.Sample.Providers;
using System.Windows.Controls.Primitives;

namespace SharpMapX.Wp.Sample
{
    public partial class OptionsDlg : UserControl
    {
        public OptionsDlg()
        {
            InitializeComponent();
        }

        public ProviderMapType MapType
        {
            get
            {
                if (sl.IsChecked == true)
                {
                    return ProviderMapType.BingMapsSatellite;
                }
                else if (h.IsChecked == true)
                {
                    return ProviderMapType.BingMapsHibrid;
                }
                else if (st.IsChecked == true)
                {
                    return ProviderMapType.BingMapsRoad;
                }
                //else if (rbGeoportail.IsChecked == true)
                //{
                //    return ProviderMapType.Custom;
                //}
                else if (googlest.IsChecked == true)
                {
                    return ProviderMapType.GoogleRoad;
                }
                else if (googlehyb.IsChecked == true)
                {
                    return ProviderMapType.GoogleHybrid;
                }
                else if (googlesat.IsChecked == true)
                {
                    return ProviderMapType.GoogleSatellite;
                }
                else if (googlemorph.IsChecked == true)
                {
                    return ProviderMapType.GooglePhisical;
                }
                else
                {
                    return ProviderMapType.Undefined;
                }
            }

            set
            {
                if (value == ProviderMapType.BingMapsRoad)
                {
                    st.IsChecked = true;
                }
                else if (value == ProviderMapType.BingMapsSatellite)
                {
                    sl.IsChecked = true;
                }
                else if (value == ProviderMapType.BingMapsHibrid)
                {
                    h.IsChecked = true;
                }
                else if (value == ProviderMapType.GoogleRoad)
                {
                    googlest.IsChecked = true;
                }
                else if (value == ProviderMapType.GoogleSatellite)
                {
                    googlesat.IsChecked = true;
                }
                else if (value == ProviderMapType.GoogleHybrid)
                {
                    googlehyb.IsChecked = true;
                }
                else if (value == ProviderMapType.GooglePhisical)
                {
                    googlemorph.IsChecked = true;
                }
                //else if (value == ProviderMapType.Custom)
                //{
                //    rbGeoportail.IsChecked = true;
                //}
                else
                {
                    googlehyb.IsChecked = false;
                    googlemorph.IsChecked = false;
                    googlesat.IsChecked = false;
                    googlest.IsChecked = false;
                    h.IsChecked = false;
                    sl.IsChecked = false;
                    st.IsChecked = false;
                    //                    rbGeoportail.IsChecked = false;
                }
            }
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            ClosePopup();
        }

        private void ClosePopup()
        {
            Popup myPop = this.Parent as Popup;
            myPop.IsOpen = false;
        }

    }
}
