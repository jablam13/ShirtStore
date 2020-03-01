using StoreModel.Store;
using System;
using System.Collections.Generic;

namespace StoreRepository.Interface
{
    public interface ICartRepository
    {
        Cart GetCartAll(Guid uid, bool isVisitor);
        Cart GetCart(Guid uid, bool isVisitor);
        List<CartItem> GetCartItems(Guid cartUid);
        List<CartItem> GetCartItems(Guid uid, bool isVisitor);
        Cart CreateCart(Guid? userUid = null, Guid? visitorUid = null);
        CartItem AddCartItem(StoreItem cart, int cartId);
        Cart EditCart(Cart cart);
        CartItem EditCartItem(Cart cart);
        bool RemoveCart(Cart cart);
        bool RemoveCartItem(CartItem cart);
        Cart MergeCarts(Guid userUid, Guid visitorUid);
        bool CartExists(Guid userUid, Guid visitorUid);
    }
}
