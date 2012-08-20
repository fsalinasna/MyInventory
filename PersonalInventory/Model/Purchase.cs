using System;
using System.Data.Linq;
using System.Data.Linq.Mapping;

namespace PersonalInventory.Model
{
    [Table(Name = "Purchases")]
    public class Purchase
    {
        [Column(IsPrimaryKey = true, IsDbGenerated = true, DbType = "INT NOT NULL Identity", CanBeNull = false, AutoSync = AutoSync.OnInsert)]
        public int Id { get; set; }

        [Column]
        public int ProductId { get; set; }

        [Column]
        public decimal Price { get; set; }

        [Column]
        public string Currency { get; set; }

        [Column]
        public int Quantity { get; set; }

        [Column]
        public DateTime DatePurchased { get; set; }

        private EntityRef<Product> _productRef;

        [Association(Name = "FK_Product_Purchases", Storage = "_productRef", ThisKey = "ProductId", OtherKey = "Id", IsForeignKey = true)]
        public Product Product
        {
            get { return _productRef.Entity; }

            set
            {
                Product previousValue = _productRef.Entity;

                if (previousValue != value || _productRef.HasLoadedOrAssignedValue == false)
                {
                    if (previousValue != null)
                    {
                        _productRef.Entity = null;

                        previousValue.Purchases.Remove(this);
                    }

                    _productRef.Entity = value;

                    if (value != null)
                    {
                        value.Purchases.Add(this);
                        ProductId = value.Id;
                    }
                    else
                    {
                        ProductId = default(int);
                    }
                }
            }
        }
    }
}