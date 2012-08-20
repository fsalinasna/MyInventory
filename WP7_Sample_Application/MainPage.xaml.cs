using System;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Phone.Controls;
using WP7_Barcode_Library;
using Microsoft.Phone.Shell;

namespace WP7_Sample_Application
{
    public partial class MainPage : PhoneApplicationPage
    {
        // Constructor
        public MainPage()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Method for setting the barcode scanning mode
        /// </summary>
        private void lpBarcodeType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //NOTE: code will be called when page is loaded before lpBarcodeType is initialize and may be called without a selected item
            if (lpBarcodeType != null && lpBarcodeType.SelectedIndex != -1)
            {
                BarcodeTypeItem selectedItem = lpBarcodeType.SelectedItem as BarcodeTypeItem;
                WP7BarcodeManager.ScanMode = selectedItem.BarcodeType; //Tell barcode library what type of scan we want to perform
            }
            if (((sender == lpBarcodeType) && (WP7BarcodeManager.LastCaptureResults.BarcodeImage != null)) && ((WP7BarcodeManager.LastCaptureResults.State == CaptureState.ScanFailed) || (WP7BarcodeManager.LastCaptureResults.State == CaptureState.UnknownError)))
            {
                start_progress(); //Try scanning previously failed image again using new selected mode
                WP7BarcodeManager.ScanBarcode(WP7BarcodeManager.LastCaptureResults.BarcodeImage, Barcode_Results);
            }
        }

        /// <summary>
        /// Loads the camera, lets the user take a picture, and then returns the result to a callback method.
        /// </summary>
        private void btnCamera_Click(object sender, RoutedEventArgs e)
        {
            start_progress();
            WP7BarcodeManager.ScanBarcode(Barcode_Results);
        }

        /// <summary>
        /// Callback method that processes results returned by the WP7BarcodeManager. Results are also stored at WP7BarcodeManager.LastCaptureResults.
        /// </summary>
        /// <param name="BCResults">Object that holds all the results of processing the barcode. Results are also stored at WP7BarcodeManager.LastCaptureResults.</param>
        public void Barcode_Results(WP7_Barcode_Library.BarcodeCaptureResult Results)
        {
            if (Results.BarcodeImage != null)
            {
                imgCapture.Source = Results.BarcodeImage; //Display image
            }
            else
            {
                //No image captured
            }
            if (Results.State == CaptureState.Success)
            {
                txtResults.Text = Results.BarcodeText; //Use results
            }
            else //Error occured
            {
                txtResults.Text = Results.ErrorMessage;
            }
            stop_progress();
        }

        public void start_progress()
        {
            pbStatus.IsIndeterminate = true;
        }

        public void stop_progress()
        {
            pbStatus.IsIndeterminate = false;
        }

        //Extra code required only if you want to use the sample/demo mode:

        /// <summary>
        /// Lets the user choose from sample images instead of using the camera since the emulator does not support the camera
        /// </summary>
        private void btnSample_Click(object sender, RoutedEventArgs e)
        {
            start_progress();
            NavigationService.Navigate(BarcodeSampleItemManager.BarcodeChooserURI); //Navigate to barcode chooser page located in WP7_Barcode_Library
        }

        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            //Detect if page is navigating back from BarcodeSampleChooser and start processing
            try
            {
                WP7BarcodeManager.aStartProgress = start_progress;

                if(PhoneApplicationService.Current.State.Keys.Contains("ReturnFromSampleChooser"))
                {
                    PhoneApplicationService.Current.State.Remove("ReturnFromSampleChooser");//Remove flag so code doesn't execute multiple times
                    if (BarcodeSampleItemManager.SelectedItem != null)
                    {
                        WP7BarcodeManager.ScanBarcode(BarcodeSampleItemManager.SelectedItem.FileURI, new Action<BarcodeCaptureResult>(this.Barcode_Results));
                    }
                    else //User backed out of the sample chooser without picking an image
                    {
                        MessageBox.Show("Error: Sample chooser canceled");
                    }
                }
                else if (PhoneApplicationService.Current.State.Keys.Contains("ReturnFromCameraCapture"))
                {
                    //You can also detect if we are returning from the camera and perform camera specific code here.
                    PhoneApplicationService.Current.State.Remove("ReturnFromCameraCapture"); //Remove flag so code doesn't execute multiple times
                }
                else //Initial page load (not from camera or sample chooser)
                {
                    lpBarcodeType_SelectionChanged(null, null); //Setup barcode type used for scanning
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Error processing sample image.", ex);
            }
        }
    }
}