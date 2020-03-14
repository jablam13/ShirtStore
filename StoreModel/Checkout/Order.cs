using StoreModel.Account;
using System;
using System.Collections.Generic;
using System.Text;

namespace StoreModel.Checkout
{
    public sealed class Order
    {
        public int Id { get; set; }
        public Guid Uid { get; set; }
        public int UserId { get; set; }
        public int ShippingId { get; set; }
        public int BillingId { get; set; }
        public string Email { get; set; }
        public int OrderStateId { get; set; }
        public DateTime OrderDate { get; set; }
        public string TokenId { get; set; }
        public string CardAuth { get; set; }
        public string CardType { get; set; }
        public decimal SubTotal { get; set; }
        public decimal Tax { get; set; }
        public decimal ShippingCost { get; set; }
        public decimal Discount { get; set; }
        public decimal Total { get; set; }
        public string IPAddress { get; set; }
        public List<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
        public UserAddress BillingAddress { get; set; } = new UserAddress();
        public UserAddress ShippingAddress { get; set; } = new UserAddress();
    }
}
