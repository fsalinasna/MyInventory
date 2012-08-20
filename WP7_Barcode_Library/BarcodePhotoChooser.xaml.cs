using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

 /*
 * Was going to use a custom task to relay the results, but they are not supported. See http://social.msdn.microsoft.com/Forums/en/windowsphone7series/thread/5ce4e5ec-0ffe-4099-bf94-f665a2e0dfcc?prof=required
 */

namespace WP7_Barcode_Library
{
    /// <summary>
    /// Page used to display a list of sample barcode images and let the user select which one they want to use.
    /// </summary>
    public partial class BarcodePhotoChooser : PhoneApplicationPage
    {
        private bool bSkip; //Flag used to skip event when fired from page load

        public BarcodePhotoChooser()
        {
            InitializeComponent();
        }

        private void lbSamples_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(lbSamples.SelectedIndex != -1)
            {
                if (!bSkip) //Check flag to see if event should be skipped
                {
                    PhoneApplicationService.Current.State["ReturnFromSampleChooser"] = true; //Set flag to indicate we are returning from Sample chooser
                    BarcodeSampleItemManager.SelectedItem = (BarcodeSampleItem)lbSamples.SelectedItem;
                    NavigationService.GoBack();
                }
                else
                {
                    bSkip = false; //Reset flag
                }
            }
        }

        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            if (BarcodeSampleItemManager.SelectedItem != null)
            {
                bSkip = true; //Set flag to skip event
                lbSamples.SelectedItem = BarcodeSampleItemManager.SelectedItem;
            }
            else
            {
                bSkip = false; //set flag to not skp events
            }
        }
    }
}