using System.Data.Linq;
using System.Data.Linq.Mapping;

namespace PersonalInventory.Model
{
    [Table(Name = "Products")]
    public class Product
    {
        [Column(IsPrimaryKey = true, IsDbGenerated = true, DbType = "INT NOT NULL Identity", CanBeNull = false, AutoSync = AutoSync.OnInsert)]
        public int Id { get; set; }

        [Column]
        public string ProductName { get; set; }

        [Column]
        public string ImageUrl { get; set; }

        [Column]
        public string Barcode { get; set; }

        private readonly EntitySet<Purchase> _purchaseRef = new EntitySet<Purchase>();

        [Association(Name = "FK_Product_Purchases", Storage = "_purchaseRef", ThisKey = "Id", OtherKey = "ProductId")]
        public EntitySet<Purchase> Purchases
        {
            get { return _purchaseRef; }
        }

    }
}
