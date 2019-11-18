using StoreModel.Account;
using StoreModel.Checkout;
using StoreRepository.Interface;
using StoreService.Interface;
using System;
using System.Collections.Generic;
using System.Text;

namespace StoreService
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository orderRep;
        private readonly IAddressService addressServ;

        public OrderService(
            IOrderRepository _orderRep,
            IAddressService _addressServ
            )
        {
            orderRep = _orderRep;
            addressServ = _addressServ;
        }

        public OrderResult ProcessOrder(Order order)
        {
            OrderResult orderResult = new OrderResult();
            try
            {

                //process billing address
                AddAddresses(order.ShippingAddress, order.BillingAddress);


                //create order in database


                //process payment through braintree


                //save braintree order result in db


            }
            catch (Exception ex)
            {

            }

            return orderResult;
        }

        private bool AddAddresses(UserAddress shipping, UserAddress billing)
        {
            bool success = false;
            try
            {
                addressServ.AddAddress(shipping);
                success = true;
            }
            catch (Exception ex)
            {

            }

            try
            {
                addressServ.AddAddress(shipping);
                success = true;
            }
            catch (Exception ex)
            {

            }
            return success;
        }

    }
}
