using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using PersonalInventory.Model;
using PersonalInventory.ViewModel;
using WP7_Barcode_Library;
using com.google.zxing;

namespace PersonalInventory
{
    public partial class MainPage
    {

        // Constructor
        public MainPage()
        {
            InitializeComponent();

            Loaded += MainPageLoaded;
        }

        private void MainPageLoaded(object sender, RoutedEventArgs e)
        {
            List<ProductListItemViewModel> productList;

            try
            {
                using (var dc = new ProductsDataContext(ProductsDataContext.ConnectionString))
                {
                    productList = (from product in dc.Products
                                   join purchase in dc.Purchases on product.Id equals purchase.ProductId
                                   group purchase by new { product.Id, product.ProductName, product.ImageUrl } into psummary
                                   orderby psummary.Max(pur => pur.DatePurchased) descending
                                   select new ProductListItemViewModel(psummary.Key.Id, psummary.Key.ProductName, psummary.Max(pur => pur.DatePurchased), psummary.Key.ImageUrl, psummary.Sum(pur => pur.Quantity))

                                   ).ToList();
                }

            }
            catch (Exception ex)
            {
                productList = new List<ProductListItemViewModel>();
                txtResults.Text = ex.Message;
            }

            ProductList.ItemsSource = productList;
        }

        /// <summary>
        /// Callback method that processes results returned by the WP7BarcodeManager. 
        /// Results are also stored at WP7BarcodeManager.LastCaptureResults.
        /// </summary>
        /// <param name="results">Object that holds all the results of processing the barcode. 
        /// Results are also stored at WP7BarcodeManager.LastCaptureResults.</param>
        public void BarcodeResults(BarcodeCaptureResult results)
        {
            StopProgress();

            if (Microsoft.Devices.Environment.DeviceType == Microsoft.Devices.DeviceType.Emulator)
            {
                NavigationService.Navigate(new Uri("/AddProduct.xaml?barcode=" + "020356363307",
                                   UriKind.RelativeOrAbsolute));
            }
            else
            {


                if (results.State == CaptureState.Success)
                {
                    txtResults.Text = results.BarcodeText;
                    txtResults.Text += "  Processing...";

                    NavigationService.Navigate(new Uri("/AddProduct.xaml?barcode=" + results.BarcodeText,
                                                       UriKind.RelativeOrAbsolute));
                }
                else if (results.State == CaptureState.Canceled)
                {
                    txtResults.Text = "Cancelled";
                    NavigationService.Navigate(new Uri("/AddProduct.xaml?barcode=" + "020356363307",
                   UriKind.RelativeOrAbsolute));

                }
                else
                {
                    txtResults.Text = results.ErrorMessage;
                }
            }

        }

        public void StartProgress()
        {
        }

        public void StopProgress()
        {
        }

        private void PhoneApplicationPageLoaded(object sender, RoutedEventArgs e)
        {
            WP7BarcodeManager.aStartProgress = StartProgress;
            WP7BarcodeManager.ScanMode = BarcodeFormat.ALL_1D;
        }

        /// <summary>
        /// Loads the camera, lets the user take a picture, and then returns the result to a callback method.
        /// </summary>
        private void ScanClick(object sender, RoutedEventArgs e)
        {
            StartProgress();
            WP7BarcodeManager.ScanBarcode(BarcodeResults);
        }

        private void SearchClick(object sender, RoutedEventArgs e)
        {
            string searchTerm = SearchBox.Text;

            List<ProductListItemViewModel> productList;

            try
            {
                using (var dc = new ProductsDataContext(ProductsDataContext.ConnectionString))
                {
                    productList = (from product in dc.Products
                                   join purchase in dc.Purchases on product.Id equals purchase.ProductId
                                   group purchase by new {product.Id, product.ProductName, product.ImageUrl}
                                   into psummary
                                   where psummary.Key.ProductName.Contains(searchTerm)
                                   orderby psummary.Max(pur => pur.DatePurchased) descending
                                   select
                                       new ProductListItemViewModel(psummary.Key.Id, psummary.Key.ProductName,
                                                                    psummary.Max(pur => pur.DatePurchased),
                                                                    psummary.Key.ImageUrl,
                                                                    psummary.Sum(pur => pur.Quantity))
                                  ).ToList();
                }
            }
            catch (Exception ex)
            {
                productList = new List<ProductListItemViewModel>();
                txtResults.Text = ex.Message;
            }

            ProductList.ItemsSource = productList;
        }

        private void ProductListSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var listBox = sender as ListBox;
            if (listBox != null)
            {
                var productViewModel = listBox.SelectedItem as ProductListItemViewModel;

                if (productViewModel != null)
                    NavigationService.Navigate(new Uri("/EditProduct.xaml?productid=" + productViewModel.Id,
                                                       UriKind.RelativeOrAbsolute));
            }
        }
    }
}