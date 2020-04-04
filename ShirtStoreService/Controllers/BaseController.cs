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
using System.Diagnostics;

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

        protected Guid GetUserUid()
        {
            if (User != null && (User?.Claims?.Count() ?? 0) > 0)
            {
                Guid.TryParse(User.FindFirst(ClaimTypes.NameIdentifier).Value.ToString(), out userUid);
            }
            return userUid;
        }

    }
}
