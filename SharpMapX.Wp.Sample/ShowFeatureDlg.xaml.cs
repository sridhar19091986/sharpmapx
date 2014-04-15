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

namespace SharpMap.Mobile.Wp.Sample
{
    public partial class ShowFeatureDlg : UserControl
    {
        private string html;

        public ShowFeatureDlg()
        {
            InitializeComponent();
        }

        public void Show(string html)
        {
            webBrowser1.NavigateToString(html);
        }

    }
}
