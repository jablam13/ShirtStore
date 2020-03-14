using StoreModel.Store;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StoreRepository.Interface
{
    public interface ICartRepository
    {
        Task<Cart> GetCartAll(Guid visitorUid, Guid userUid);
        Task<Cart> GetCart(Guid visitorUid, Guid userUid);
        Task<Cart> GetVisitorCart(Guid visitorUid);
        Task<Cart> GetUserCart(Guid userUid);
        Task<List<CartItem>> GetCartItems(Guid cartUid);
        Task<List<CartItem>> GetCartItems(int cartId);
        Task<CartItem> AddCartItem(StoreItem cart, int cartId);
        Task<CartItem> EditCartItem(CartItem cart);
        Task<Guid> RemoveCartItem(Guid cartItemUid);
        Task<bool> CartExists(Guid userUid, Guid visitorUid);
        Task<Cart> UpdateCart(Cart cart, Guid visitorUid, Guid userUid);
        Task<Cart> UpdateCartAndItems(Cart userCart);
        Task<Cart> UpdateCart(Cart userCart);
        Task<CartItem> UpdateCartItem(CartItem userCart);
        Task<bool> DeactivateVisitorCart(Guid uid);
        Task<int> CartItemCount(Guid userUid);
    }
}
