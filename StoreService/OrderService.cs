using StoreModel.Account;
using StoreModel.Checkout;
using StoreModel.Store;
using StoreRepository.Interface;
using StoreService.Interface;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace StoreService
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository orderRep;
        private readonly ICartRepository cartRep;
        private readonly IAddressRepository addressRep;
        private readonly IAddressService addressService;

        public OrderService(
            IOrderRepository _orderRep,
            ICartRepository _cartRep,
            IAddressRepository _addressRep,
            IAddressService _addressService
            )
        {
            orderRep = _orderRep;
            addressRep = _addressRep;
            cartRep = _cartRep;
            addressService = _addressService;
        }

        public async Task<Order> CreateOrder(Guid userUid, string ipAddress)
        {
            //check if there are items in the cart
            var cartItemCount = await cartRep.CartItemCount(userUid).ConfigureAwait(false);
            if (cartItemCount <= 0)
                throw new NullReferenceException("Error getting cart items");

            //insert into order table
            var order = await orderRep.CreateOrder(userUid, ipAddress).ConfigureAwait(false);

            if(order == null)
                throw new NullReferenceException("Error Creating Order");

            //insert cart items in order items table
            order.OrderItems = await orderRep.CreateOrderItemsFromCart(userUid, order.Id).ConfigureAwait(false);
            
            if(order.OrderItems.Count == 0)
                throw new NullReferenceException("Error Creating Order Items");

            //return success
            return order;
        }

        public async Task<Order> GetOrder(Guid userUid, int orderState)
        {
            try
            {
                //get order by userUid and pending
                var order = await orderRep.GetOrder(userUid, orderState).ConfigureAwait(false);

                //set user uid, address type in the addresses
                order.ShippingAddress.UserUid = userUid;
                order.BillingAddress.UserUid = userUid;

                var orderItemsTask = orderRep.GetOrderItems(order.Id);
                var shippingAddressTask = addressService.CreateAddress(order.ShippingAddress);
                var billingAddressTask = addressService.CreateAddress(order.BillingAddress);

                await Task.WhenAll(orderItemsTask, shippingAddressTask, billingAddressTask).ConfigureAwait(false);

                order.OrderItems = await orderItemsTask.ConfigureAwait(false);
                order.ShippingAddress = await shippingAddressTask.ConfigureAwait(false);
                order.BillingAddress = await billingAddressTask.ConfigureAwait(false);

                return order;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task ProcessOrder(Checkout checkout, Guid userUid)
        {
            var order = await orderRep.InitProcessOrder(checkout, userUid).ConfigureAwait(false);

            order.ShippingAddress.UserUid = userUid;
            order.BillingAddress.UserUid = userUid;

            var orderItemsTask = orderRep.GetOrderItems(order.Id);
            var shippingAddressTask = addressService.CreateAddress(order.ShippingAddress);
            var billingAddressTask = addressService.CreateAddress(order.BillingAddress);

            await Task.WhenAll(orderItemsTask, shippingAddressTask, billingAddressTask).ConfigureAwait(false);

            order.OrderItems = await orderItemsTask.ConfigureAwait(false);
            order.ShippingAddress = await shippingAddressTask.ConfigureAwait(false);
            order.BillingAddress = await billingAddressTask.ConfigureAwait(false);


        }
    }
}
