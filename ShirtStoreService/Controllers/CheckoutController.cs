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
using PaymentService.Stripe;
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
        private readonly ICartService _cartService;
        private readonly IAccountService _userService;
        private readonly IOrderService _orderService;
        private readonly IUserVisitorService _userVisitorService;
        private readonly IBraintreeService _braintreeGateway;
        private readonly IStripeService _stripeService;
        private readonly ILogger<CheckoutController> logger;

        public CheckoutController(
            ICartService cartService,
            IOrderService orderService,
            IUserVisitorService userVisitorService,
            IOptions<AppSettings> appSettings,
            IHttpContextAccessor _httpContextAccessor,
            ILogger<CheckoutController> _logger,
            IStripeService stripeService,
            IAccountService userService) : base(appSettings, _httpContextAccessor, userService)
        {
            _cartService = cartService;
            _userService = userService;
            _userVisitorService = userVisitorService;
            _orderService = orderService;
            _stripeService = stripeService;
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
            string ipAddress = _userVisitorService.GetIpValue();

            try
            {
                order = await _orderService.CreateOrder(userUid, ipAddress).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message, ex);
                return BadRequest();
            }

            return Ok(order);
        }

        [Authorize]
        [HttpGet("stripesessiontest")]
        public async Task<IActionResult> StripeSessionTest()
        {
            var userUid = GetUserUid();
            if (userUid == Guid.Empty)
                return Unauthorized();
            string sessionId;

            try
            {
                sessionId = await _stripeService.CreateSessionTest().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message, ex);
                return BadRequest();
            }

            return Ok(sessionId);
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetOrder()
        {
            var userUid = GetUserUid();
            if (userUid == Guid.Empty)
                return Unauthorized();

            var order = await _orderService.GetOrder(userUid, 1).ConfigureAwait(false);
            return Ok(order);
        }

        [Authorize]
        [HttpPost("submit")]
        public async Task<IActionResult> Submit([FromBody]Checkout checkout)
        {
            var userUid = GetUserUid();
            if (userUid == Guid.Empty)
                return Unauthorized();

            await _orderService.ProcessOrder(checkout, userUid).ConfigureAwait(false);

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
