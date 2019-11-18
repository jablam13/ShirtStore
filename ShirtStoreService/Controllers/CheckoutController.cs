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
using Microsoft.Extensions.Options;
using PaymentService.Braintree;
using StoreModel.Account;
using StoreModel.Checkout;
using StoreModel.Generic;
using StoreModel.Store;
using StoreService.Interface;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ShirtStoreService.Controllers
{
    [Route("checkout")]
    public class CheckoutController : BaseController
    {
        private readonly ICartService cartService;
        private readonly IAccountService userService;
        private readonly IBraintreeService braintreeGateway;

        public CheckoutController(
            ICartService _cartService,
            IOptions<AppSettings> _appSettings,
            IHttpContextAccessor _httpContextAccessor,
            IBraintreeService _braintreeService,
            IAccountService _userService) : base(_appSettings, _httpContextAccessor, _userService)
        {
            cartService = _cartService;
            userService = _userService;
            braintreeGateway = _braintreeService;
        }

        // GET: api/<controller>
        [HttpGet("token")]
        public IActionResult GetClientToken()
        {
            var token = braintreeGateway.GenerateClientToken();
            return Ok(token);
        }

        // GET: api/<controller>
        [HttpPost("nonce")]
        public string GetPaymentNonce([FromBody]string token)
        {
            return braintreeGateway.GeneratePaymentNonce(token);
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet("user")]
        public IActionResult GetUser()
        {
            var uid = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? Guid.Empty.ToString());
            var firstName = User.FindFirst("FirstName")?.Value ?? "EmptyFirstName";
            var lastName = User.FindFirst("LastName")?.Value ?? "EmptyLastName";
            var emailAddress = User.FindFirst(ClaimTypes.Email)?.Value ?? "EmptyEmail";
            var user = new Users()
            {
                Uid = uid,
                FirstName = firstName,
                LastName = lastName,
                EmailAddress = emailAddress,
            };
            //var user = new Users()
            //{
            //    Uid = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value),
            //    FirstName = User.FindFirst("FirstName")?.Value,
            //    LastName = User.FindFirst("LastName")?.Value,
            //    EmailAddress = User.FindFirst(ClaimTypes.Email)?.Value,
            //};

            return Ok(user);
        }

        // GET api/<controller>/5
        [HttpPost("submit")]
        public IActionResult Submit([FromBody]Checkout checkout)
        {
            //validate user 
            //find,create user in braintree,db,identity
            var user = new Users()
            {
                Uid = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value),
                FirstName = User.FindFirst("FirstName")?.Value,
                LastName = User.FindFirst("LastName")?.Value,
                EmailAddress = User.FindFirst(ClaimTypes.Email)?.Value,
            };

            //validate addresses 

            //find,create addresses in braintree,db,identity

            //submit order 

            var result = braintreeGateway.Order(checkout, user);
            return Ok(result);
        }

        // GET api/<controller>/5
        [HttpPost("submittest")]
        public IActionResult SubmitTest([FromBody]CheckoutTest checkout)
        {
            var userGuid = Guid.Empty;


            return Ok(checkout);
        }

        // GET api/<controller>/5
        [HttpPost("submitaddress")]
        public IActionResult SubmitAddress([FromBody]UserAddress address)
        {
            var userGuid = Guid.Empty;


            return Ok(address);
        }

        // GET api/<controller>/5
        [HttpPost("submititems")]
        public IActionResult SubmitOrderItems([FromBody]StoreItem[] items)
        {
            var userGuid = Guid.Empty;


            return Ok(items);
        }

        // POST api/<controller>
        [HttpPost]
        public void Post([FromBody]string value)
        {
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
