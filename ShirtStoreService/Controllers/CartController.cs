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
        private readonly ICartService _cartService;
        private readonly IOrderService _orderService;
        private readonly IUserVisitorService _userVisitorService;

        public CartController(
            IOptions<AppSettings> appSettings,
            IHttpContextAccessor httpContextAccessor,
            IUserVisitorService userVisitorService,
            IAccountService userService,
            IOrderService orderService,
            ICartService cartService) : base(appSettings, httpContextAccessor, userService)
        {
            _cartService = cartService;
            _orderService = orderService;
            _userVisitorService = userVisitorService;
        }

        [HttpGet("")]
        public async Task<Cart> Get()
        {
            var visitorUid = await _userVisitorService.GetVisitorUid().ConfigureAwait(false);
            var userUid = GetUserUid();

            return await _cartService.GetCartAll(visitorUid, userUid).ConfigureAwait(false);
        }

        [HttpPost("add")]
        public async Task<CartItem> AddCartItem([FromBody]StoreItem item)
        {
            var userUid = GetUserUid();
            var visitorUid = await _userVisitorService.GetVisitorUid().ConfigureAwait(false);
            return await _cartService.AddCartItem(item, visitorUid, userUid).ConfigureAwait(false);
        }

        [HttpPut("edit")]
        public async Task<CartItem> EditCartItem([FromBody]CartItem item)
        {
            var userUid = GetUserUid();
            var visitorUid = await _userVisitorService.GetVisitorUid().ConfigureAwait(false);
            return await _cartService.EditCartItem(item, visitorUid, userUid).ConfigureAwait(false);
        }

        [HttpDelete("remove/{uid}")]
        public async Task<Guid> RemoveCartItem(Guid uid)
        {
            return await _cartService.RemoveCartItem(uid).ConfigureAwait(false);
        }

        [HttpPost("submit")]
        public async Task<IActionResult> CreateOrder()
        {
            var userUid = GetUserUid();
            if (userUid == Guid.Empty)
                return Unauthorized();

            var order = await _orderService.GetOrder(userUid, 1);

            return Ok(order);
        }
    }
}
