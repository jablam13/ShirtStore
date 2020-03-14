using StoreModel.Store;
using StoreRepository.Interface;
using StoreService.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public async Task<Cart> GetCartAll(Guid visitorUid, Guid userUid)
        {
            return await cartRep.GetCartAll(visitorUid, userUid);
        }

        public async Task<Cart> GetCart(Guid visitorUid, Guid userUid)
        {
            return await cartRep.GetCartAll(visitorUid, userUid);
        }

        public async Task<List<CartItem>> GetCartItems(Guid cartUid)
        {
            return await cartRep.GetCartItems(cartUid);
        }

        public async Task<List<CartItem>> GetCartItems(Guid visitorUid, Guid userUid)
        {
            var cart = await cartRep.GetCart(visitorUid, userUid);

            // throw error
            if (cart == null)
            {
                return null;
            }

            return await cartRep.GetCartItems(cart.Id);
        }

        public async Task<CartItem> AddCartItem(StoreItem item, Guid visitorUid, Guid userUid)
        {
            var cart = await cartRep.GetCart(visitorUid, userUid);

            if (cart == null)
            {
                return null;
            }

            return await cartRep.AddCartItem(item, cart.Id);
        }

        public async Task<CartItem> EditCartItem(CartItem item, Guid visitorUid, Guid userUid)
        {
            var cart = await cartRep.GetCart(visitorUid, userUid);

            if (cart == null)
            {
                return null;
            }

            return await cartRep.EditCartItem(item);
        }

        public async Task<Guid> RemoveCartItem(Guid itemUid)
        {
            return await cartRep.RemoveCartItem(itemUid);
        }

        public async Task<Cart> MergeCarts(Guid visitorUid, Guid userUid)
        {
            if (userUid == null && visitorUid == null)
                return null;

            var visitorCart = await cartRep.GetVisitorCart(visitorUid);
            var userCart = await cartRep.GetUserCart(userUid);

            if (visitorCart == null && userCart == null)
                return await cartRep.GetCartAll(visitorUid, userUid);

            if (visitorCart == null && userCart != null)
            {
                return userCart;
            }

            if (visitorCart != null && userCart == null)
            {
                //merge visitorcart into usercart
                visitorCart.UserUid = userUid;
                //update usercart
                var cart = await cartRep.UpdateCartAndItems(visitorCart);

                return userCart;
            }

            if (visitorCart != null && userCart != null)
            {
                //merge visitorcart into usercart
                //if visitor cart and user cart are different carts
                if (userCart.Id != visitorCart.Id && visitorCart.CartItems.Count > 0)
                {

                }

                //if visitor cart and user cart are the same cart with the same items
                if (userCart.Id != visitorCart.Id)
                {
                    userCart = await MergeVisitorIntoUserCart(visitorCart, userCart);
                    visitorCart.CartItems.ForEach(item => item.Active = 0);
                }

                //update usercart
                userCart = await cartRep.UpdateCartAndItems(userCart);

                //deactivate visitorcart
                await cartRep.DeactivateVisitorCart(visitorCart.Uid);

                return userCart;
            }

            return userCart;
        }

        private async Task<Cart> MergeVisitorIntoUserCart(Cart visitorCart, Cart userCart)
        {
            //merge visitor items with same item id into user items with same id
            foreach (var vItem in visitorCart.CartItems)
            {
                foreach (var uItem in userCart.CartItems)
                {
                    if (uItem.Id == vItem.Id && uItem.Price == vItem.Price)
                    {
                        uItem.Quantity = (uItem.Quantity > vItem.Quantity) ? uItem.Quantity : vItem.Quantity;
                    }

                    if (vItem.Id == uItem.Id && vItem.Price != uItem.Price)
                    {
                        userCart.CartItems.Add(vItem);
                    }
                }
            }

            //add visitor items not in user items
            var newItems = new List<CartItem>();

            newItems = visitorCart.CartItems
                .Where(visitorItem =>
                    !userCart.CartItems.Any(userItem => userItem.Id == visitorItem.Id))
                .ToList();
            if (newItems.Count > 0)
                userCart.CartItems.AddRange(newItems);

            return userCart;
        }
    }
}
