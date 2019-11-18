using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StoreModel.Store
{
    public class Cart
    {
        public int Id { get; set; }
        public Guid Uid { get; set; }
        public decimal TotalPrice { get; set; }
        public int Quantity { get; set; }
        public List<StoreItem> CartItems { get; set; }
        public DateTime LastModifiedDate { get; set; }
        public List<string> Errors { get; set; }
        public decimal ComputeTotalValue()
        {
            return CartItems.Sum(e => e.Price * e.Quantity);
        }
    }
}
