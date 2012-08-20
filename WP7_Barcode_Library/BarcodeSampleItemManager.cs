using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Collections.Generic;
using System.Windows.Media.Imaging;
using System.Xml.Linq;
using System.Linq;


/* UPDATE: See BarcodeChooserURI property for a static reference to the BarcodePhotoChooser.xaml uri
 * NOTE: since the BarcodePhotoChooser.xaml file is in a reference assembly you must use the following code to navigate to it:
 * NavigationService.Navigate(new Uri("/WP7_Barcode_Library;component/BarcodePhotoChooser.xaml", UriKind.RelativeOrAbsolute));
 * See http://www.davidpoll.com/2009/07/12/silverlight-3-navigation-navigating-to-pages-in-referenced-assemblies/ for more information
 */

namespace WP7_Barcode_Library
{
    /// <summary>
    /// Stores information for each sample image. Can be for data binding in ListPicker or ListBox.
    /// </summary>
    public class BarcodeSampleItem 
    {
        /// <summary>
        /// Returns the name of the sample file. Text property will return this if text has not been set manually.
        /// </summary>
        public string Filename { get { var strParts = FileURI.OriginalString.Split('/'); return strParts[strParts.Length - 1];} }
        
        private Uri _FileURI;
        /// <summary>
        /// Returns the URI to the sample file. The URI will be generated using the FilePath if it is not set manually.
        /// </summary>
        public Uri FileURI { 
        get { //TipnTrick
            if (_FileURI != null)
            {
                return _FileURI; //Return URI that was set when object was created
            }
            else if (!string.IsNullOrEmpty(FilePath))
            {
                //Create Full URI for given FilePath. Assume that files are loaded as content in executing assembly
                //Get name of executing assembly. Code from http://blog.wpfwonderland.com/2010/10/04/windows-phone-7-get-assembly-name/
                //var nameHelper = new System.Reflection.AssemblyName(System.Reflection.Assembly.GetExecutingAssembly().FullName);
                //NOTE: Above doesn't work when code is in library. GetCallingAssembly doesn't work either.
                
                //string strAppName = Application.Current.ToString(); //Gets WP7 App name with ".app" at the end
                //string strURI = string.Format("/{0};component/{1}", strAppName.Remove(strAppName.Length-4), FilePath.TrimStart('/')); //Create full URI to given path
                //THIS DIDN'T Work either! Application.GetResourceStream doesn't like the full URLs.
                return new Uri(FilePath, UriKind.Relative); 
            }
            else
            {
                return null;
            }
        } set { _FileURI = value; } }

        /// <summary>
        /// Path to the sample image. These should be content files but can also be resource files.
        /// <para>For content files use "/Samples/Type_UPC-A.png" and store them in the parent project (cannot use content files from referenced assemblies)</para>
        /// <para>For resource files use "/WP7_Barcode_Library;component/Icons/Type_UPC-A.png" where WP7_Barcode_Library is the assembly where the files are stored</para>
        /// <para>See detals on bulk media: http://msdn.microsoft.com/en-us/library/ff967560(v=VS.92).aspx#BKMK_Media </para>
        /// </summary>
        public string FilePath { get; set;} 

        /// <summary>
        /// Returns a BitmapImage of the sample file that can be used to for binding to an Image element
        /// </summary>
        public BitmapImage Bitmap { get
            {
                BitmapImage BarcodeImage = new BitmapImage(this.FileURI);
                BarcodeImage.CreateOptions = BitmapCreateOptions.None; //Override DelayCreation default to make sure that image is rendered properly.
                return BarcodeImage;
            }
        }
        
        private string _Text;
        /// <summary>
        /// The text that should be displayed next to the sample image. If omitted then the image filename will be used instead.
        /// </summary>
        public string Text { get { return string.IsNullOrEmpty(_Text) ? Filename : _Text;} set { _Text = value; } }
        
        /// <summary>
        /// Used to enable or disable this barcode type.
        /// </summary>
        public bool isEnabled { get; set; }

        /// <summary>
        /// Binding used to hide barcode types that are not enabled. Should be set on the grid or stack panel of the DataTemplate for ListBox or ListPicker control.
        /// </summary>
        public Visibility Visible { get { return isEnabled ? Visibility.Visible : Visibility.Collapsed; } }
    }

    /// <summary>
    /// Initializes the list of sample barcodes and can be used as the datacontext for ListPicker or ListBox
    /// </summary>
    public class BarcodeSampleItemManager
    {
        static BarcodeSampleItemManager()
        {
            //Initialize sample barcodes
            System.Windows.Resources.StreamResourceInfo sri = Application.GetResourceStream(new Uri("SampleImages.xml", UriKind.Relative));
            if (sri != null) //Load images specified in application's SampleImages.xml file
            {
                XDocument xDoc = XDocument.Load(sri.Stream);
                var q = from x in xDoc.Descendants("Sample")
                        select new BarcodeSampleItem
                        {
                            isEnabled = bool.Parse(x.Attribute("isEnabled").Value),
                            Text = x.Attribute("Text").Value,
                            FilePath = x.Attribute("FilePath").Value
                        };
                lstItems = q.ToList<BarcodeSampleItem>();
            }
            else //Load static list if SampleImages.xml does not exist. This is mainly for design time in Visual Studio.
            {
                    //TipnTrick
                    //NOTE: Samples must be either stored as resources in the WP7_Barcode_Library assembly or as resource or content files in the main application assembly.
                    //Also best practices state media files should be stored as content instead of resources to speed up application loading. See: http://msdn.microsoft.com/en-us/library/ff967560(v=VS.92).aspx#BKMK_Media
                    //Also you must use a leading / otherwise the image will have problems loading
                    //These are sample files from zxing source in \core\test\data
                    lstItems = new List<BarcodeSampleItem>()
                {
                    new BarcodeSampleItem {isEnabled = true, Text="UPC", FilePath = "/WP7_Barcode_Library;component/Icons/Type_UPC-A.png"}, //Resourse file from WP7_Barcode_Library assembly
                    new BarcodeSampleItem {isEnabled = true, Text="QR Code", FileURI = new Uri("http://upload.wikimedia.org/wikipedia/commons/c/ce/WikiQRCode.png", UriKind.Absolute)}, //Using live URL
                    new BarcodeSampleItem {isEnabled = true, FilePath = "/Samples/Sample_Book_1.jpg"},//Content file in parent application. NOTE: these will not be displayed at design time but will work once the xap is compiled.
                    new BarcodeSampleItem {isEnabled = true, FilePath = "/Samples/Sample_CD.jpg"}
                };
            }
        }

        private static List<BarcodeSampleItem> lstItems = new List<BarcodeSampleItem>();
        /// <summary>
        /// List of sample images that can be used as the data context for a ListPicker or ListBox
        /// </summary>
        public static List<BarcodeSampleItem> Items { get { return lstItems; } }

        /// <summary>
        /// Static reference to the URL that can be used to navigate to the BarcodePhotoChooser page with NavigationService.Navigate(...)
        /// </summary>
        public static Uri BarcodeChooserURI {get {return new Uri("/WP7_Barcode_Library;component/BarcodePhotoChooser.xaml", UriKind.Relative);}}
        
        /// <summary>
        /// Used to pass the selected item from the BarcodePhotoChooser page back to the parent page.
        /// </summary>
        public static BarcodeSampleItem SelectedItem { get; set; }
    }
}

