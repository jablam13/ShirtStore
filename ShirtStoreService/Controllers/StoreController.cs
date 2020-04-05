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
    public class StoreController : BaseController
    {
        private readonly IStoreService _storeService;
        private readonly IUserVisitorService _userVisitorService;
        private Guid defaultStoreUid;

        public StoreController(
            IStoreService storeService,
            IUserVisitorService userVisitorService)
        {
            _storeService = storeService;
            _userVisitorService = userVisitorService;
            defaultStoreUid = Guid.Parse("07A06F5C-6C91-4070-AEF7-7D8E44EB24E5");
        }

        [HttpGet("")]
        [HttpGet("/{suid}")]
        public IActionResult GetStore(Guid? _storeUid)
        {
            var storeUid = _storeUid.HasValue ? _storeUid.Value : defaultStoreUid;
            var userUid = GetUserUid();
            var visitorUid = _userVisitorService.GetVisitorUid().Result;

            // return storeUid != Guid.Empty ? storeService.GetStore(storeUid, userUid, visitorUid) : new Store();
            return Ok(_storeService.GetStore(storeUid, userUid, visitorUid));
        }


        [HttpGet("storeitem/{itemuid}")]
        public IActionResult StoreItem(Guid? itemuid)
        {
            Guid itemUid = itemuid.HasValue ? itemuid.Value : Guid.Empty;

            var visitorUid = _userVisitorService.GetVisitorUid().Result;
            return Ok(_storeService.GetStoreItem(itemUid, GetUserUid(), visitorUid));

        }
    }
}
