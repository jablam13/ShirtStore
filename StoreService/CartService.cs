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
            return cartRep.GetCartAll(userUid, visitorUid);
        }

        public Cart GetCart(Guid userUid, Guid visitorUid)
        {
            return cartRep.GetCartAll(userUid, visitorUid);
        }

        public List<CartItem> GetCartItems(Guid cartUid)
        {
            return cartRep.GetCartItems(cartUid);
        }

        public List<CartItem> GetCartItems(Guid userUid, Guid visitorUid)
        {
            var cart = cartRep.GetCart(userUid, visitorUid);

            if (cart == null)
            {
                return null;
            }

            return cartRep.GetCartItems(cart.Id);
        }

        public CartItem AddCartItem(StoreItem item, Guid userUid, Guid visitorUid)
        {
            var cart = cartRep.GetCart(userUid, visitorUid);

            if (cart == null) {
                return null;
            }

            return cartRep.AddCartItem(item, cart.Id);
        }

        public CartItem EditCartItem(StoreItem item, Guid userUid, Guid visitorUid)
        {
            var cart = cartRep.GetCart(userUid, visitorUid);

            if (cart == null)
            {
                return null;
            }

            return cartRep.AddCartItem(item, cart.Id);
        }

        public bool RemoveCartItem(CartItem item)
        {
            return cartRep.RemoveCartItem(item);
        }

        public Cart MergeCarts(Guid userUid, Guid visitorUid)
        {
            if (userUid == null && visitorUid == null)
                return null;

            var visitorCart = cartRep.GetVisitorCart(visitorUid);
            var userCart = cartRep.GetUserCart(userUid);

            if (visitorCart == null && userCart == null)
                userCart = cartRep.GetCartAll(userUid, visitorUid);

            if (visitorCart.Id == userCart.Id)
                return userCart;

            if (visitorCart != null && userCart == null)
            {
                //update cart with userUid, userId
                visitorCart.UserUid = userUid;
                userCart = cartRep.UpdateCart(visitorCart);
                //update 
            } 

            return userCart;
        }
    }
}
