using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
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

                btnAddToInventory.IsEnabled = true;
            }
            else
            {
                //
                // TODO: Add notification saying that no info could be found.
                // leave all controls blank, set imgProductImage to the default image
                // to allow for manual entry.
            }
        }

        private void AddToInventoryClick(object sender, System.Windows.RoutedEventArgs e)
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
            PreserveFocusState(txtCurrency);
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


    }
}
