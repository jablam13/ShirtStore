using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using StoreModel.Generic;
using StoreModel.Store;
using StoreService.Interface;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ShirtStoreService.Controllers
{
    [Route("api/[controller]")]
    public class CartController : BaseController
    {
        private readonly ICartService cartService;

        public CartController(
            IOptions<AppSettings> _appSettings,
            IHttpContextAccessor _httpContextAccessor,
            IAccountService _userService,
            ICartService _cartService) : base(_appSettings, _httpContextAccessor, _userService)
        {
            cartService = _cartService;
        }

        // GET api/<controller>/5
        [HttpGet("")]
        public Cart Get()
        {
            var userUid = GetUserUid();
            var visitorUid = GetVisitorUid();

            return cartService.GetCart(userUid, visitorUid);
        }

        // POST api/<controller>
        [HttpPost("addtocart")]
        public CartItem AddCartItem([FromBody]StoreItem item)
        {
            var addedCartItem = cartService.AddCartItem(item);

            return addedCartItem;
        }

        // PUT api/<controller>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/<controller>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
