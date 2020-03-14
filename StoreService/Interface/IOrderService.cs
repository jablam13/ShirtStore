using StoreModel.Checkout;
using StoreModel.Store;
using System;
using System.Threading.Tasks;

namespace StoreService.Interface
{
    public interface IOrderService
    {
        Task<Order> CreateOrder(Guid userUid, string ipAddress);
        Task<Order> GetOrder(Guid userUid, int orderState);
        Task ProcessOrder(Order order);
    }
}
