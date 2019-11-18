using System;
using System.Collections.Generic;
using System.Text;

namespace StoreModel.Checkout
{
    public sealed class OrderItem
    {
        public int Id { get; set; }
        public Guid Uid { get; set; }
        public int OrderId { get; set; }
        public Guid StoreItemInfoUid { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public decimal Tax { get; set; }
        public decimal ShippingCost { get; set; }
        public decimal Discount { get; set; }
        public string Tracking { get; set; }
        public bool IsBackorder { get; set; }
    }
}
