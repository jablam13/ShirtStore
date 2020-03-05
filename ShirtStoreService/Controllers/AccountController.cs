using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
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
        private readonly ILogger<AccountController> logger;

        public AccountController(
            IOptions<AppSettings> _appSettings,
            IHttpContextAccessor _httpContextAccessor,
            IAccountService _userService,
            ILogger<AccountController> _logger) : base(_appSettings, _httpContextAccessor, _userService)
        {
            logger = _logger;
            userService = _userService;
        }

        [AllowAnonymous]
        //[ValidateAntiForgeryToken]
        [HttpPost("login")]
        public IActionResult Login([FromBody] UserCredentials userCredentials)
        {
            string result = userService.LoginUser(userCredentials, User.Identity.IsAuthenticated);

            if (!result.Any())
            {
                return NotFound(userCredentials);
            }

            return Ok(result);
        }

        //[AllowAnonymous]
        //[ValidateAntiForgeryToken]
        [HttpGet("checkauthentication")]
        //[Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public IActionResult CheckAuthenticated()
        {
            var principal = User as ClaimsPrincipal;
            var check = User.Identity.IsAuthenticated;
            return Ok(check);
        }

        [AllowAnonymous]
        [HttpGet("isauthenticated")]
        public IActionResult IsAuthenticated()
        {
            var principal = User as ClaimsPrincipal;
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

            return Ok(result);
        }

        [Authorize]
        [HttpGet("logout")]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            return Ok(CookieAuthenticationDefaults.AuthenticationScheme.ToString());
        }
    }
}
