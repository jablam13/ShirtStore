using StoreModel.Account;
using StoreModel.Store;
using System;
using System.Collections.Generic;
using System.Text;

namespace StoreModel.Checkout
{
    public class Checkout
    {
        public UserAddress ShippingAddress { get; set; }
        public UserAddress BillingAddress { get; set; }
        public List<OrderItem> OrderItems { get; set; }
    }

    public class CheckoutTest
    {
        public string EmailAddress { get; set; }
        public string PhoneNumber { get; set; }
        public string BraintreeClientToken { get; set; }
        public string BraintreeNonce { get; set; }
        public decimal Amount { get; set; }
        public UserAddress ShippingAddress { get; set; }
    }
}
