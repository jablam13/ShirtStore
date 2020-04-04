using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using StoreModel.Checkout;

namespace StoreRepository.Interface
{
    public interface IOrderRepository
    {
        Task<Order> CreateOrder(Guid userUid, string ipAddress);
        Task<List<OrderItem>> CreateOrderItemsFromCart(Guid userUid, int orderId);
        Task<Order> GetOrderAll(Guid userUid, int statusId);
        Task<Order> GetOrder(Guid userUid, int statusId);
        Task<List<OrderItem>> GetOrderItems(int orderId);
        Task<Order> InitProcessOrder(Checkout checkout, Guid userUid);
    }
}
