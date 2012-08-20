namespace PersonalInventory.Model
{
    public class ProductsRepository
    {
        public static void Initialize()
        {
            using (var dc = new ProductsDataContext(ProductsDataContext.ConnectionString))
            {
                //dc.DeleteDatabase();
                if (dc.DatabaseExists() == false)
                {
                    dc.CreateDatabase();

                    dc.SubmitChanges();
                }
            }
        }
    }
}
