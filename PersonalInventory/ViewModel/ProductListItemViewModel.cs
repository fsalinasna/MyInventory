using System;

namespace PersonalInventory.ViewModel
{
    public class ProductListItemViewModel
    {
        public int Id { get; set; }

        public string ProductName { get; set; }

        public string PurchaseDate { get; set; }

        public string ImageUrl { get; set; }

        public int Quantity { get; set; }

        public ProductListItemViewModel(int productId, string productName, DateTime purchaseDate, string imageUrl, int quantity)
        {
            Id = productId;
            ProductName = productName.Substring(0, Math.Min(productName.Length, 75));
            PurchaseDate = purchaseDate.ToShortDateString();
            ImageUrl = imageUrl;
            Quantity = quantity;
        }
    }
}
