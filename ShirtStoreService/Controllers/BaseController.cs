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
    public abstract class BaseController : Controller
    {
        protected Guid GetUserUid()
        {
            Guid userUid = Guid.Empty;
            if (User != null && (User?.Claims?.Count() ?? 0) > 0)
                Guid.TryParse(User.FindFirst(ClaimTypes.NameIdentifier).Value, out userUid);

            return userUid;
        }

    }
}
