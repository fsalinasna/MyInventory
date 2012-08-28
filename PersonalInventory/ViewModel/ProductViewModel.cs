using System;
using PersonalInventory.Model;

namespace PersonalInventory.ViewModel
{
    public class ProductViewModel
    {
        public string ProductName { get; set; }

        public string ImageUrl { get; set; }

        public string Barcode { get; set; }

        public decimal Price { get; set; }

        public string Currency { get; set; }

        public int Quantity { get; set; }

        public DateTime DatePurchased { get; set; }

        public ProductViewModel(SearchUpcItemInfo itemInfo)
        {
            ProductName = itemInfo.ProductName;
            ImageUrl = itemInfo.ImageUrl;

            Price = ConvertToPrice(itemInfo);

            Currency = GetCurrencySymbol(itemInfo.Currency);

            Quantity = 1;

            DatePurchased = DateTime.UtcNow;
        }

        public ProductViewModel(Product product)
        {
            ImageUrl = product.ImageUrl;
            ProductName = product.ProductName;
        }

        public ProductViewModel()
        {
        }

        public static decimal ConvertToPrice(SearchUpcItemInfo itemInfo)
        {
            decimal price;

            return decimal.TryParse(itemInfo.Price, out price) ? price : default(decimal);
        }

        public static string GetCurrencySymbol(string currency)
        {
            string symbol;
            switch (currency.ToLower())
            {
                case "usd":
                    symbol = "$";
                    break;
                default:
                    symbol = "";
                    break;
            }

            return symbol;
        }
    }
}
