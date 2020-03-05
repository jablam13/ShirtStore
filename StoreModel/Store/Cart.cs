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
        public Guid VisitorUid { get; set; }
        public Guid UserUid { get; set; }
        public Guid CartUid { get; set; }
        public int UserId { get; set; }
        public decimal TotalPrice { get; set; }
        public int Quantity { get; set; }
        public List<CartItem> CartItems { get; set; } = new List<CartItem>();
        public DateTime LastModifiedDate { get; set; }
        public List<string> Errors { get; set; }

        public decimal ComputeTotalValue()
        {
            return CartItems.Sum(e => e.Price * e.Quantity);
        }
    }
}
