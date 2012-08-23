using System;
using System.Linq;
using System.Windows;
using System.Windows.Media.Imaging;
using PersonalInventory.Model;
using PersonalInventory.ViewModel;

namespace PersonalInventory
{
    public partial class EditProduct
    {
        public EditProduct()
        {
            InitializeComponent();
        }

         protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
         {
             base.OnNavigatedTo(e);

             string productIdStr;

             if (NavigationContext.QueryString.TryGetValue("productid", out productIdStr))
             {
                 btnSave.IsEnabled = false;

                 GetProduct(int.Parse(productIdStr));
             }

         }

         private void GetProduct(int productId)
         {
             ProductViewModel productInfo = null;

             using (var dc = new ProductsDataContext(ProductsDataContext.ConnectionString))
             {
                 var product = (from p in dc.Products
                                where p.Id == productId
                                select p).FirstOrDefault();

                 if (product != null)
                 {
                     productInfo = new ProductViewModel(product);
                 }

             }

             if (productInfo != null)
             {

                 txtName.Text = productInfo.ProductName;

                 imgProductImage.Source = new BitmapImage(new Uri(productInfo.ImageUrl, UriKind.Absolute));

                 btnSave.IsEnabled = true;
             }
         }

         private void SaveButtonClick(object sender, RoutedEventArgs e)
         {
         }

         private void CancelButtonClick(object sender, RoutedEventArgs e)
         {
             // Navigate to main page.
             NavigationService.Navigate(new Uri("/MainPage.xaml", UriKind.RelativeOrAbsolute));
         }
    }
}