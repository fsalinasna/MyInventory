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

namespace WP7_Barcode_Library
{
    /// <summary>
    /// Stores information about each barcode type. Can be for data binding in ListPicker or ListBox.
    /// Each item represents a single barcode type or a group of types (ex: 'UPC or EAN', 'All 1D')
    /// </summary>
    public class BarcodeTypeItem 
    {
        /// <summary>
        /// Name of barcode type. Used for binding to text description
        /// </summary>
        public string Name { get; set; }
        
        /// <summary>
        /// Path to the icon image.
        /// </summary>
        public string Filename { get; set; }
        
        private Uri _URI = null;
        /// <summary>
        /// Full URI to icon image.
        /// </summary>
        public Uri FileURI { get { return _URI ?? new Uri("/WP7_Barcode_Library;component/" + this.Filename, UriKind.Relative); } set { _URI = value; } }
        
        /// <summary>
        /// BitmapImage of icon. Note: FileURI must be set before this is available.
        /// </summary>
        public BitmapImage SampleImage //TIPnTrick
        {
            get
            {
                BitmapImage BarcodeImage = new BitmapImage(this.FileURI);
                BarcodeImage.CreateOptions = BitmapCreateOptions.None; //Override DelayCreation default to make sure that image is rendered properly.
                return BarcodeImage;
            }
        }
        /// <summary>
        /// Used to enable or disable this barcode type.
        /// </summary>
        public bool isEnabled { get; set; }

        /// <summary>
        /// Binding used to hide barcode types that are not enabled. Should be set on the grid or stack panel of the DataTemplate for ListBox or ListPicker control.
        /// </summary>
        public Visibility Visible { get { return isEnabled ? Visibility.Visible : Visibility.Collapsed; } }
        
        /// <summary>
        /// The BarcodeFormat as defined by ZXing
        /// </summary>
        public com.google.zxing.BarcodeFormat BarcodeType { get; set; }

    }

    /// <summary>
    /// Initializes the list of BarcodeTypes and can be used as the datacontext for ListPicker or ListBox
    /// </summary>
    public class BarcodeTypeManager
    {
        static private List<BarcodeTypeItem> lstBarcodeTypeItems = new List<BarcodeTypeItem>();
        static BarcodeTypeManager()
        {
            lstBarcodeTypeItems = new List<BarcodeTypeItem>(){
                new BarcodeTypeItem(){Name="UPC or EAN", isEnabled=true, Filename = "Icons/Type_UPC-A.png", BarcodeType = com.google.zxing.BarcodeFormat.UPC_EAN},
                new BarcodeTypeItem(){Name="All 1D Types", isEnabled=true, Filename = "Icons/Type_Code-128.png", BarcodeType = com.google.zxing.BarcodeFormat.ALL_1D},
                
                new BarcodeTypeItem(){Name="QR Code", isEnabled=true, Filename = "Icons/Type_QRCode.png", BarcodeType = com.google.zxing.BarcodeFormat.QR_CODE},
                new BarcodeTypeItem(){Name="Data Matrix", isEnabled=true, Filename = "Icons/Type_DataMatrix.png", BarcodeType = com.google.zxing.BarcodeFormat.DATAMATRIX},
                new BarcodeTypeItem(){Name="UPC-A", isEnabled=true, Filename = "Icons/Type_UPC-A.png", BarcodeType = com.google.zxing.BarcodeFormat.UPC_A}, //May want to remove this or use com.google.zxing.BarcodeFormat.UPC_EAN instead for better scanning results
                new BarcodeTypeItem(){Name="UPC-E", isEnabled=true, Filename = "Icons/Type_UPC-E.png", BarcodeType = com.google.zxing.BarcodeFormat.UPC_E}, //May want to remove this or use com.google.zxing.BarcodeFormat.UPC_EAN instead for better scanning results
                new BarcodeTypeItem(){Name="EAN-8", isEnabled=true, Filename = "Icons/Type_EAN-8.png", BarcodeType = com.google.zxing.BarcodeFormat.EAN_8}, //May want to remove this or use com.google.zxing.BarcodeFormat.UPC_EAN instead for better scanning results
                new BarcodeTypeItem(){Name="EAN-13", isEnabled=true, Filename = "Icons/Type_EAN-13.png", BarcodeType = com.google.zxing.BarcodeFormat.EAN_13}, //May want to remove this or use com.google.zxing.BarcodeFormat.UPC_EAN instead for better scanning results
                new BarcodeTypeItem(){Name="Code 39", isEnabled=true, Filename = "Icons/Type_Code-39.png", BarcodeType = com.google.zxing.BarcodeFormat.CODE_39 },
                new BarcodeTypeItem(){Name="Code 128", isEnabled=true, Filename = "Icons/Type_Code-128.png", BarcodeType = com.google.zxing.BarcodeFormat.CODE_128},
                new BarcodeTypeItem(){Name="PDF417", isEnabled=false, Filename = "Icons/Type_PDF417.png", BarcodeType = com.google.zxing.BarcodeFormat.PDF417},
                new BarcodeTypeItem(){Name="ITF", isEnabled=true, Filename = "Icons/Type_ITF-14.png", BarcodeType = com.google.zxing.BarcodeFormat.ITF}
            };
        }

        public BarcodeTypeManager()
        {
            BarcodeTypeManager.Current = this; //Stores reference to the current instance
        }

        /// <summary>
        /// Used to access the current instance of this class.
        /// </summary>
        public static BarcodeTypeManager Current { get; set; }

        /// <summary>
        /// List of BarcodeTypes that can be used as the data context for ListPicker or ListBox controls
        /// </summary>
        public List<BarcodeTypeItem> BarcodeTypes { get { return lstBarcodeTypeItems; } }
    }
}

