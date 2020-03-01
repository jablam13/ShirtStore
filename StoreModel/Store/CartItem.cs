using System;
using System.Collections.Generic;
using System.Text;

namespace StoreModel.Store
{
    public class CartItem
    {
        public int Id { get; set; }
        public Guid Uid { get; set; }
        public int CartId { get; set; }
        public int ItemId { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public string LargeImg { get; set; }
        public string SmallImg { get; set; }
        public int Active { get; set; }
        public int Status { get; set; }
        public string CartTransaction { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime LastModifiedDate { get; set; }
    }
}
