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
        public Guid CartUid { get; set; }
        public int ItemId { get; set; }
        public Guid ItemUid { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public decimal SubTotal { get; set; }
        public decimal Tax { get; set; }
        public decimal ShippingCost { get; set; }
        public decimal Discount { get; set; }
        public int Quantity { get; set; }
        public int UpdatedQuantity { get; set; }
        public string LargeImg { get; set; }
        public string SmallImg { get; set; }
        public int Active { get; set; }
        public int Status { get; set; }
        public string CartTransaction { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime LastModifiedDate { get; set; }
    }
}
