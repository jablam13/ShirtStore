using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using StoreModel.Account;
using StoreModel.Generic;
using StoreService.Interface;
using Stripe;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ShirtStoreService.Controllers
{
    [Route("account")]
    public class AccountController : BaseController
    {
        private readonly IAccountService userService;
        private readonly ICartService cartService;
        private readonly ILogger<AccountController> logger;

        public AccountController(
            IAccountService _userService,
            ICartService _cartService,
            ILogger<AccountController> _logger)
        {
            logger = _logger;
            userService = _userService;
            cartService = _cartService;
        }

        [AllowAnonymous]
        //[ValidateAntiForgeryToken]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserCredentials userCredentials)
        {
            string token;
            try
            {
                var authAttempt = userService.AttemptLoginUser(userCredentials);

                var user = userService.GetLoginUser(userCredentials, authAttempt);

                token = userService.AuthorizeUserJwt(user);
            }
            catch(Exception ex)
            {
                logger.LogError(ex.Message);
                return BadRequest();
            }

            if (string.IsNullOrEmpty(token))
            {
                return NotFound(userCredentials);
            }

            //// update user visitor relation
            //var updateUserVisitor = await userService.UpdateUserVisitor(visitorUid, userUid).ConfigureAwait(false);

            //// merge cart
            //if (updateUserVisitor)
            //{
            //    await cartService.MergeCarts(visitorUid, userUid).ConfigureAwait(false);
            //}
            return Ok(token);
        }

        //[AllowAnonymous]
        //[ValidateAntiForgeryToken]
        [HttpGet("checkauthentication")]
        //[Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public IActionResult CheckAuthenticated()
        {
            var check = User.Identity.IsAuthenticated;
            return Ok(check);
        }

        [AllowAnonymous]
        [HttpGet("isauthenticated")]
        public IActionResult IsAuthenticated()
        {
            //var principal = User as ClaimsPrincipal;
            var check = User.Identity.IsAuthenticated;
            return Ok(check);
        }

        [EnableCors("DevPolicy")]
        [AllowAnonymous]
        //[ValidateAntiForgeryToken]
        [HttpPost("register")]
        public IActionResult Register([FromBody] UserCredentials user)
        {
            var result = userService.RegisterUser(user);

            if(result.Length == 0)
            {
                return BadRequest();
            }

            return Ok(result);
        }

        [Authorize]
        [HttpGet("logout")]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(JwtBearerDefaults.AuthenticationScheme);

            return Ok(CookieAuthenticationDefaults.AuthenticationScheme);
        }
    }
}
