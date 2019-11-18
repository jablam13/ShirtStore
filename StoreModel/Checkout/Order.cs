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
        public string BraintreeNonce { get; set; }
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
        public string LastFourDigits { get; set; }
        public string IpAddress { get; set; }
        public string ProcessorResponseCode { get; set; }
        public string ProcessorResponseText { get; set; }
        public string CvvResponseCode { get; set; }
        public string AvsStreetAddressResponseCode { get; set; }
        public string AvsPostalCodeResponseCode { get; set; }
        public string AvsErrorResponseCode { get; set; }
        public List<OrderItem> OrderItems { get; set; }
        public UserAddress BillingAddress { get; set; }
        public UserAddress ShippingAddress { get; set; }
    }
}
