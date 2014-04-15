//==============================================================================
// Author: Fabrizio Vita            
// Date: 2014-03-20
// Copyright: (c)2010-2011 ItacaSoft di Vita Fabrizio. ALL RIGHTS RESERVED.
//===============================================================================
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace SharpMapX.Wp.Sample
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

        private void ClosePopup()
        {
            Popup myPop = this.Parent as Popup;
            myPop.IsOpen = false;
        }

        private void Button_BindingValidationError(object sender, ValidationErrorEventArgs e)
        {

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ClosePopup();
        }
    }
}
