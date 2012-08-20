using System.Data.Linq;

namespace PersonalInventory.Model
{
    public class ProductsDataContext : DataContext
    {
        public const string ConnectionString = "isostore:/products.sdf";

        public ProductsDataContext(string connectionString)
            : base(connectionString)
        {
            Products = GetTable<Product>();
            Purchases = GetTable<Purchase>();
        }

        public Table<Product> Products { get; set; }
        public Table<Purchase> Purchases { get; set; }
    }
}