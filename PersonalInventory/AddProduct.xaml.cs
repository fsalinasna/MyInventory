using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using Microsoft.Phone;
using Microsoft.Phone.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PersonalInventory.Model;
using PersonalInventory.UPCSearch;
using PersonalInventory.ViewModel;

namespace PersonalInventory
{
    public partial class AddProduct
    {
        private const String SEARCHUPC_ACCESS_TOKEN = "66289885-CC47-4E64-97D1-D794DBB3F9EB";

        private string _scannedBarcode;

        private ProductViewModel _currentProduct;

        private WriteableBitmap _currentImage;
        private CameraCaptureTask _task;

        public AddProduct()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            
            if (NavigationContext.QueryString.TryGetValue("barcode", out _scannedBarcode))
            {
                btnAddToInventory.IsEnabled = false;

                var searchUpcClient = new UPCSearchSoapClient();

                searchUpcClient.GetProductJSONCompleted += GetProductCompleted;

                searchUpcClient.GetProductJSONAsync(_scannedBarcode, SEARCHUPC_ACCESS_TOKEN);
                btnAddImage.Visibility = Visibility.Collapsed;
                imgProductImage.Visibility = Visibility.Visible;
            }
            else
            {
                btnAddToInventory.IsEnabled = true;

                if (_currentProduct == null || string.IsNullOrEmpty(_currentProduct.ImageUrl))
                {
                    btnAddImage.Visibility = Visibility.Visible;
                    imgProductImage.Visibility = Visibility.Collapsed;
                }
                else
                {
                    //imgProductImage.Source = new BitmapImage(new Uri(_currentProduct.ImageUrl, UriKind.Absolute));
                    imgProductImage.Source = _currentImage;
                    btnAddImage.Visibility = Visibility.Collapsed;
                    btnAddImage.Content = "";
                    imgProductImage.Visibility = Visibility.Visible;
                }

                txtQuantity.Text = "1";
                txtCurrency.Text = ProductViewModel.GetCurrencySymbol("usd");
                txtDate.Text = DateTime.Now.ToShortDateString();
            }

        }

        private void GetProductCompleted(object sender, GetProductJSONCompletedEventArgs e)
        {
            var js = new JsonSerializer();

            SearchUpcItemInfo p = null;
            foreach (var token in JObject.Parse(e.Result).Values())
            {
                p = js.Deserialize<SearchUpcItemInfo>(token.CreateReader());
                if (p != null)
                {
                    break;
                }
            }

            if (p != null)
            {
                _currentProduct = new ProductViewModel(p);

                txtName.Text = p.ProductName;
                txtPrice.Text = p.Price;
                txtQuantity.Text = "1";
                txtCurrency.Text = ProductViewModel.GetCurrencySymbol(p.Currency);

                txtDate.Text = DateTime.Now.ToShortDateString();
                
                imgProductImage.Source = new BitmapImage(new Uri(p.ImageUrl, UriKind.Absolute));

            }
            else
            {
                //
                // TODO: Add notification saying that no info could be found.
                // leave all controls blank, set imgProductImage to the default image
                // to allow for manual entry.
            }

            btnAddToInventory.IsEnabled = true;

        }

        private void AddToInventoryClick(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(txtName.Text))
            {
                // TODO: Add notification error saying product name cannot be empty
                return;
            }



            try
            {
                using (var dc = new ProductsDataContext(ProductsDataContext.ConnectionString))
                {
                    var existingProduct = (from p in dc.Products
                                           where p.Barcode == _scannedBarcode || p.ProductName == txtName.Text
                                           select p).FirstOrDefault();

                    Product product;

                    if (existingProduct == null)
                    {
                        product = new Product
                        {
                            ProductName = txtName.Text,
                            Barcode = _scannedBarcode,
                            ImageUrl = _currentProduct.ImageUrl,
                        };

                        dc.Products.InsertOnSubmit(product);
                        dc.SubmitChanges();
                    }
                    else
                    {
                        product = existingProduct;
                    }

                    var purchase = new Purchase
                        {
                            ProductId = product.Id,
                            Currency = txtCurrency.Text,
                            DatePurchased = DateTime.Parse(txtDate.Text),
                            Quantity = int.Parse(txtQuantity.Text),
                            Price = decimal.Parse(txtPrice.Text)
                        };

                    dc.Purchases.InsertOnSubmit(purchase);
                    dc.SubmitChanges();
                }
                // Navigate to main page.
                NavigationService.Navigate(new Uri("/MainPage.xaml", UriKind.RelativeOrAbsolute));
            }
            catch (Exception ex)
            {
                txtName.Text = ex.Message;
            }

        }

        private void CancelButtonClick(object sender, RoutedEventArgs e)
        {
            // Navigate to main page.
            NavigationService.Navigate(new Uri("/MainPage.xaml", UriKind.RelativeOrAbsolute));
        }

        protected override void OnNavigatedFrom(System.Windows.Navigation.NavigationEventArgs e)
        {
            PreserveFocusState(txtName);
        }

        private void PreserveFocusState(FrameworkElement parent)
        {

            Control focusedControl = FocusManager.GetFocusedElement() as Control;

            if (focusedControl == null)
            {
                
            }
            else
            {
                Control controlWithFocus = parent.FindName(focusedControl.Name) as Control;

                if (controlWithFocus == null)
                {
                    
                }
                else
                {
                    State["FocusedElementName"] = controlWithFocus.Name;
                }
            }

        }

        private void AddImageClick(object sender, RoutedEventArgs e)
        {
            _task = new CameraCaptureTask();
            _task.Completed += CaptureImageTaskCompleted;
            _task.Show();
        }

        private void CaptureImageTaskCompleted(object sender, PhotoResult e)
        {
            try
            {
                if (e.TaskResult == TaskResult.OK)
                {
                    _currentImage = PictureDecoder.DecodeJpeg(e.ChosenPhoto);

                    if (_currentProduct == null)
                    {
                        _currentProduct = new ProductViewModel();
                    }

                    _currentProduct.ImageUrl = e.OriginalFileName;

                }
            }
            catch (Exception ex)
            {
                txtName.Text = ex.Message + Environment.NewLine + e.Error.Message + Environment.NewLine + e.OriginalFileName; 
            }
        }

    }
}
