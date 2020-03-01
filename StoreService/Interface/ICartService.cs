using StoreModel.Store;
using System;
using System.Collections.Generic;
using System.Text;

namespace StoreService.Interface
{
    public interface ICartService
    {
        Cart GetCartAll(Guid userUid, Guid visitorUid);
        Cart GetCart(Guid userUid, Guid visitorUid);
        List<CartItem> GetCartItems(Guid cartUid);
        List<CartItem> GetCartItems(Guid uid, bool isVisitor);
        Cart CreateCart(Guid userUid, Guid visitorUid);
        CartItem AddCartItem(StoreItem cart);
        Cart EditCart(Cart cart);
        CartItem EditCartItem(Cart cart);
        bool RemoveCart(Cart cart);
        bool RemoveCartItem(CartItem cart);
        Cart MergeCarts(Guid userUid, Guid visitorUid);
    }
}
