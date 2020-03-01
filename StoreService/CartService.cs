using StoreModel.Store;
using StoreRepository.Interface;
using StoreService.Interface;
using System;
using System.Collections.Generic;
using System.Text;

namespace StoreService
{
    public class CartService : BaseService, ICartService
    {
        private readonly ICartRepository cartRep;

        public CartService(
            ICartRepository _cartRep)
        {
            cartRep = _cartRep;
        }

        public Cart GetCartAll(Guid userUid, Guid visitorUid)
        {
            Guid uid = visitorUid;
            bool isVisitor = true;

            if (userUid == Guid.Empty && visitorUid == Guid.Empty)
                return new Cart();

            if (userUid != Guid.Empty)
                uid = userUid;

            return cartRep.GetCartAll(uid, isVisitor);
        }

        public Cart GetCart(Guid userUid, Guid visitorUid)
        {
            Guid uid = visitorUid;
            bool isVisitor = true;

            if (userUid == Guid.Empty && visitorUid == Guid.Empty)
                return new Cart();

            if (userUid != Guid.Empty)
                uid = userUid;

            return cartRep.GetCartAll(uid, isVisitor);
        }

        public List<CartItem> GetCartItems(Guid cartUid)
        {
            return cartRep.GetCartItems(cartUid);
        }

        public List<CartItem> GetCartItems(Guid uid, bool isVisitor)
        {
            return cartRep.GetCartItems(uid, isVisitor);
        }

        public Cart CreateCart(Guid userUid, Guid visitorUid)
        {
            return cartRep.CreateCart(userUid, visitorUid);
        }

        public CartItem AddCartItem(StoreItem cart)
        {
            return new CartItem();
        }

        public Cart EditCart(Cart cart)
        {
            return new Cart();
        }
        public CartItem EditCartItem(Cart cart)
        {
            return new CartItem();
        }
        public bool RemoveCart(Cart cart)
        {
            bool success = false;

            return success;
        }
        public bool RemoveCartItem(CartItem cart)
        {
            bool success = false;

            return success;
        }

        public Cart MergeCarts(Guid userUid, Guid visitorUid)
        {


            return new Cart();
        }
    }
}
