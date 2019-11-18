using System;
using System.Collections.Generic;
using System.Text;
using StoreModel.Checkout;

namespace StoreRepository.Interface
{
    public interface IOrderRepository
    {
        Order CreateOrder(Order order);
        List<OrderItem> CreateOrderItems(List<OrderItem> items);
        void ProcessOrder(Order order);
    }
}
