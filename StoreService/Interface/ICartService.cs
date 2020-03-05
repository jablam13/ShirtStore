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
        List<CartItem> GetCartItems(Guid userUid, Guid visitorUid);
        CartItem AddCartItem(StoreItem item, Guid userUid, Guid visitorUid);
        CartItem EditCartItem(StoreItem item, Guid userUid, Guid visitorUid);
        bool RemoveCartItem(CartItem cart);
        Cart MergeCarts(Guid userUid, Guid visitorUid);
    }
}
