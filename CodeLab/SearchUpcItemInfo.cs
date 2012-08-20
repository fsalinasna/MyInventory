using System.Runtime.Serialization;

namespace CodeLab
{
    [DataContract]
    public class SearchUpcItemInfo
    {
        [DataMember(Name = "productname")]
        public string ProductName { get; set; }

        [DataMember(Name = "imageurl")]
        public string ImageUrl { get; set; }

        [DataMember(Name = "producturl")]
        public string ProductUrl { get; set; }

        [DataMember(Name = "price")]
        public string Price { get; set; }

        [DataMember(Name = "currency")]
        public string Currency { get; set; }

        [DataMember(Name = "saleprice")]
        public string SalePrice { get; set; }

        [DataMember(Name = "storename")]
        public string StoreName { get; set; }

    }
}
