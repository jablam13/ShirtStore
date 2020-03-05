using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using StoreModel.Generic;
using StoreService.Interface;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ShirtStoreService.Controllers
{
    //[Route("api/[controller]")]
    public class BaseController : Controller
    {

        protected Guid visitorUid = Guid.Empty;
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
            string uid = context.HttpContext.Response.Headers["X-Custom"].ToString();

            if (string.IsNullOrEmpty(uid))
            {
                uid = accountService.GetVisitorUid().Result.ToString();
                context.HttpContext.Response.Headers.Add("X-Custom", uid);
                context.HttpContext.Response.Headers.Add("visitor", uid);
                context.HttpContext.Response.Headers.Add("Access-Control-Expose-Headers", "X-Custom, visitor");
            }
        }

        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            await ConfigureVisitorUid(context);

            await next().ConfigureAwait(false);
        }

        private async Task<string> ConfigureVisitorUid(ActionExecutingContext context)
        {
            string uid = context.HttpContext.Request.Headers["X-Custom"].ToString();

            if (string.IsNullOrEmpty(uid))
            {
                var visitorUid = await accountService.GetVisitorUid().ConfigureAwait(false);
                uid = visitorUid.ToString();
            }

            context.HttpContext.Response.Headers.Add("X-Custom", uid);
            context.HttpContext.Response.Headers.Add("visitor", uid);
            context.HttpContext.Response.Headers.Add("Access-Control-Expose-Headers", "X-Custom, visitor");

            return uid;
        }

        protected Guid GetUserUid()
        {
            if (User != null && (User?.Claims?.Count() ?? 0) > 0)
            {
                Guid.TryParse(User.FindFirst(ClaimTypes.NameIdentifier).Value.ToString(), out userUid);
            }
            return userUid;
        }

        protected Guid GetVisitorUid()
        {
            string uid = httpContextAccessor.HttpContext.Request.Headers["visitor"].ToString();

            if (string.IsNullOrEmpty(uid))
            {
                uid = accountService.GetVisitorUid().Result.ToString();
            }

            return Guid.Parse(uid);
        }
    }
}
