using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Options;
using System.Linq;
using System.Security.Claims;
using StoreModel.Generic;
using StoreService.Interface;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ShirtStoreService.Controllers
{
    //[Route("api/[controller]")]
    public class BaseController : Controller
    {

        protected Guid visitorUid;
        protected Guid userUid = Guid.Empty;
        private readonly AppSettings _appSettings;
        private readonly IAccountService accountService;
        private readonly IHttpContextAccessor httpContextAccessor;

        public BaseController(IOptions<AppSettings> appSettings, IHttpContextAccessor _httpContextAccessor, IAccountService _accountService)
        {
            accountService = _accountService;
            _appSettings = appSettings.Value;
            httpContextAccessor = _httpContextAccessor;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (visitorUid == null || visitorUid == Guid.Empty)
            {
                GetCookie();
            }
        }

        public Guid GetUserUid()
        {
            if (User != null && (User?.Claims?.Count() ?? 0) > 0)
            {
                Guid.TryParse(User.FindFirst(ClaimTypes.NameIdentifier).Value.ToString(), out userUid);
            }
            return userUid;
        }

        private Guid GetCookie()
        {
            //Get cookies from browser
            var cookies = httpContextAccessor.HttpContext.Request.Cookies;
            var _visitorCookie = cookies["VisitorUid"]?.ToString();

            //parse cookie to Guid from string 
            Guid.TryParse(_visitorCookie, out visitorUid);

            //if(cookie doesnt exist, get new VisitorUid from database and add a new cookie for VisitorUid
            if (visitorUid == null || Guid.Empty == visitorUid)
            {
                var cookieOptions = new CookieOptions
                {
                    Expires = DateTime.Now.AddYears(1)
                };

                visitorUid = accountService.GetVisitorUid();
                httpContextAccessor.HttpContext.Response.Cookies.Append("VisitorUid", visitorUid.ToString(), cookieOptions);
            }

            return visitorUid;
        }

        private void UpdateCookie(Guid _visitorUid)
        {
            visitorUid = _visitorUid;

            //Get cookies from browser
            var cookies = httpContextAccessor.HttpContext.Request.Cookies;
            var cookieOptions = new CookieOptions
            {
                Expires = DateTime.Now.AddYears(1)
            };
            httpContextAccessor.HttpContext.Response.Cookies.Append("VisitorUid", visitorUid.ToString(), cookieOptions);
        }



    }
}
