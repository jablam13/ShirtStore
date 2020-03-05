using StoreModel.Store;
using System;
using System.Collections.Generic;

namespace StoreRepository.Interface
{
    public interface ICartRepository
    {
        Cart GetCartAll(Guid visitorUid, Guid userUid);
        Cart GetCart(Guid visitorUid, Guid userUid);
        Cart GetVisitorCart(Guid visitorUid);
        Cart GetUserCart(Guid userUid);
        List<CartItem> GetCartItems(Guid cartUid);
        List<CartItem> GetCartItems(int cartId);
        CartItem AddCartItem(StoreItem cart, int cartId);
        Cart UpdateCart(Cart cart);
        CartItem EditCartItem(Cart cart);
        bool RemoveCart(Cart cart);
        bool RemoveCartItem(CartItem cart);
        Cart MergeCarts(Guid userUid, Guid visitorUid);
        bool CartExists(Guid userUid, Guid visitorUid);
    }
}
