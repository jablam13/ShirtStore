using StoreModel.Store;
using StoreService.Interface;
using System;
using System.Collections.Generic;
using System.Text;

namespace StoreService
{
    public class CartService : BaseService, ICartService
    {
        public CartService()
        {

        }

        public Cart GetCart(Guid userUid)
        {
            return new Cart();
        }

        public Cart GetCart(Guid? userUid, Guid? cartUid)
        {
            return new Cart();
        }

        public Cart AddCart(Cart cart)
        {
            return new Cart();
        }

        public Cart AddCartItem(StoreItem cart)
        {
            return new Cart();
        }

        public Cart EditCart(Cart cart)
        {
            return new Cart();
        }
        public Cart EditCartItem(Cart cart)
        {
            return new Cart();
        }
        public bool RemoveCart(Cart cart)
        {
            bool success = false;

            return success;
        }
        public bool RemoveCartItem(Cart cart)
        {
            bool success = false;

            return success;
        }
    }
}
