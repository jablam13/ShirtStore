using StoreModel.Store;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace StoreService.Interface
{
    public interface ICartService
    {
        Task<Cart> GetCartAll(Guid visitorUid, Guid userUid);
        Task<Cart> GetCart(Guid visitorUid, Guid userUid);
        Task<List<CartItem>> GetCartItems(Guid cartUid);
        Task<List<CartItem>> GetCartItems(Guid visitorUid, Guid userUid);
        Task<CartItem> AddCartItem(StoreItem item, Guid visitorUid, Guid userUid);
        Task<CartItem> EditCartItem(CartItem item, Guid visitorUid, Guid userUid);
        Task<Guid> RemoveCartItem(Guid cartItemUid);
        Task<Cart> MergeCarts(Guid visitorUid, Guid userUid);
    }
}
