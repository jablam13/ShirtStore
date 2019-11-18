using StoreModel.Store;
using System;
using System.Collections.Generic;
using System.Text;

namespace StoreService.Interface
{
    public interface ICartService
    {
        Cart GetCart(Guid userUid);
        Cart GetCart(Guid? cartUid, Guid? userUid);
        Cart AddCart(Cart cart);
        Cart AddCartItem(StoreItem cart);
        Cart EditCart(Cart cart);
        Cart EditCartItem(Cart cart);
        bool RemoveCart(Cart cart);
        bool RemoveCartItem(Cart cart);
    }
}
