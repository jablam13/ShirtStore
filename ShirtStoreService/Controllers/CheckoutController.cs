using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Braintree;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PaymentService.Braintree;
using StoreModel.Account;
using StoreModel.Checkout;
using StoreModel.Generic;
using StoreModel.Store;
using StoreService.Interface;
using Stripe;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ShirtStoreService.Controllers
{
    [Route("api/[controller]")]
    public class CheckoutController : BaseController
    {
        private readonly ICartService cartService;
        private readonly IAccountService userService;
        private readonly IOrderService orderService;
        private readonly IBraintreeService braintreeGateway;
        private readonly ILogger<CheckoutController> logger;

        public CheckoutController(
            ICartService _cartService,
            IOrderService _orderService,
            IOptions<AppSettings> _appSettings,
            IHttpContextAccessor _httpContextAccessor,
            ILogger<CheckoutController> _logger,
            IAccountService _userService) : base(_appSettings, _httpContextAccessor, _userService)
        {
            cartService = _cartService;
            userService = _userService;
            orderService = _orderService;
            logger = _logger;
        }

        [Authorize]
        [HttpGet("load")]
        public async Task<IActionResult> CreateOrder()
        {
            var userUid = GetUserUid();
            if (userUid == Guid.Empty)
                return Unauthorized();

            StoreModel.Checkout.Order order;
            string ipAddress = GetIpValue();

            try
            {
                order = await orderService.CreateOrder(userUid, ipAddress).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message, ex);
                return BadRequest();
            }

            return Ok(order);
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetOrder()
        {
            var userUid = GetUserUid();
            if (userUid == Guid.Empty)
                return Unauthorized();

            string ipAddress = GetIpValue();

            var order = await orderService.GetOrder(userUid, 1).ConfigureAwait(false);
            return Ok(order);
        }

        [Authorize]
        [HttpPost("submit")]
        public async Task<IActionResult> Submit([FromBody]Checkout checkout)
        {
            var userUid = GetUserUid();
            if (userUid == Guid.Empty)
                return Unauthorized();

            var order = await orderService.GetOrder(userUid, 1).ConfigureAwait(false);
            await orderService.ProcessOrder(order).ConfigureAwait(false);

            return Ok();
        }

        [HttpGet("teststripe")]
        public IActionResult TestStripe()
        {
            var options = new PaymentIntentCreateOptions
            {
                Amount = 1000,
                Currency = "usd",
                PaymentMethodTypes = new List<string> { "card" },
                ReceiptEmail = "jenny.rosen@example.com",
            };
            var service = new PaymentIntentService();
            var response = service.Create(options);

            return Ok(response);
        }
    }
}
